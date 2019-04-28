using System;

namespace StolenNetwork
{
    public class Connection
    {
        #region Public Vars

        // CONNECTION INFO
        public ConnectionState State;

        public bool IsConnected;

        public ulong Guid;

        public string Address;

        public uint Port;

        public string DisconnectReason;

        public DateTime ConnectionTime;

        // USER INFO
        public object Data;

        #endregion

        #region Public Methods

        // TODO: Add ping methods
        
        public void Clean()
        {
            Guid = 0;
            Data = null;
        }

        public override string ToString()
        {
            return $"{Guid}/{Address}:{Port}/{State}";
        }

        #endregion
    }
}
