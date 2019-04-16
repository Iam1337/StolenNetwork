using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StolenNetwork
{
    public class Server : Network
    {
        #region Static Private Vars

        private static readonly string _serverTickWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.Tick";

        private static readonly string _serverProcessWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessMessage";

        private static readonly string _serverProcessConnectedWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessConnectedMessage";

        private static readonly string _serverProcessUnconnectedWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessUnconnectedMessage";

        #endregion

        #region Extensions

        public interface IHandler
        {
            void PacketProcess(Packet packet);

            void ClientConnected(Connection connection);

            void ClientDisconnected(Connection connection, string reason);

	        void HandshakeCallback(Connection connection, PacketWriter writer);
        }

        #endregion

        #region Public Vars

        public static float MaxReceiveTime = 20f;

        public static ulong MaxPacketsPerSecond = 1500;

        #endregion

        #region Public Vars

        public string Host;

        public ushort Port;

        public ushort ConnectionsCount;

        public IHandler CallbackHandler;

        #endregion

        #region Private Vars

        private Peer _peer;

        private Stopwatch _tickTimer = Stopwatch.StartNew();

        private ushort _previousConnectionId;

        private List<Connection> _connections = new List<Connection>();

        private Dictionary<ulong, Connection> _connectionsGuids = new Dictionary<ulong, Connection>();

        #endregion

        #region Public Methods

        // SETUP
        public virtual bool Start(string host, ushort port, ushort connectionsCount)
        {
            if (_peer != null)
                throw new Exception("[STOLEN NETWORK RAKNET: SERVER] Server is already running.");

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
                throw new Exception("[STOLEN NETWORK RAKNET: SERVER] Server is not running.");

            //TODO: Debug.Log($"[SERVER RAKNET] Server Shutting Down: {reason}");

            Writer.Dispose();
            Writer = null;

            Reader.Dispose();
            Reader = null;

            //using (TimeKeeper.Warning("RakNet: Server.Stop", 20D))
            //{
            Peer.Destroy(ref _peer);
            //}
        }

        // LOOP
        public void Tick()
        {
            if (_peer == null)
                return;

            //using (TimeKeeper.Warning(_serverTickWarning, 20D))
            {
                _tickTimer.Reset();
                _tickTimer.Start();

                while (_peer.IsReceived())
                {
                    //using (TimeKeeper.Warning(_serverProcessWarning, 20D))
                    {
                        ProcessPacket();
                    }


                    var totalMilliseconds = _tickTimer.Elapsed.TotalMilliseconds;
                    if (totalMilliseconds > MaxReceiveTime)
                    {
                        /* TODO:
						Debug.Log($"[SERVER RAKNET] Drop interval: {Time.frameCount - _dropFrame} frames, {Time.time - _dropTime} sec.");

						_dropTime = Time.time;
						_dropFrame = Time.frameCount;
						*/

                        break;
                    }
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
                throw new Exception("[STOLEN NETWORK RAKNET: SERVER] Server is not running.");

            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            if (reason == null)
                throw new ArgumentNullException(nameof(reason));

            if (Writer.Start())
            {
                Writer.PacketId((byte)StolenPacketType.DisconnectReason);
                Writer.String(reason);
                Writer.Send(new PacketInfo(connection,
                                           PacketReliability.ReliableUnordered,
                                           PacketPriority.Immediate));
            }

            //TODO: Debug.Log($"[NETWORK] {connection} dropped: {reason}");

            _peer.CloseConnection(connection.Guid);

            ConnectionDisconnect(connection, reason);
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

            if (CallbackHandler != null)
                CallbackHandler.ClientConnected(connection);
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

            /*
		    if (IsDemoRecording)
		    {
			    RecordMessage(guid);
		    }
			*/

            var connection = GetConnection(guid);
            if (connection != null)
            {
                //using (TimeKeeper.Warning(_serverProcessConnectedWarning, 20D))
                {
                    ProcessConnectedPacket(connection);
                }
            }
            else
            {
                //using (TimeKeeper.Warning(_serverProcessUnconnectedWarning, 20D))
                {
                    ProcessUnconnectedMessage();
                }
            }
        }

        private void ProcessConnectedPacket(Connection connection)
        {
            /* TODO: MPS STATS
            if (connection.GetMPSStats() >= MaxPacketsPerSecond)
            {
                Drop(connection, "Packet Flooding");

                Debug.LogWarning($"[NETWORK] {connection} was kicked for packet flooding");
            }
            else
			*/
            {
                var packetType = Reader.PacketType();

                if (ProcessRakNetPacket(packetType, connection))
                    return;

                //var customId = (byte)(packetId - (byte)RakPacketType.NUMBER_OF_TYPES); //RakNetUtils.LowestUserPacket;
                var packet = CreatePacket(packetType, connection);

                //TODO: connection.AddMessageStats();
                //TODO: connection.AddMessageStats(customId);

                if (CallbackHandler != null)
                    CallbackHandler.PacketProcess(packet);

                ReleasePacket(ref packet);
            }
        }

        private void ProcessUnconnectedMessage()
        {
            ProcessRakNetPacket(Reader.PacketType(), null);
        }

        private bool ProcessRakNetPacket(byte type, Connection connection)
        {
            if (type >= (byte)RakPacketType.NUMBER_OF_TYPES)
                return false;

            var packetType = (RakPacketType)type;
            if (packetType != RakPacketType.NEW_INCOMING_CONNECTION)
            {
                if (packetType != RakPacketType.DISCONNECTION_NOTIFICATION)
                {
                    if (packetType != RakPacketType.CONNECTION_LOST || connection == null)
                        return true;

                    //using (TimeKeeper.Warning("RakNet: Server.OnDisconnected: timed out", 20D))
                    {
                        ConnectionDisconnect(connection, "Timed Out");
                    }

                    return true;
                }

                if (connection != null)
                {
                    //using (TimeKeeper.Warning("RakNet: Server.OnDisconnected: disconnected", 20D))
                    {
                        ConnectionDisconnect(connection, connection.DisconnectReason);
                    }
                }

                return true;
            }

            if (connection == null)
            {
                connection = new Connection();
                connection.Guid = _peer.GetPacketGUID();
                connection.Address = _peer.GetPacketAddress();
                connection.Port = _peer.GetPacketPort();
                connection.State = ConnectionState.Connecting;

                if (string.IsNullOrEmpty(connection.Address) || connection.Address == "UNASSIGNED_SYSTEM_ADDRESS")
                    return false;

                //using (TimeKeeper.Warning("RakNet: Server.CreateConnection", 20D))
                {
                    AddConnection(connection);
                }

                return true;
            }

            // Тут мы ловим другие доступные пакеты RakNet.
            return false;
        }

        #endregion
    }
}
