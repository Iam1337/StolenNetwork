/* Copyright (c) 2020 ExT (V.Sigalkin) */

namespace StolenNetwork
{
    public enum StolenPacketType : byte
    {
		Handshake = RakPacketType.NUMBER_OF_TYPES,

        /// <summary>
        /// Причина отключения.
        /// </summary>
        DisconnectReason,
		
        /// <summary>
        /// Используйте это значение для своих типов.
        /// </summary>
        NUMBER_OF_TYPES
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

    // NATIVE
	public enum StartupResult
	{
		// CLIENT & SERVER
		Started,
		AlreadyStarted,
		InvalidSocketDescriptors,
		InvalidMaxConnections,
		SocketFamilyNotSupported,
		SocketPortAlreadyInUse,
		SocketFailedToBind,
		SocketFailedTestSend,
		PortCannotBeZero,
		FailedToCreateNetworkThread,
		CouldNotGenerateGuid,
		StartupOtherFailure,

		// CLIENT ONLY
		ConnectionAttemptStarted = 100,
		InvalidParameter,
		CannotResolveDomainName,
		AlreadyConnectedToEndpoint,
		ConnectionAttemptAlreadyInProgress,
		SecurityInitializationFailed
    }

	/// <summary>
    /// Packet Reliability.
    /// </summary>
    public enum PacketReliability : byte
    {
	    Unreliable,

	    UnreliableSequenced,

	    ReliableUnordered,

        Reliable,
		
        ReliableSequenced,

	    UnreliableAck,

        ReliableUnorderedAck,

        ReliableAck,
    }

    /// <summary>
    /// Packet Priority.
    /// </summary>
    public enum PacketPriority : byte
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
    /// Packet Types based on RakNet.
    /// </summary>
    public enum RakPacketType : byte
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

        /*
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
        */

        /// <summary>
        /// For the user to use.  Start your first enumeration at this value.
        /// </summary>
        NUMBER_OF_TYPES,
    }
}
