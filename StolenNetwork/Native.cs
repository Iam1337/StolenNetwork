using System;
using System.Security;
using System.Runtime.InteropServices;

namespace StolenNetwork
{
    [SuppressUnmanagedCodeSecurity]
    public static class Native
    {
        #region Externs

        // PEERS
        [DllImport("RakNetDLL")]
        public static extern IntPtr PEER_CreateInstance();

        [DllImport("RakNetDLL")]
        public static extern void PEER_DestroyInstance(IntPtr peer);


        // SERVER
        [DllImport("RakNetDLL")]
        public static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

        [DllImport("RakNetDLL")]
        public static extern void PEER_CloseConnection(IntPtr peer, ulong guid);
        

        // CLIENT
        [DllImport("RakNetDLL")]
        public static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);
        

        // RECEIVE
        [DllImport("RakNetDLL")]
        public static extern bool PEER_Receive(IntPtr peer);

        [DllImport("RakNetDLL")]
        public static extern int PACKET_GetLength(IntPtr peer);

        [DllImport("RakNetDLL")]
        public static extern ulong PACKET_GetGUID(IntPtr peer);

        [DllImport("RakNetDLL")]
        public static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

        public static string PACKET_GetAddress(IntPtr peer) => IntPtrToString(PACKET_GetAddressPtr(peer));

        [DllImport("RakNetDLL")]
        public static extern ushort PACKET_GetPort(IntPtr peer);

        [DllImport("RakNetDLL")]
        public static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


        // SEND
        [DllImport("RakNetDLL")]
        public static extern void PACKET_StartPacket(IntPtr peer);

        [DllImport("RakNetDLL")]
        public static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

        [DllImport("RakNetDLL")]
        public static extern int PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

        [DllImport("RakNetDLL")]
        public static extern int PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


        // SHARED
        [DllImport("RakNetDLL")]
        public static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

        [DllImport("RakNetDLL")]
        public static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

        public static string PEER_GetStatisticsString(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => IntPtrToString(PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel));

        [DllImport("RakNetDLL")]
        public static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

        [DllImport("RakNetDLL")]
        public static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

        [DllImport("RakNetDLL")]
        public static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

        #endregion

        #region Static Private Methods

        public static string IntPtrToString(IntPtr pointer)
        {
            if (pointer == IntPtr.Zero)
                return string.Empty;

            return Marshal.PtrToStringAnsi(pointer);
        }

        #endregion
    }

    #region RakNet Statistins

    // STRUCTS
    public enum StatisticsMetric
    {
        /// <summary>
        /// How many bytes per pushed via a call to PACKET_SendPacketUnicast or PACKET_SendPacketBroadcast.
        /// </summary>
        USER_MESSAGE_BYTES_PUSHED,

        /// <summary>
        /// How many user message bytes were sent via a call to PACKET_SendPacketUnicast or PACKET_SendPacketBroadcast. This is less than or equal to USER_MESSAGE_BYTES_PUSHED.
        /// A message would be pushed, but not yet sent, due to congestion control
        /// </summary>
        USER_MESSAGE_BYTES_SENT,

        /// <summary>
        /// How many user message bytes were resent. A message is resent if it is marked as reliable, and either the message didn't arrive or the message ack didn't arrive.
        /// </summary>
        USER_MESSAGE_BYTES_RESENT,

        /// <summary>
        /// How many user message bytes were received, and returned to the user successfully.
        /// </summary>
        USER_MESSAGE_BYTES_RECEIVED_PROCESSED,

        /// <summary>
        /// How many user message bytes were received, but ignored due to data format errors. This will usually be 0.
        /// </summary>
        USER_MESSAGE_BYTES_RECEIVED_IGNORED,

        /// <summary>
        /// How many actual bytes were sent, including per-message and per-datagram overhead, and reliable message acks
        /// </summary>
        ACTUAL_BYTES_SENT,

        /// <summary>
        /// How many actual bytes were received, including overead and acks.
        /// </summary>
        ACTUAL_BYTES_RECEIVED,

        /// Internal
        NUMBER_OF_METRICS
    }

    public enum VerbosityLevel
    {
        Low,

        Medium,

        High,

        NUMBER_OF_LEVELS
    }

    public struct RakNetStatistics
    {
        #region Public Vars

        /// <summary>
        /// For each type in RNSPerSecondMetrics, what is the value over the last 1 second?
        /// </summary>
        public unsafe fixed ulong ValueOverLastSecond[(int)StatisticsMetric.NUMBER_OF_METRICS];

        /// <summary>
        /// For each type in RNSPerSecondMetrics, what is the total value over the lifetime of the connection?
        /// </summary>
        public unsafe fixed ulong RunningTotal[(int)StatisticsMetric.NUMBER_OF_METRICS];

        /// <summary>
        /// When did the connection start?
        /// </summary>
        public ulong ConnectionStartTime;

        /// <summary>
        /// Is our current send rate throttled by congestion control?
        /// This value should be true if you send more data per second than your bandwidth capacity
        /// </summary>
        public byte IsLimitedByCongestionControl;

        /// <summary>
        /// If isLimitedByCongestionControl is true, what is the limit, in bytes per second?
        /// </summary>
        public ulong BPSLimitByCongestionControl;

        /// <summary>
        /// Is our current send rate throttled by a call to RakPeer::SetPerConnectionOutgoingBandwidthLimit()?
        /// </summary>
        public byte IsLimitedByOutgoingBandwidthLimit;

        /// <summary>
        /// If isLimitedByOutgoingBandwidthLimit is true, what is the limit, in bytes per second?
        /// </summary>
        public ulong BPSLimitByOutgoingBandwidthLimit;

        /// <summary>
        /// For each priority level, how many messages are waiting to be sent out?
        /// </summary>
        public unsafe fixed uint MessageInSendBuffer[4];

        /// <summary>
        /// For each priority level, how many bytes are waiting to be sent out?
        /// </summary>
        public unsafe fixed double BytesInSendBuffer[4];

        /// <summary>
        /// How many messages are waiting in the resend buffer? This includes messages waiting for an ack, so should normally be a small value
        /// If the value is rising over time, you are exceeding the bandwidth capacity. See BPSLimitByCongestionControl
        /// </summary>
        public uint MessagesInResendBuffer;

        /// <summary>
        /// How many bytes are waiting in the resend buffer. See also messagesInResendBuffer
        /// </summary>
        public ulong BytesInResendBuffer;

        /// <summary>
        /// Over the last second, what was our packetloss? This number will range from 0.0 (for none) to 1.0 (for 100%)
        /// </summary>
        public float PacketlossLastSecond;

        /// <summary>
        /// What is the average total packetloss over the lifetime of the connection?
        /// </summary>
        public float PacketlossTotal;

        #endregion
    }

    #endregion
}
