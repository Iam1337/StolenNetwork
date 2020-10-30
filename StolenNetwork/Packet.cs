/* Copyright (c) 2020 ExT (V.Sigalkin) */

namespace StolenNetwork
{
    public class Packet<TConnection> where TConnection : IConnection
    {
        #region Public Vars

        public byte Type;

		public TConnection Connection;

        public Network<TConnection> Network;
        
        public PacketReader Reader => Network.Reader;

        public PacketWriter Writer => Network.Writer;

        #endregion
    }
}
