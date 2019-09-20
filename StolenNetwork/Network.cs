/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace StolenNetwork
{
    public abstract class Network
    {
        #region Public Vars

        public virtual int MaxPacketSize => 10 * 1024 * 1024; // 10Mb.

        public PacketWriter Writer { get; protected set; }

        public PacketReader Reader { get; protected set; }

        #endregion

        #region Private Vars

        #endregion

        #region Protected Methods

        protected Packet CreatePacket(byte packetType, Connection connection)
        {
            var packet = Pool<Packet>.Get();
            packet.Type = packetType;
            packet.Connection = connection;
            packet.Network = this;

            return packet;
        }

        protected void ReleasePacket(ref Packet packet)
        {
            packet.Type = 0;
            packet.Connection = null;
            packet.Network = null;

            Pool<Packet>.Free(ref packet);
        }

        #endregion
    }
}
