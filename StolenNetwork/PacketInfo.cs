/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System.Collections.Generic;

namespace StolenNetwork
{
    public class PacketInfo
    {
        #region Public Vars

        public PacketReliability Reliability;

        public PacketPriority Priority;

        public byte Channel;

		public bool Broadcast;

        /// <summary>
        /// Recipient of the message.
        ///
        /// You can specify either one Connection or one Connections at a time.
        /// </summary>
        public IConnection Connection;

        /// <summary>
        /// List of message recipients. 
        ///
        /// You can specify either one Connection or one Connections at a time. 
        /// </summary>
        public IEnumerable<IConnection> Connections;

        /// <summary>
        /// Excluded recipient. Only while Connections is set.
        /// </summary>
        public IConnection ExcludeConnection;

		#endregion

        #region Public Methods

        public PacketInfo(bool broadcast,
                          PacketReliability reliability = PacketReliability.Reliable,
                          PacketPriority priority = PacketPriority.Medium,
                          byte channel = 0)
        {
            Broadcast = broadcast;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = null;
            Connection = default;
            ExcludeConnection = default;
        }

        public PacketInfo(IEnumerable<IConnection> connections,
                          PacketReliability reliability = PacketReliability.Reliable,
                          PacketPriority priority = PacketPriority.Medium,
                          byte channel = 0)
        {
            Broadcast = false;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = connections;
            Connection = default;
            ExcludeConnection = default;
        }

        public PacketInfo(IEnumerable<IConnection> connections,
						  IConnection excludeConnection,
                          PacketReliability reliability = PacketReliability.Reliable,
                          PacketPriority priority = PacketPriority.Medium,
                          byte channel = 0)
        {
            Broadcast = false;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = connections;
            Connection = default;
            ExcludeConnection = excludeConnection;
        }

        public PacketInfo(IConnection connection,
						  PacketReliability reliability = PacketReliability.Reliable,
						  PacketPriority priority = PacketPriority.Medium,
						  byte channel = 0)
        {
            Broadcast = false;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = null;
            Connection = connection;
            ExcludeConnection = null;
        }

        #endregion
    }
}
