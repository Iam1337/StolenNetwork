using System;
using System.Collections.Generic;
using System.Text;

namespace StolenNetwork
{
    public class Connection
    {
        #region Public Vars

        // CONNECTION INFO
        public ConnectionState State;

        public bool Connected;

        public ulong Guid;

        public string Ip;

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
            return $"{Guid}/{Ip}:{Port}/{State}";
        }

        #endregion
    }
}
