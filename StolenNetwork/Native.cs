/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace StolenNetwork
{
	// This part of project work with this CrabNet fork: https://github.com/iam1337/CrabNet

	[SuppressUnmanagedCodeSecurity]
	internal static class Native
	{
		#region Private Vars

		private static readonly NativeBackend _b;

		#endregion

		#region Contstructor

		static Native()
		{
			_b = LoadNativeBackend();
		}

		private static NativeBackend LoadNativeBackend()
		{
			if (PlatformApi.IsUnity)
			{
				return LoadUnityBackend();
			}

			if (PlatformApi.IsXamarin)
			{
				return LoadXamarinBackend();
			}

			return LoadSingleFileAppBackend();
		}

		private static NativeBackend LoadUnityBackend()
		{
			if (PlatformApi.IsUnityIOS)
			{
				throw new InvalidOperationException("Unsupported platform");
				//return new NativeBackend_StaticLib();
			}

			// most other platforms load unity plugins as a shared library
			return new NativeBackend_SharedLib();
		}

		private static NativeBackend LoadXamarinBackend()
		{
			if (PlatformApi.IsXamarinAndroid)
			{
				return new NativeBackend_SharedLib();
			}

			throw new InvalidOperationException("Unsupported platform");
			//return new NativeBackend_StaticLib();
		}

		private static NativeBackend LoadSingleFileAppBackend()
		{
			if (PlatformApi.Architecture == Architecture.X64)
			{
				if (PlatformApi.IsWindows)
				{
					return new NativeBackend_SharedLib_x64_dll();
				}

				return new NativeBackend_SharedLib_x64();
			}

			if (PlatformApi.Architecture == Architecture.X86)
			{
				if (PlatformApi.IsWindows)
				{
					return new NativeBackend_SharedLib_x86_dll();
				}

				return new NativeBackend_SharedLib_x86();
			}

			throw new InvalidOperationException($"Unsupported architecture \"{PlatformApi.Architecture}\".");
		}

		#endregion

		#region Externs

		// PEERS
		public static IntPtr PEER_CreateInstance() => _b._PEER_CreateInstance();

		public static void PEER_DestroyInstance(IntPtr peer) => _b._PEER_DestroyInstance(peer);


		// SERVER
		public static int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => _b._PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public static void PEER_CloseConnection(IntPtr peer, ulong guid) => _b._PEER_CloseConnection(peer, guid);


		// CLIENT
		public static int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => _b._PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);


		// RECEIVE
		public static bool PEER_Receive(IntPtr peer) => _b._PEER_Receive(peer);

		public static int PACKET_GetLength(IntPtr peer) => _b._PACKET_GetLength(peer);

		public static ulong PACKET_GetGUID(IntPtr peer) => _b._PACKET_GetGUID(peer);

		public static string PACKET_GetAddress(IntPtr peer) => _b._PACKET_GetAddress(peer);

		public static ushort PACKET_GetPort(IntPtr peer) => _b._PACKET_GetPort(peer);

		public static unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes) => _b._PACKET_ReadBytes(peer, bytes);

		// SEND
		public static void PACKET_StartPacket(IntPtr peer) => _b._PACKET_StartPacket(peer);

		public static unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size) => _b._PACKET_WriteBytes(peer, bytes, size);

		public static uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => _b._PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public static uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => _b._PACKET_SendPacketBroadcast(peer, priority, reliability, channel);


		// SHARED
		public static void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => _b._PEER_GetStatistics(peer, guid, ref statistics);

		public static string PEER_GetStatisticsString(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => _b._PEER_GetStatisticsString(peer, guid, verbosityLevel);

		public static int PEER_GetAveragePing(IntPtr peer, ulong guid) => _b._PEER_GetAveragePing(peer, guid);

		public static int PEER_GetLastPing(IntPtr peer, ulong guid) => _b._PEER_GetLastPing(peer, guid);

		public static int PEER_GetLowestPing(IntPtr peer, ulong guid) => _b._PEER_GetLowestPing(peer, guid);

		#endregion

	}

	#region RakNet Statistics

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
		public unsafe fixed ulong ValueOverLastSecond[(int) StatisticsMetric.NUMBER_OF_METRICS];

		/// <summary>
		/// For each type in RNSPerSecondMetrics, what is the total value over the lifetime of the connection?
		/// </summary>
		public unsafe fixed ulong RunningTotal[(int) StatisticsMetric.NUMBER_OF_METRICS];

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