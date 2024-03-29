﻿/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.Diagnostics;

namespace StolenNetwork
{
	public class Client<TConnection> : Network<TConnection> where TConnection : IConnection, new()
	{
		#region Extensions

		public interface IHandler
		{
			#region Methods

			void PacketProcess(Packet<TConnection> packet);

			void ClientConnecting();

			void ClientConnected(PacketReader reader, PacketWriter writer);

			void ClientDisconnected(DisconnectType disconnectType, string reason);

			void SendedPacketAcked(uint packetId);

			void SendedPacketLoss(uint packetId);

			#endregion
		}

		#endregion

		#region Static Public Vars

		public static float MaxReceiveTime = 20f;

		#endregion

		#region Public Vars

		public string Host { get; private set; }

		public ushort Port { get; private set; }

		public TConnection Connection { get; private set; }

		public IHandler CallbackHandler { get; }

		public string DisconnectReason { get; private set; }

		#endregion

		#region Private Vars

		private Peer _peer;

		private readonly Stopwatch _tickTimer = Stopwatch.StartNew();

		#endregion

		#region Public Methods

		public Client(IHandler callbackHandler)
		{
			CallbackHandler = callbackHandler;
		}

		// CONECTING/DISCONECTING
		public virtual StartupResult Connect(string host, ushort port, uint timeout = 0)
		{
			if (_peer != null)
				throw new InvalidOperationException("Client is already running");

			Host = host;
			Port = port;
			DisconnectReason = "Disconnected";

			var result = Peer.Client(host, port, 24, 200, timeout, out _peer);

			if (_peer != null)
			{
				Writer = new PacketWriter(_peer, this);
				Reader = new PacketReader(_peer, this);

				Connection = new TConnection();
				Connection.State = ConnectionState.Unconnected;
				Connection.IsConnected = false;
			}

			return result;
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

			_tickTimer.Reset();
			_tickTimer.Start();

			while (_peer.IsReceived())
			{
				ProcessPacket();

				var totalMilliseconds = _tickTimer.Elapsed.TotalMilliseconds;
				if (totalMilliseconds > MaxReceiveTime || !IsConnected())
				{
					break;
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
				throw new InvalidOperationException("Client is not running.");

			if (sendReason && Writer.Start((byte) StolenPacketType.DisconnectReason))
			{
				Writer.String(reason);
				Writer.Send(new PacketInfo(Connection,
										   PacketReliability.ReliableUnordered,
										   PacketPriority.Immediate));
			}

			Writer.Dispose();
			Writer = null;

			Reader.Dispose();
			Reader = null;

			Host = string.Empty;
			Port = 0;
			Connection = default;

			Peer.Destroy(ref _peer);

			CallbackHandler?.ClientDisconnected(disconnectType, reason);
		}

		#endregion

		#region Private Methods

		protected void ProcessPacket()
		{
			Reader.Start();

			var packetId = Reader.PacketId();

			if (ProcessRakNetPacket(packetId, Reader))
				return;

			if (Connection == null)
			{
				Logs.Warning($"Ignoring packet {packetId}. Client Connection is null");
				return;
			}

			if (Connection.Guid != _peer.GetPacketGUID())
			{
				Logs.Warning($"Wrong ReceiveId {_peer.GetPacketGUID()}");
				return;
			}

			if (ProcessStolenPacket(packetId, Reader))
				return;

			// PROCESS PACKET
			var packet = CreatePacket(packetId, Connection);

			try
			{
				CallbackHandler?.PacketProcess(packet);
			}
			catch (Exception exception)
			{
				Disconnect(exception.Message + "\n" + exception.StackTrace, true);

				throw;
			}

			ReleasePacket(ref packet);
		}

		protected bool ProcessRakNetPacket(byte packetId, PacketReader reader)
		{
			if (packetId >= (byte) RakPacketType.NUMBER_OF_TYPES)
				return false;

			var packetType = (RakPacketType) packetId;
			if (packetType == RakPacketType.SND_RECEIPT_ACKED)
			{
				CallbackHandler?.SendedPacketAcked(reader.UInt32());

				return true;
			}

			if (packetType == RakPacketType.SND_RECEIPT_LOSS)
			{
				CallbackHandler?.SendedPacketLoss(reader.UInt32());

				return true;
			}

			if (packetType == RakPacketType.CONNECTION_REQUEST_ACCEPTED)
			{
				if (Connection.Guid != 0)
				{
					Logs.Warning("Multiple PacketType.CONNECTION_REQUEST_ACCEPTED");
					return true;
				}

				Connection.Guid = _peer.GetPacketGUID();
				Connection.IsConnected = true;
				Connection.State = ConnectionState.Connecting;

				CallbackHandler?.ClientConnecting();

				return true;
			}

			if (packetType == RakPacketType.CONNECTION_ATTEMPT_FAILED)
			{
				Disconnect(DisconnectType.ConnectionFailed, "Connection attempt failed", false);
				Logs.Info("Disconnected: Connection attempt failed");
				return true;
			}

			if (packetType == RakPacketType.NO_FREE_INCOMING_CONNECTIONS)
			{
				Disconnect(DisconnectType.FullServer, "Server is full", false);
				Logs.Info("Disconnected: Server is full");
				return true;
			}

			if (packetType == RakPacketType.DISCONNECTION_NOTIFICATION)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
				{
					Disconnect(DisconnectType.CustomReason, DisconnectReason, false);
					Logs.Info($"Disconnected: {DisconnectReason}");
				}

				return true;
			}

			if (packetType == RakPacketType.CONNECTION_LOST)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
				{
					Disconnect(DisconnectType.ConnectionLost, "Timed out", false);
					Logs.Info("Disconnected: Timed out");
				}

				return true;
			}

			if (packetType == RakPacketType.CONNECTION_BANNED)
			{
				if (Connection == null || Connection.Guid == _peer.GetPacketGUID())
				{
					Disconnect(DisconnectType.ConnectionBanned, "Connection banned", false);
					Logs.Info("Disconnected: Connection banned");
				}

				return true;
			}

			if (Connection != null && Connection.Guid != _peer.GetPacketGUID())
			{
				Logs.Warning($"Unhandled RakNet packet {packetId} from unknown source {_peer.GetPacketAddress()}");
			}

			return true;
		}

		protected bool ProcessStolenPacket(byte packetId, PacketReader reader)
		{
			if (packetId >= (byte) StolenPacketType.NUMBER_OF_TYPES)
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
					throw new InvalidOperationException("Cannot start Handshake message in Client.ProcessStolenPacket");
				}

				return true;
			}

			if (packetType == StolenPacketType.DisconnectReason)
			{
				DisconnectReason = reader.String();

				return true;
			}

			return true;
		}

		#endregion
	}
}