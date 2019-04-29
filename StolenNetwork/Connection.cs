using System;

using StolenNetwork.Internal;

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

        #region Private Vars

        private Network _network;

        private readonly TimeAverageValue[] _packetStats;

        #endregion

        #region Public Methods

        public Connection(Network network)
        {
            _network = network;
            _packetStats = new TimeAverageValue[byte.MaxValue];
        }

        public int GetAveragePing()
        {
            var server = _network as Server;
            if (server != null)
            {
                return server.GetAveragePing(this);
            }

            var client = _network as Client;
            if (client != null)
            {
                return client.GetAveragePing();
            }

            return 0;
        }

        public int GetLastPing()
        {
            var server = _network as Server;
            if (server != null)
            {
                return server.GetLastPing(this);
            }

            var client = _network as Client;
            if (client != null)
            {
                return client.GetLastPing();
            }

            return 0;
        }

        public int GetLowestPing()
        {
            var server = _network as Server;
            if (server != null)
            {
                return server.GetLowestPing(this);
            }

            var client = _network as Client;
            if (client != null)
            {
                return client.GetLowestPing();
            }

            return 0;
        }

        public void AddPacketStats(byte packetId = 0)
        {
            _packetStats[packetId].Increment();
        }

        public ulong GetPacketPerSecond(byte packetId = 0)
        {
            return _packetStats[packetId].Calculate();
        }

        public void Clean()
        {
            Guid = 0;
            Data = null;

            for (var i = 0; i < _packetStats.Length; i++)
                _packetStats[i].Reset();
        }

        public override string ToString()
        {
            return $"{Guid}/{Address}:{Port}/{State}";
        }

        #endregion

        #region Private Methods


        #endregion
    }
}
