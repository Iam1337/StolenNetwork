/* Copyright (c) 2019 ExT (V.Sigalkin) */

using System.Collections.Generic;

namespace StolenNetwork
{
    public class PacketInfo
    {
        #region Public Vars

        /// <summary>
        /// Надежность отправления.
        /// </summary>
        public PacketReliability Reliability;

        /// <summary>
        /// Приоритет отправления.
        /// </summary>
        public PacketPriority Priority;

        /// <summary>
        /// Канал надежности.
        /// </summary>
        public byte Channel;

        /// <summary>
        /// Единичный получатель сообщения.
        ///
        /// Одновременно можно задать либо один Connection, либо один Connections.
        /// </summary>
        public Connection Connection;

        /// <summary>
        /// Список получателей сообщения.
        ///
        /// Одновременно можно задать либо один Connection, либо один Connections.
        /// </summary>
        public IEnumerable<Connection> Connections;

        /// <summary>
        /// Исключенный получатель. Используется для оптимизации.
        /// </summary>
        public Connection ExcludeConnection;

        /// <summary>
        /// Массовая рассылка всем соединениями.
        /// </summary>
        public bool Broadcast;

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
            Connection = null;
            ExcludeConnection = null;
        }

        public PacketInfo(IEnumerable<Connection> connections,
                          PacketReliability reliability = PacketReliability.Reliable,
                          PacketPriority priority = PacketPriority.Medium,
                          byte channel = 0)
        {
            Broadcast = false;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = connections;
            Connection = null;
            ExcludeConnection = null;
        }

        public PacketInfo(IEnumerable<Connection> connections,
                          Connection excludeConnection,
                          PacketReliability reliability = PacketReliability.Reliable,
                          PacketPriority priority = PacketPriority.Medium,
                          byte channel = 0)
        {
            Broadcast = false;
            Channel = channel;
            Reliability = reliability;
            Priority = priority;
            Connections = connections;
            Connection = null;
            ExcludeConnection = excludeConnection;
        }

        public PacketInfo(Connection connection,
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
