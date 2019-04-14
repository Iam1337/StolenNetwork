using System;
using System.Diagnostics;

namespace StolenNetwork.RakNet
{
	public class RakClient : Client
	{
		#region Public Vars

		public static float MaxReceiveTime = 20f;

		#endregion

		#region Private Vars

		private RakPeer _peer;

		private Stopwatch _tickTimer = Stopwatch.StartNew();

		#endregion

		#region Public Methods

		public override bool Connect(string host, ushort port)
		{
			if (_peer != null)
				throw new Exception("[STOLEN NETWORK RAKNET: CLIENT] Client is already running.");

            base.Connect(host, port);

			_peer = RakPeer.Client(host, port, 24, 200, 0);

			if (_peer != null)
			{
				Writer = new RakPacketWriter(_peer, this);
				Reader = new RakPacketReader(_peer, this);

				Connection = new Connection();
				Connection.State = ConnectionState.Unconnected;
				Connection.Connected = false;

				return true;
			}

			return false;
		}

		public override bool IsConnected()
		{
			return _peer != null;
		}

		// TICK
		// GAME LOOP
		public override void Tick()
		{
			//if (IsDemoPlaying)
			//    return;

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
						ProcessMessage();
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
		public override int GetAveragePing()
		{
			return Connection == null ? 1 : _peer.GetConnectionAveragePing(Connection.Guid);
		}

		public override int GetLastPing()
		{
			return Connection == null ? 1 : _peer.GetConnectionLastPing(Connection.Guid);
		}

		public override int GetLowestPing()
		{
			return Connection == null ? 1 : _peer.GetConnectionLowestPing(Connection.Guid);
		}

		#endregion

		#region Protected Methods

		protected override void Disconnect(DisconnectType disconnectType, string reason, bool sendReason = true)
		{
			if (_peer == null)
				throw new Exception("[STOLEN NETWORK RAKNET: CLIENT] Client is not running.");

			if (sendReason && Writer != null && Writer.Start())
			{
				Writer.PacketId((byte) StolenPacketType.DisconnectReason);
				Writer.String(reason);
				Writer.Send(new PacketInfo(Connection,
				                           PacketReliability.ReliableUnordered,
				                           PacketPriority.Immediate));
			}

			//using (TimeKeeper.Warning("RakNet: Client.Disconnect", 20D))
			{
				_peer.Dispose();
				_peer = null;
			}

			Writer.Dispose();
			Writer = null;

			Reader.Dispose();
			Reader = null;

			Host = string.Empty;
			Port = 0;
			Connection = null;

			DisconnectedInternal(disconnectType, reason);
		}

		#endregion

		#region Private Methods

		protected void ProcessMessage()
		{
			Reader.Start();

			/*
			if (IsDemoRecording)
			{
				// На клиенте connectionId не используется.
				RecordMessage(0);
			}
			*/

			var packetId = Reader.PacketId();

			if (ProcessDefaultMessage(packetId))
				return;

			var customId = (byte) (packetId - (byte) DefaultPacketType.USER_PACKET_ENUM); //RakNetUtils.LowestUserPacket;
			//var messageTypesCount = CoreUtils.GetEnumByte<MessageType>();

			if (Connection == null)
			{
				//TODO: Debug.LogWarning($"[CLIENT RAKNET] Ignoring message {(MessageType) messageId} ({messageId}) clientConnection is null");
			}
			else if (Connection.Guid != _peer.GetPacketGUID())
			{
				//TODO: Debug.LogWarning($"[CLIENT RAKNET] Wrong ReceiveId {_rakNet.ReceiveId}");
            }
            /*
			// TODO: А надо ли делать проверку на перебор?
			else if (messageId > messageTypesCount)
			{
				//Debug.LogWarning();
				Disconnect($"Invalid Packet ({messageId}) {_rakNet.ReceiveBytes} bytes", true);
				throw new Exception($"[CLIENT RAKNET] Invalid Packet (higher than {messageTypesCount})");
			}
			*/
            else
			{
				var packet = CreatePacket(customId, Connection); //CreateMessage((MessageType) messageId, Connection);

				if (CallbackHandler != null)
				{
					try
					{
						//using (TimeKeeper.Warning(_clientProcessMessageWarning, 20D))
						{
							CallbackHandler.PacketProcess(packet);
						}
					}
					catch (Exception exception)
					{
						//if (!IsDemoPlaying)

						Disconnect(exception.Message + "\n" + exception.StackTrace);

						throw exception;
					}
				}

				ReleasePacket(ref packet);
			}
		}

		protected bool ProcessDefaultMessage(byte packetId)
		{
			if (packetId >= (byte) DefaultPacketType.USER_PACKET_ENUM)
				return false;

			//if (IsDemoPlaying)
			//    return true;

			var packetType = (DefaultPacketType) packetId;
			if (packetType == DefaultPacketType.CONNECTION_REQUEST_ACCEPTED)
			{
				connectionAccepted = true;

				//if (Connection.Guid != 0)
				// TODO: Debug.LogWarning("[CLIENT RAKNET] Multiple PacketType.CONNECTION_REQUEST_ACCEPTED");

				Connection.Guid = _peer.GetPacketGUID();
				Connection.Connected = true;
				Connection.State = ConnectionState.Connecting;

				if (CallbackHandler != null)
					CallbackHandler.ClientConnected();

                return true;
			}

			if (packetType == DefaultPacketType.CONNECTION_ATTEMPT_FAILED)
			{
				Disconnect(DisconnectType.ConnectionFailed, "Connection Attempt Failed", false);
				return true;
			}

			if (packetType == DefaultPacketType.NO_FREE_INCOMING_CONNECTIONS)
			{
				Disconnect(DisconnectType.FullServer, "Server is Full", false);
				return true;
			}

			if (packetType == DefaultPacketType.DISCONNECTION_NOTIFICATION)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
					Disconnect(DisconnectType.CustomReason, DisconnectReason, false);

				return true;
			}

			if (packetType == DefaultPacketType.CONNECTION_LOST)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
					Disconnect(DisconnectType.ConnectionLost, "Timed Out", false);

				return true;
			}

			if (packetType == DefaultPacketType.CONNECTION_BANNED)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
					Disconnect(DisconnectType.ConnectionBanned, "Connection Banned", false);

				return true;
			}

			if (Connection != null && Connection.Guid != _peer.GetPacketGUID())
			{
				//TODO: Debug.LogWarning($"[CLIENT RAKNET] Unhandled Raknet packet {packetId} from unknown source {_peer.GetPacketAddress()}");
				return true;
			}

			// TODO: Debug.LogWarning("[CLIENT RAKNET] Unhandled Raknet packet " + packetId);
			return true;
		}

		#endregion
	}
}
