using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StolenNetwork
{
    public class Server : Network
    {
        #region Static Private Vars

        private static readonly string _serverTickWarning = "[STOLEN SERVER] Server.Tick";

        private static readonly string _serverProcessWarning = "[STOLEN SERVER] Server.ProcessMessage";

        private static readonly string _serverProcessConnectedWarning = "[STOLEN SERVER] Server.ProcessConnectedMessage";

        private static readonly string _serverProcessUnconnectedWarning = "[STOLEN SERVER] Server.ProcessUnconnectedMessage";

        #endregion

        #region Extensions

        public interface IHandler
        {
            void PacketProcess(Packet packet);

            void ClientConnecting(Connection connection, PacketWriter writer);

	        void ClientConnected(Connection connection, PacketReader reader);

            void ClientDisconnected(Connection connection, string reason);
        }

        #endregion

        #region Public Vars

        public static float MaxReceiveTime = 20f;

        public static ulong MaxPacketsPerSecond = 1500;

        #endregion

        #region Public Vars

        public Action<string> OnLog;

        public Action OnTickDrop;

        public string Host { get; private set; }

        public ushort Port { get; private set; }

        public ushort ConnectionsCount { get; private set; }

        public IHandler CallbackHandler { get; }

        public List<Connection> Connections => _connections;

        #endregion

        #region Private Vars

        private Peer _peer;

        private Stopwatch _tickTimer = Stopwatch.StartNew();

        private ushort _previousConnectionId;

        private List<Connection> _connections = new List<Connection>();

        private Dictionary<ulong, Connection> _connectionsGuids = new Dictionary<ulong, Connection>();

        #endregion

        #region Public Methods

        public Server(IHandler callbackHandler)
        {
            CallbackHandler = callbackHandler;
        }

        // SETUP
        public virtual bool Start(string host, ushort port, ushort connectionsCount)
        {
            if (_peer != null)
                throw new Exception("[STOLEN SERVER] Server is already running.");

            Host = host;
            Port = port;
            ConnectionsCount = connectionsCount;

            _peer = Peer.Server(host, port, connectionsCount);

            if (_peer != null)
            {
                Writer = new PacketWriter(_peer, this);
                Reader = new PacketReader(_peer, this);

                return true;
            }

            return false;
        }

        public bool IsStarted()
        {
            return _peer != null;
        }

        public void Stop(string reason)
        {
            if (_peer == null)
                throw new Exception("[STOLEN SERVER] Server is not running.");

            if (OnLog != null)
                OnLog.Invoke($"[STOLEN SERVER] Server Shutting Down: {reason}");

            Writer.Dispose();
            Writer = null;

            Reader.Dispose();
            Reader = null;

            Peer.Destroy(ref _peer);
        }

        // LOOP
        public void Tick()
        {
            if (_peer == null)
                return;

            _tickTimer.Reset();
            _tickTimer.Start();

            while (_peer.IsReceived())
            {
                //using (TimeKeeper.Warning(_serverProcessWarning, 20D))
                //{
                ProcessPacket();
                //}

                var totalMilliseconds = _tickTimer.Elapsed.TotalMilliseconds;
                if (totalMilliseconds > MaxReceiveTime)
                {
                    if (OnTickDrop != null)
                        OnTickDrop.Invoke();

                    break;
                }
            }
        }

        // METHODS
        public Connection GetConnection(ulong guid)
        {
            _connectionsGuids.TryGetValue(guid, out var connection);

            return connection;
        }

        public void Kick(Connection connection, string reason)
        {
            if (_peer == null)
                throw new Exception("[STOLEN SERVER] Server is not running.");

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            if (Writer.Start((byte)StolenPacketType.DisconnectReason))
            {
                Writer.String(reason);
                Writer.Send(new PacketInfo(connection,
                                           PacketReliability.ReliableUnordered,
                                           PacketPriority.Immediate));
            }

            if (OnLog != null)
                OnLog.Invoke($"[STOLEN SERVER] {connection} dropped: {reason}");

            _peer.CloseConnection(connection.Guid);

            ConnectionDisconnect(connection, reason);
        }

        // PING
        public int GetAveragePing(Connection connection)
        {
            return _peer.GetConnectionAveragePing(connection.Guid);
        }

        public int GetLastPing(Connection connection)
        {
            return _peer.GetConnectionLastPing(connection.Guid);
        }

        public int GetLowestPing(Connection connection)
        {
            return _peer.GetConnectionLowestPing(connection.Guid);
        }

        #endregion

        #region Protected Methods

        // CONNECTIONS
        protected void ConnectionDisconnect(Connection connection, string reason)
        {
            if (connection == null)
                return;

            connection.IsConnected = false;
            connection.State = ConnectionState.Disconnected;

            if (CallbackHandler != null)
                CallbackHandler.ClientDisconnected(connection, reason);

            RemoveConnection(connection);
        }

        protected void AddConnection(Connection connection)
        {
            if (connection == null)
                return;

            connection.IsConnected = true;
            connection.ConnectionTime = DateTime.Now;

            _connections.Add(connection);
            _connectionsGuids.Add(connection.Guid, connection);
        }

        protected void RemoveConnection(Connection connection)
        {
            _connectionsGuids.Remove(connection.Guid);
            _connections.Remove(connection);

            connection.Clean();
        }

        #endregion

        #region Private Methods

        private void ProcessPacket()
        {
            Reader.Start();

            var guid = _peer.GetPacketGUID();

            var connection = GetConnection(guid);
            if (connection != null)
            {
                ProcessConnectedPacket(connection);
            }
            else
            {
                ProcessUnconnectedMessage();
            }
        }

        private void ProcessConnectedPacket(Connection connection)
        {
            if (connection.GetPacketPerSecond() >= MaxPacketsPerSecond)
            {
                Kick(connection, "Packet Flooding");

                if (OnLog != null)
                    OnLog.Invoke($"[STOLEN SERVER] {connection} was kicked for packet flooding.");

                return;
            }

            var packetId = Reader.PacketId();

            if (ProcessRakNetPacket(packetId, connection))
                return;

            if (ProcessStolenPacket(packetId, connection, Reader))
                return;

            // Process packet.
            var packet = CreatePacket(packetId, connection);

            connection.AddPacketStats();
            connection.AddPacketStats(packetId);

            if (CallbackHandler != null)
                CallbackHandler.PacketProcess(packet);

            ReleasePacket(ref packet);
        }

        private void ProcessUnconnectedMessage()
        {
            ProcessRakNetPacket(Reader.PacketId(), null);
        }

        private bool ProcessRakNetPacket(byte packetId, Connection connection)
        {
            if (packetId >= (byte)RakPacketType.NUMBER_OF_TYPES)
                return false;

            var packetType = (RakPacketType)packetId;
            if (packetType != RakPacketType.NEW_INCOMING_CONNECTION)
            {
                if (packetType != RakPacketType.DISCONNECTION_NOTIFICATION)
                {
                    if (packetType != RakPacketType.CONNECTION_LOST || connection == null)
                        return true;

                    ConnectionDisconnect(connection, "Timed Out");
                    return true;
                }

                if (connection != null)
                {
                    ConnectionDisconnect(connection, connection.DisconnectReason);
                }

                return true;
            }

            if (connection == null)
            {
                connection = new Connection(this);
                connection.Guid = _peer.GetPacketGUID();
                connection.Address = _peer.GetPacketAddress();
                connection.Port = _peer.GetPacketPort();
                connection.State = ConnectionState.Connecting;

                if (string.IsNullOrEmpty(connection.Address) || connection.Address == "UNASSIGNED_SYSTEM_ADDRESS")
                    return true;

                AddConnection(connection);

	            if (Writer.Start((byte) StolenPacketType.Handshake))
	            {
					if (CallbackHandler != null)
			            CallbackHandler.ClientConnecting(connection, Writer);

		            Writer.Send(new PacketInfo(connection));
	            }
	            else
	            {
		            throw new Exception("[STOLEN SERVER] TODO: Write.");
	            }

	            return true;
            }
            
            return true;
        }

	    private bool ProcessStolenPacket(byte packetId, Connection connection, PacketReader reader)
	    {
		    if (packetId >= (byte)StolenPacketType.NUMBER_OF_TYPES)
			    return false;

		    var packetType = (StolenPacketType)packetId;
		    if (packetType == StolenPacketType.Handshake)
		    {
			    connection.State = ConnectionState.Connected;

			    if (CallbackHandler != null)
				    CallbackHandler.ClientConnected(connection, reader);

                return true;
		    }

		    if (packetType == StolenPacketType.DisconnectReason)
		    {
			    connection.DisconnectReason = reader.String();

		        return true;
		    }
			
		    return true;
	    }

        #endregion
    }
}
