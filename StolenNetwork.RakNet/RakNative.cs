using System;
using System.Security;
using System.Runtime.InteropServices;

namespace StolenNetwork.RakNet
{
    [SuppressUnmanagedCodeSecurity]
    public static class RakNative
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

    #region RakNet Enums

    public enum DefaultPacketType : byte
    {
        //
        // RESERVED TYPES - DO NOT CHANGE THESE
        //

        /// <summary>
        /// Ping from a connected system.  Update timestamps (internal use only)
        /// </summary>
        CONNECTED_PING,

        /// <summary>
        /// Ping from an unconnected system.  Reply but do not update timestamps. (internal use only)
        /// </summary>
        UNCONNECTED_PING,

        /// <summary>
        /// Ping from an unconnected system.  Only reply if we have open connections. Do not update timestamps. (internal use only)
        /// </summary>
        UNCONNECTED_PING_OPEN_CONNECTIONS,

        /// <summary>
        /// Pong from a connected system.  Update timestamps (internal use only)
        /// </summary>
        CONNECTED_PONG,

        /// <summary>
        ///  A reliable packet to detect lost connections (internal use only)
        /// </summary>
        DETECT_LOST_CONNECTIONS,

        /// <summary>
        /// C2S: Initial query: Header(1), OfflineMesageID(16), Protocol number(1), Pad(toMTU), sent with no fragment set.
        /// If protocol fails on server, returns ID_INCOMPATIBLE_PROTOCOL_VERSION to client
        /// </summary>
        OPEN_CONNECTION_REQUEST_1,

        /// <summary>
        /// S2C: Header(1), OfflineMessageID(16), server GUID(8), HasSecurity(1), Cookie(4, if HasSecurity), 
        /// public key (if do security is true), MTU(2). If public key fails on client, returns ID_PUBLIC_KEY_MISMATCH
        /// </summary>
        OPEN_CONNECTION_REPLY_1,

        /// <summary>
        /// C2S: Header(1), OfflineMessageID(16), Cookie(4, if HasSecurity is true on the server), clientSupportsSecurity(1 bit),
        /// handshakeChallenge (if has security on both server and client), remoteBindingAddress(6), MTU(2), client GUID(8)
        /// Connection slot allocated if cookie is valid, server is not full, GUID and IP not already in use.
        /// </summary>
        OPEN_CONNECTION_REQUEST_2,

        /// <summary>
        /// S2C: Header(1), OfflineMesageID(16), server GUID(8), mtu(2), doSecurity(1 bit), handshakeAnswer (if do security is true)
        /// </summary>
        OPEN_CONNECTION_REPLY_2,

        /// <summary>
        /// C2S: Header(1), GUID(8), Timestamp, HasSecurity(1), Proof(32)
        /// </summary>
        CONNECTION_REQUEST,

        /// <summary>
        /// RakPeer - Remote system requires secure connections, pass a public key to RakPeerInterface::Connect()
        /// </summary>
        REMOTE_SYSTEM_REQUIRES_PUBLIC_KEY,

        /// <summary>
        /// RakPeer - We passed a public key to RakPeerInterface::Connect(), but the other system did not have security turned on.
        /// </summary>
        OUR_SYSTEM_REQUIRES_SECURITY,

        /// <summary>
        /// RakPeer - Wrong public key passed to RakPeerInterface::Connect()
        /// </summary>
        PUBLIC_KEY_MISMATCH,

        /// <summary>
        /// RakPeer - Same as ID_ADVERTISE_SYSTEM, but intended for internal use rather than being passed to the user.
        /// Second byte indicates type. Used currently for NAT punchthrough for receiver port advertisement. See ID_NAT_ADVERTISE_RECIPIENT_PORT
        /// </summary>
        OUT_OF_BAND_INTERNAL,

        /// <summary>
        /// If RakPeerInterface::Send() is called where PacketReliability contains _WITH_ACK_RECEIPT, then on a later call to
        /// RakPeerInterface::Receive() you will get ID_SND_RECEIPT_ACKED or ID_SND_RECEIPT_LOSS. The message will be 5 bytes long,
        /// and bytes 1-4 inclusive will contain a number in native order containing a number that identifies this message.
        /// This number will be returned by RakPeerInterface::Send() or RakPeerInterface::SendList(). ID_SND_RECEIPT_ACKED means that
        /// the message arrived
        /// </summary>
        SND_RECEIPT_ACKED,

        /// <summary>
        /// If RakPeerInterface::Send() is called where PacketReliability contains UNRELIABLE_WITH_ACK_RECEIPT, then on a later call to
        /// RakPeerInterface::Receive() you will get ID_SND_RECEIPT_ACKED or ID_SND_RECEIPT_LOSS. The message will be 5 bytes long,
        /// and bytes 1-4 inclusive will contain a number in native order containing a number that identifies this message. This number
        /// will be returned by RakPeerInterface::Send() or RakPeerInterface::SendList(). ID_SND_RECEIPT_LOSS means that an ack for the
        /// message did not arrive (it may or may not have been delivered, probably not). On disconnect or shutdown, you will not get
        /// ID_SND_RECEIPT_LOSS for unsent messages, you should consider those messages as all lost.
        /// </summary>
        SND_RECEIPT_LOSS,

        //
        // USER TYPES - DO NOT CHANGE THESE
        //

        /// <summary>
        /// RakPeer - In a client/server environment, our connection request to the server has been accepted.
        /// </summary>
        CONNECTION_REQUEST_ACCEPTED,

        /// <summary>
        /// RakPeer - Sent to the player when a connection request cannot be completed due to inability to connect.
        /// </summary>
        CONNECTION_ATTEMPT_FAILED,

        /// <summary>
        /// RakPeer - Sent a connect request to a system we are currently connected to.
        /// </summary>
        ALREADY_CONNECTED,

        /// <summary>
        /// RakPeer - A remote system has successfully connected.
        /// </summary>
        NEW_INCOMING_CONNECTION,

        /// <summary>
        /// RakPeer - The system we attempted to connect to is not accepting new connections.
        /// </summary>
        NO_FREE_INCOMING_CONNECTIONS,

        /// <summary>
        /// RakPeer - The system specified in Packet::systemAddress has disconnected from us.  For the client, this would mean the
        /// server has shutdown.
        /// </summary>
        DISCONNECTION_NOTIFICATION,

        /// <summary>
        /// RakPeer - Reliable packets cannot be delivered to the system specified in Packet::systemAddress.  The connection to that
        /// system has been closed.
        /// </summary>
        CONNECTION_LOST,

        /// <summary>
        /// RakPeer - We are banned from the system we attempted to connect to.
        /// </summary>
        CONNECTION_BANNED,

        /// <summary>
        /// RakPeer - The remote system is using a password and has refused our connection because we did not set the correct password.
        /// </summary>
        INVALID_PASSWORD,

        /// <summary>
        /// This means the two systems cannot communicate.
        /// The 2nd byte of the message contains the value of RakNET_PROTOCOL_VERSION for the remote system
        /// </summary>
        INCOMPATIBLE_PROTOCOL_VERSION,

        /// <summary>
        ///	Means that this IP address connected recently, and can't connect again as a security measure. See
        /// RakPeer::SetLimitIPConnectionFrequency()
        /// </summary>
        IP_RECENTLY_CONNECTED,

        /// <summary>
        /// RakPeer - The sizeof(RakNetTime) bytes following this byte represent a value which is automatically modified by the difference
        /// in system times between the sender and the recipient. Requires that you call SetOccasionalPing.
        /// </summary>
        TIMESTAMP,

        /// <summary>
        /// RakPeer - Pong from an unconnected system.  First byte is ID_UNCONNECTED_PONG, second sizeof(RakNet::TimeMS) bytes is the ping,
        /// following bytes is system specific enumeration data.
        /// Read using bitstreams
        /// </summary>
        UNCONNECTED_PONG,

        /// <summary>
        /// RakPeer - Inform a remote system of our IP/Port. On the recipient, all data past ID_ADVERTISE_SYSTEM is whatever was passed to
        /// the data parameter
        /// </summary>
        ADVERTISE_SYSTEM,

        /// <summary>
        /// RakPeer - Downloading a large message. Format is ID_DOWNLOAD_PROGRESS (MessageID), partCount (unsigned int), partTotal (unsigned int),
        /// partLength (unsigned int), first part data (length &amp;lt;= MAX_MTU_SIZE). See the three parameters partCount, partTotal
        /// and partLength in OnFileProgress in FileListTransferCBInterface.h
        /// </summary>
        DOWNLOAD_PROGRESS,

        // PLUGINS?
        REMOTE_DISCONNECTION_NOTIFICATION,
        REMOTE_CONNECTION_LOST,
        REMOTE_NEW_INCOMING_CONNECTION,
        FILE_LIST_TRANSFER_HEADER,
        FILE_LIST_TRANSFER_FILE,
        FILE_LIST_REFERENCE_PUSH_ACK,
        DDT_DOWNLOAD_REQUEST,
        TRANSPORT_STRING,
        REPLICA_MANAGER_CONSTRUCTION,
        REPLICA_MANAGER_SCOPE_CHANGE,
        REPLICA_MANAGER_SERIALIZE,
        REPLICA_MANAGER_DOWNLOAD_STARTED,
        REPLICA_MANAGER_DOWNLOAD_COMPLETE,
        RAKVOICE_OPEN_CHANNEL_REQUEST,
        RAKVOICE_OPEN_CHANNEL_REPLY,
        RAKVOICE_CLOSE_CHANNEL,
        RAKVOICE_DATA,
        AUTOPATCHER_GET_CHANGELIST_SINCE_DATE,
        AUTOPATCHER_CREATION_LIST,
        AUTOPATCHER_DELETION_LIST,
        AUTOPATCHER_GET_PATCH,
        AUTOPATCHER_PATCH_LIST,
        AUTOPATCHER_REPOSITORY_FATAL_ERROR,
        AUTOPATCHER_CANNOT_DOWNLOAD_ORIGINAL_UNMODIFIED_FILES,
        AUTOPATCHER_FINISHED_INTERNAL,
        AUTOPATCHER_FINISHED,
        AUTOPATCHER_RESTART_APPLICATION,
        NAT_PUNCHTHROUGH_REQUEST,
        //NAT_GROUP_PUNCHTHROUGH_REQUEST,
        //NAT_GROUP_PUNCHTHROUGH_REPLY,
        NAT_CONNECT_AT_TIME,
        NAT_GET_MOST_RECENT_PORT,
        NAT_CLIENT_READY,

        //NAT_GROUP_PUNCHTHROUGH_FAILURE_NOTIFICATION,
        NAT_TARGET_NOT_CONNECTED,
        NAT_TARGET_UNRESPONSIVE,
        NAT_CONNECTION_TO_TARGET_LOST,
        NAT_ALREADY_IN_PROGRESS,
        NAT_PUNCHTHROUGH_FAILED,
        NAT_PUNCHTHROUGH_SUCCEEDED,
        //NAT_GROUP_PUNCH_FAILED,
        //NAT_GROUP_PUNCH_SUCCEEDED,
        READY_EVENT_SET,
        READY_EVENT_UNSET,
        READY_EVENT_ALL_SET,
        READY_EVENT_QUERY,
        LOBBY_GENERAL,
        RPC_REMOTE_ERROR,
        RPC_PLUGIN,
        FILE_LIST_REFERENCE_PUSH,
        READY_EVENT_FORCE_ALL_SET,
        ROOMS_EXECUTE_FUNC,
        ROOMS_LOGON_STATUS,
        ROOMS_HANDLE_CHANGE,
        LOBBY2_SEND_MESSAGE,
        LOBBY2_SERVER_ERROR,
        FCM2_NEW_HOST,
        FCM2_REQUEST_FCMGUID,
        FCM2_RESPOND_CONNECTION_COUNT,
        FCM2_INFORM_FCMGUID,
        FCM2_UPDATE_MIN_TOTAL_CONNECTION_COUNT,
        ID_FCM2_VERIFIED_JOIN_START,
        ID_FCM2_VERIFIED_JOIN_CAPABLE,
        ID_FCM2_VERIFIED_JOIN_FAILED,
        ID_FCM2_VERIFIED_JOIN_ACCEPTED,
        ID_FCM2_VERIFIED_JOIN_REJECTED,
        UDP_PROXY_GENERAL,
        SQLite3_EXEC,
        SQLite3_UNKNOWN_DB,
        SQLLITE_LOGGER,
        NAT_TYPE_DETECTION_REQUEST,
        NAT_TYPE_DETECTION_RESULT,
        ROUTER_2_INTERNAL,
        ROUTER_2_FORWARDING_NO_PATH,
        ROUTER_2_FORWARDING_ESTABLISHED,
        ROUTER_2_REROUTED,
        TEAM_BALANCER_INTERNAL,
        TEAM_BALANCER_REQUESTED_TEAM_FULL,
        TEAM_BALANCER_REQUESTED_TEAM_LOCKED,
        TEAM_BALANCER_TEAM_REQUESTED_CANCELLED,
        TEAM_BALANCER_TEAM_ASSIGNED,
        LIGHTSPEED_INTEGRATION,
        XBOX_LOBBY,
        TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_SUCCESS,
        TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_SUCCESS,
        TWO_WAY_AUTHENTICATION_INCOMING_CHALLENGE_FAILURE,
        TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_FAILURE,
        TWO_WAY_AUTHENTICATION_OUTGOING_CHALLENGE_TIMEOUT,
        TWO_WAY_AUTHENTICATION_NEGOTIATION,
        CLOUD_POST_REQUEST,
        CLOUD_RELEASE_REQUEST,
        CLOUD_GET_REQUEST,
        CLOUD_GET_RESPONSE,
        CLOUD_UNSUBSCRIBE_REQUEST,
        CLOUD_SERVER_TO_SERVER_COMMAND,
        CLOUD_SUBSCRIPTION_NOTIFICATION,

        // Lib voice?
        ID_LIB_VOICE,
        ID_RELAY_PLUGIN,
        ID_NAT_REQUEST_BOUND_ADDRESSES,
        ID_NAT_RESPOND_BOUND_ADDRESSES,
        ID_FCM2_UPDATE_USER_CONTEXT,

        RESERVED_3,
        RESERVED_4,
        RESERVED_5,
        RESERVED_6,
        RESERVED_7,
        RESERVED_8,
        RESERVED_9,


        /// <summary>
        /// Reseived by AidsRak.
        /// </summary>
        DisconnectReason,

        /// <summary>
        /// For the user to use.  Start your first enumeration at this value.
        /// </summary>
        USER_PACKET_ENUM,



    }

	/*
/// <summary>
/// These enumerations are used to describe when packets are delivered.
/// </summary>
public enum PacketPriority
{
	/// <summary>
	/// The highest possible priority. These message trigger sends immediately, and are generally not buffered or aggregated into a single datagram.
	/// </summary>
	Immediate,

	/// <summary>
	/// For every 2 IMMEDIATE_PRIORITY messages, 1 HIGH_PRIORITY will be sent.
	/// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control.
	/// </summary>
	High,

	/// <summary>
	/// For every 2 HIGH_PRIORITY messages, 1 MEDIUM_PRIORITY will be sent.
	/// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control.
	/// </summary>
	Medium,

	/// <summary>
	/// For every 2 MEDIUM_PRIORITY messages, 1 LOW_PRIORITY will be sent.
	/// Messages at this priority and lower are buffered to be sent in groups at 10 millisecond intervals to reduce UDP overhead and better measure congestion control.
	/// </summary>
	Low,

	/// Internal
	NUMBER_OF_PRIORITIES
}


/// <summary>
/// These enumerations are used to describe how packets are delivered.
/// </summary>
public enum PacketReliability
{
	/// <summary>
	/// Same as regular UDP, except that it will also discard duplicate datagrams.  RakNet adds (6 to 17) + 21 bits of overhead, 16 of which is used to detect duplicate packets and 6 to 17 of which is used for message length.
	/// </summary>
	Unreliable,

	/// <summary>
	/// Regular UDP with a sequence counter.  Out of order messages will be discarded.
	/// Sequenced and ordered messages sent on the same channel will arrive in the order sent.
	/// </summary>
	UnreliableSequenced,

	/// <summary>
	/// The message is sent reliably, but not necessarily in any order.  Same overhead as UNRELIABLE.
	/// </summary>
	Reliable,

	/// <summary>
	/// This message is reliable and will arrive in the order you sent it.  Messages will be delayed while waiting for out of order messages.  Same overhead as UNRELIABLE_SEQUENCED.
	/// Sequenced and ordered messages sent on the same channel will arrive in the order sent.
	/// </summary>
	ReliableOrdered,

	/// <summary>
	/// This message is reliable and will arrive in the sequence you sent it.  Out or order messages will be dropped.  Same overhead as UNRELIABLE_SEQUENCED.
	/// Sequenced and ordered messages sent on the same channel will arrive in the order sent.
	/// </summary>
	ReliableSequenced,

	/// <summary>
	/// Same as UNRELIABLE, however the user will get either ID_SND_RECEIPT_ACKED or ID_SND_RECEIPT_LOSS based on the result of sending this message when calling RakPeerInterface::Receive(). Bytes 1-4 will contain the number returned from the Send() function. On disconnect or shutdown, all messages not previously acked should be considered lost. 
	/// </summary>
	UnreliableWithAckReceipt,

	/// <summary>
	/// Same as RELIABLE. The user will also get ID_SND_RECEIPT_ACKED after the message is delivered when calling RakPeerInterface::Receive(). ID_SND_RECEIPT_ACKED is returned when the message arrives, not necessarily the order when it was sent. Bytes 1-4 will contain the number returned from the Send() function. On disconnect or shutdown, all messages not previously acked should be considered lost. This does not return ID_SND_RECEIPT_LOSS.
	/// </summary>
	ReliableWithAckReceipt,

	/// <summary>
	/// Same as RELIABLE_ORDERED_ACK_RECEIPT. The user will also get ID_SND_RECEIPT_ACKED after the message is delivered when calling RakPeerInterface::Receive(). ID_SND_RECEIPT_ACKED is returned when the message arrives, not necessarily the order when it was sent. Bytes 1-4 will contain the number returned from the Send() function. On disconnect or shutdown, all messages not previously acked should be considered lost. This does not return ID_SND_RECEIPT_LOSS.
	/// </summary>
	ReliableOrderedWithAckReceipt,

	/// Internal
	NUMBER_OF_RELIABILITIES
}
*/

    #endregion

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
