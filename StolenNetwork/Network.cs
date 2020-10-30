/* Copyright (c) 2020 ExT (V.Sigalkin) */

namespace StolenNetwork
{
	public abstract class Network
	{
		public virtual int MaxPacketSize => 10 * 1024 * 1024; // 10Mb.
	}

	public abstract class Network<TConnection> : Network where TConnection : IConnection
	{
		#region Public Vars

		public PacketWriter Writer { get; protected set; }

		public PacketReader Reader { get; protected set; }

		#endregion

		#region Private Vars

		#endregion

		#region Protected Methods

		protected Packet<TConnection> CreatePacket(byte packetType, TConnection connection)
		{
			var packet = Pool<Packet<TConnection>>.Get();
			packet.Type = packetType;
			packet.Connection = connection;
			packet.Network = this;

			return packet;
		}

		protected void ReleasePacket(ref Packet<TConnection> packet)
		{
			packet.Type = 0;
			packet.Connection = default;
			packet.Network = null;

			Pool<Packet<TConnection>>.Free(ref packet);
		}

		#endregion
	}
}