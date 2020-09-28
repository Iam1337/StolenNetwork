/* Copyright (c) 2019 ExT (V.Sigalkin) */

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

		public byte Id;

        public ulong Guid;

        public string Address;

        public uint Port;

        public string DisconnectReason;

        public DateTime ConnectionTime;

        // USER INFO
        public object Data;

        #endregion

        #region Private Vars

        private readonly Network _network;

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
            if (_network is Server server)
            {
                return server.GetAveragePing(this);
            }

            if (_network is Client client)
            {
                return client.GetAveragePing();
            }

            return 0;
        }

        public int GetLastPing()
        {
            if (_network is Server server)
            {
                return server.GetLastPing(this);
            }

            if (_network is Client client)
            {
                return client.GetLastPing();
            }

            return 0;
        }

        public int GetLowestPing()
        {
            if (_network is Server server)
            {
                return server.GetLowestPing(this);
            }

            if (_network is Client client)
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
			Id = 0;
            Guid = 0;
            Data = null;
			Address = null;
			State = ConnectionState.Unconnected;
			Port = 0;

            for (var i = 0; i < _packetStats.Length; i++)
                _packetStats[i].Reset();
        }

        public override string ToString()
        {
            return $"{Id}/{Guid}/{Address}:{Port}/{State}";
        }

        #endregion
    }
}
