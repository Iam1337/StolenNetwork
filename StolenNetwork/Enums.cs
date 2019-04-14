namespace StolenNetwork
{
    public enum StolenPacketType : byte
    {
        /// <summary>
        /// Причина отключения.
        /// </summary>
        DisconnectReason = 0,
    }

    /// <summary>
    /// Connection states.
    /// </summary>
    public enum ConnectionState
    {
        Unconnected,

        Connecting,

        Connected,

        Disconnected
    }

    /// <summary>
    /// Packet Reliability.
    /// </summary>
    public enum PacketReliability
    {
        Reliable,

        ReliableUnordered,

        ReliableSequenced,

        Unreliable,

        UnreliableSequenced,
    }

    /// <summary>
    /// Packet Priority.
    /// </summary>
    public enum PacketPriority
    {
        /// <summary>
        /// Самый высокий приоритет. Сообщение не отправляется незамедлительно, оно так же не буферизуется и не агрегируется в одну датаграму.
        /// </summary>
        Immediate,

        /// <summary>
        /// Сообщение высокого приоритета. На каждые два Immediate сообщение отправляется одно Hight.
        ///
        /// Такие сообщения агрегируются в группы отправляемые каждые 10 милисекнуд.
        /// </summary>
        High,

        /// <summary>
        /// Сообщение среднего приоритета. На каждые два High сообщение отправляется одно Medium.
        ///
        /// Такие сообщения агрегируются в группы отправляемые каждые 10 милисекнуд.
        /// </summary>
        Medium,

        /// <summary>
        /// Сообщение низкого приоритета. На каждые два Medium сообщение отправляется одно Low.
        ///
        /// Такие сообщения агрегируются в группы отправляемые каждые 10 милисекнуд.
        /// </summary>
        Low,
    }

    /// <summary>
    /// Disconnect Types.
    /// </summary>
    public enum DisconnectType
    {
        CustomReason,

        ConnectionFailed,

        FullServer,

        ConnectionLost,

        ConnectionBanned
    }
}
