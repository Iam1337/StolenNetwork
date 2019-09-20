/* Copyright (c) 2019 ExT (V.Sigalkin) */

namespace StolenNetwork
{
    public class Packet
    {
        #region Public Vars

        public byte Type;

        public Network Network;

        public Connection Connection;

        public PacketReader Reader => Network.Reader;

        public PacketWriter Writer => Network.Writer;

        #endregion
    }
}
