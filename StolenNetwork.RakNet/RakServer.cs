using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace StolenNetwork.RakNet
{
    public class RakServer : Server
    {
	    #region Static Private Vars

	    private static readonly string _serverTickWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.Tick";

	    private static readonly string _serverProcessWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessMessage";

	    private static readonly string _serverProcessConnectedWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessConnectedMessage";

	    private static readonly string _serverProcessUnconnectedWarning = "[STOLEN NETWORK RAKNET: SERVER] RakServer.ProcessUnconnectedMessage";

        #endregion

        #region Public Vars

	    public static float MaxReceiveTime = 20f;

	    public static ulong MaxPacketsPerSecond = 1500;

        #endregion

        #region Private Vars

        private RakPeer _peer;

	    private Stopwatch _tickTimer = Stopwatch.StartNew();

        #endregion

        #region Public Methods
		
        public override bool Start(string host, ushort port, ushort connectionsCount)
	    {
			if (_peer != null)
				throw new Exception("[STOLEN NETWORK RAKNET: SERVER] Server is already running.");

            base.Start(host, port, connectionsCount);

            _peer = RakPeer.Server(host, port, connectionsCount);

		    if (_peer != null)
		    {
				Writer = new RakPacketWriter(_peer, this);
				Reader = new RakPacketReader(_peer, this);

			    return true;
		    }

		    return false;
	    }

	    public override bool IsStarted()
	    {
		    return _peer != null;
	    }

	    public override void Stop(string reason)
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
		    _peer.Dispose();
		    _peer = null;
		    //}
        }

		// LOOP
	    public override void Tick()
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
					    ProcessMessage();
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

		// CONNECTIONS
	    public override void Kick(Connection connection, string reason)
	    {
		    if (_peer == null)
			    throw new Exception("[STOLEN NETWORK RAKNET: SERVER] Server is not running.");

			if (connection == null)
				throw new ArgumentNullException(nameof(connection));

			if (reason == null)
				throw new ArgumentNullException(nameof(reason));

		    if (Writer.Start())
		    {
			    Writer.PacketId((byte) StolenPacketType.DisconnectReason);
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

	    #region Private Methods

	    private void ProcessMessage()
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
				    ProcessConnectedMessage(connection);
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

        private void ProcessConnectedMessage(Connection connection)
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
                var packetId = Reader.PacketId();

                if (ProcessDefaultPacket(packetId, connection))
                    return;

	            var customId = (byte) (packetId - (byte) DefaultPacketType.USER_PACKET_ENUM); //RakNetUtils.LowestUserPacket;
                var packet = CreatePacket((byte)customId, connection);

                //connection.AddMessageStats();
                //connection.AddMessageStats(customId);

                if (CallbackHandler != null)
                    CallbackHandler.PacketProcess(packet);

                ReleasePacket(ref packet);
            }
        }

        private void ProcessUnconnectedMessage()
        {
            ProcessDefaultPacket(Reader.PacketId(), null);
        }

        private bool ProcessDefaultPacket(byte type, Connection connection)
        {
            if (type >= (byte) DefaultPacketType.USER_PACKET_ENUM)
                return false;

	        var packetType = (DefaultPacketType) type;
            if (packetType != DefaultPacketType.NEW_INCOMING_CONNECTION)
            {
                if (packetType != DefaultPacketType.DISCONNECTION_NOTIFICATION)
                {
                    if (packetType != DefaultPacketType.CONNECTION_LOST || connection == null)
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

	        return false;
        }

        #endregion
    }
}
