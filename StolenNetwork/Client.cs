using System;
using System.Diagnostics;

namespace StolenNetwork
{
    public class Client : Network
    {
        #region Extensions

	    public interface IHandler
        {
		    #region Methods

		    void PacketProcess(Packet packet);

	        void ClientConnecting();

	        void ClientConnected(PacketReader reader, PacketWriter writer);

            void ClientDisconnected(DisconnectType disconnectType, string reason);
			
		    #endregion
	    }

        #endregion

        #region Static Public Vars

        public static float MaxReceiveTime = 20f;

        #endregion

        #region Public Vars

        public string Host { get; private set; }

	    public ushort Port { get; private set; }

	    public Connection Connection { get; private set; }

	    public IHandler CallbackHandler;

	    public static string DisconnectReason;

        #endregion

        #region Private Vars

        private Peer _peer;

        private Stopwatch _tickTimer = Stopwatch.StartNew();

        #endregion

	    #region Public Methods

	    // CONECTING/DISCONECTING
	    public virtual bool Connect(string host, ushort port)
	    {
	        if (_peer != null)
	            throw new Exception("[STOLEN CLIENT] Client is already running.");

            Host = host;
		    Port = port;
	        DisconnectReason = "Disconnected";

            _peer = Peer.Client(host, port, 24, 200, 0);

            if (_peer != null)
	        {
	            Writer = new PacketWriter(_peer, this);
	            Reader = new PacketReader(_peer, this);

	            Connection = new Connection();
	            Connection.State = ConnectionState.Unconnected;
	            Connection.IsConnected = false;

	            return true;
	        }
            
		    return false;
	    }

	    public virtual void Disconnect(string reason, bool sendReason)
	    {
            Disconnect(DisconnectType.CustomReason, reason, sendReason);
	    }

        public bool IsConnected()
        {
            return _peer != null && Connection != null && Connection.IsConnected;
        }

	    // GAME LOOP
        public void Tick()
        {
            if (_peer == null)
                return;

            //using (TimeKeeper.Warning(_clientTickWarning, 20D))
            {
                _tickTimer.Reset();
                _tickTimer.Start();

                while (_peer.IsReceived())
                {
                    //using (TimeKeeper.Warning(_clientProcessMessageWarning, 20D))
                    {
                        ProcessPacket();
                    }

                    var totalMilliseconds = _tickTimer.Elapsed.TotalMilliseconds;
                    if (totalMilliseconds > MaxReceiveTime || !IsConnected())
                    {
                        /*
						Debug.Log($"[CLIENT RAKNET] Drop interval: {Time.frameCount - _dropFrame} frames, {Time.time - _dropTime} sec.");

						_dropTime = Time.time;
						_dropFrame = Time.frameCount;
						*/
                        break;
                    }
                }
            }
        }

	    // PING
        public int GetAveragePing()
        {
            return Connection == null ? 1 : _peer.GetConnectionAveragePing(Connection.Guid);
        }
        
        public int GetLastPing()
        {
            return Connection == null ? 1 : _peer.GetConnectionLastPing(Connection.Guid);
        }

        public int GetLowestPing()
        {
            return Connection == null ? 1 : _peer.GetConnectionLowestPing(Connection.Guid);
        }

        #endregion

        #region Protected Methods

        protected void Disconnect(DisconnectType disconnectType, string reason, bool sendReason)
        {
            if (_peer == null)
                throw new Exception("[STOLEN CLIENT] Client is not running.");

            if (sendReason && Writer.Start((byte)StolenPacketType.DisconnectReason))
            {
                Writer.String(reason);
                Writer.Send(new PacketInfo(Connection,
                                           PacketReliability.ReliableUnordered,
                                           PacketPriority.Immediate));
            }

            //using (TimeKeeper.Warning("RakNet: Client.Disconnect", 20D))
            {
                Peer.Destroy(ref _peer);
            }

            Writer.Dispose();
            Writer = null;

            Reader.Dispose();
            Reader = null;

            Host = string.Empty;
            Port = 0;
            Connection = null;

            if (CallbackHandler != null)
                CallbackHandler.ClientDisconnected(disconnectType, reason);
        }

        #endregion

        #region Private Methods

	    protected void ProcessPacket()
	    {
		    Reader.Start();

		    var packetId = Reader.PacketId();

		    if (ProcessRakNetPacket(packetId))
			    return;

		    if (Connection == null)
		    {
			    throw new Exception($"[STOLEN CLIENT] Ignoring packet {packetId}. Client Connection is null.");
		    }

		    if (Connection.Guid != _peer.GetPacketGUID())
		    {
			    throw new Exception($"[STOLEN CLIENT]  Wrong ReceiveId {_peer.GetPacketGUID()}");
		    }

		    if (ProcessStolenPacket(packetId, Reader))
			    return;

			// PROCESS PACKET
		    var packet = CreatePacket(packetId, Connection);

		    try
		    {
			    //using (TimeKeeper.Warning(_clientProcessMessageWarning, 20D))
			    {
				    if (CallbackHandler != null)
					    CallbackHandler.PacketProcess(packet);
			    }
		    }
		    catch (Exception exception)
		    {
			    Disconnect(exception.Message + "\n" + exception.StackTrace, true);

			    throw;
		    }

		    ReleasePacket(ref packet);
	    }

	    protected bool ProcessRakNetPacket(byte packetId)
        {
            if (packetId >= (byte)RakPacketType.NUMBER_OF_TYPES)
                return false;

            //if (IsDemoPlaying)
            //    return true;

            var packetType = (RakPacketType)packetId;
            if (packetType == RakPacketType.CONNECTION_REQUEST_ACCEPTED)
            {
                //connectionAccepted = true;

                if (Connection.Guid != 0)
					throw new Exception($"[STOLEN CLIENT] Multiple PacketType.CONNECTION_REQUEST_ACCEPTED.");

                Connection.Guid = _peer.GetPacketGUID();
                Connection.IsConnected = true;
                Connection.State = ConnectionState.Connecting;

                if (CallbackHandler != null)
                    CallbackHandler.ClientConnecting();

                return true;
            }

            if (packetType == RakPacketType.CONNECTION_ATTEMPT_FAILED)
            {
                Disconnect(DisconnectType.ConnectionFailed, "Connection Attempt Failed", false);
                return true;
            }

            if (packetType == RakPacketType.NO_FREE_INCOMING_CONNECTIONS)
            {
                Disconnect(DisconnectType.FullServer, "Server is Full", false);
                return true;
            }

            if (packetType == RakPacketType.DISCONNECTION_NOTIFICATION)
            {
                if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
                    Disconnect(DisconnectType.CustomReason, DisconnectReason, false);

                return true;
            }

            if (packetType == RakPacketType.CONNECTION_LOST)
            {
                if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
                    Disconnect(DisconnectType.ConnectionLost, "Timed Out", false);

                return true;
            }

            if (packetType == RakPacketType.CONNECTION_BANNED)
            {
                if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
                    Disconnect(DisconnectType.ConnectionBanned, "Connection Banned", false);

                return true;
            }

            if (Connection != null && Connection.Guid != _peer.GetPacketGUID())
            {
				throw new Exception($"[STOLEN CLIENT] Unhandled Raknet packet {packetId} from unknown source {_peer.GetPacketAddress()}");
            }

			//TODO: Тут я сделал возврат False, если я все же хочу видеть все пакеты раковые.
            // TODO: Debug.LogWarning("[CLIENT RAKNET] Unhandled Raknet packet " + packetId);
            return true;
        }

	    protected bool ProcessStolenPacket(byte packetId, PacketReader reader)
	    {
		    if (packetId >= (byte)StolenPacketType.NUMBER_OF_TYPES)
			    return false;
			
		    var packetType = (StolenPacketType) packetId;
		    if (packetType == StolenPacketType.Handshake)
		    {
			    Connection.State = ConnectionState.Connected;

			    if (Writer.Start((byte) StolenPacketType.Handshake))
			    {
					if (CallbackHandler != null)
					    CallbackHandler.ClientConnected(reader, Writer);

				    Writer.Send(new PacketInfo(Connection));
			    }
			    else
			    {
				    throw new Exception("TODO");
			    }

			    return true;
		    }

		    if (packetType == StolenPacketType.DisconnectReason)
		    {
			    DisconnectReason = reader.String();

			    return true;
		    }

			// TODO: Process stolen packets.
			return true;
	    }

        #endregion
    }
}
