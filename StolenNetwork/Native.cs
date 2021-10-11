/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Runtime.InteropServices;

namespace StolenNetwork
{
	// This part of project work with this CrabNet fork: https://github.com/iam1337/CrabNet
	// Based on https://github.com/grpc/grpc/blob/master/src/csharp/Grpc.Core/

	[SuppressUnmanagedCodeSecurity]
	internal static class Native
	{
		#region Private Vars

		private static readonly NativeBackend _b = LoadNativeBackend();

		#endregion

		#region Public Methods

		// PEERS
		public static IntPtr CreateInstance() => _b.CreateInstance();

		public static void DestroyInstance(IntPtr peer) => _b.DestroyInstance(peer);


		// SERVER
		public static int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => _b.SetupServer(peer, serverHost, serverPort, maxConnections);

		public static void CloseConnection(IntPtr peer, ulong guid) => _b.CloseConnection(peer, guid);

		// CLIENT
		public static int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => _b.SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);


		// RECEIVE
		public static bool IsReceived(IntPtr peer) => _b.IsReceived(peer);

		public static int GetPacketLength(IntPtr peer) => _b.GetPacketLength(peer);

		public static ulong GetPacketGUID(IntPtr peer) => _b.GetPacketGUID(peer);

		public static string GetPacketAddress(IntPtr peer) =>  IntPtrToString(_b.GetPacketAddressPtr(peer));

		public static ushort GetPacketPort(IntPtr peer) => _b.GetPacketPort(peer);

		public static unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => _b.ReadPacketBytes(peer, bytes);

		// SEND
		public static void StartPacket(IntPtr peer) => _b.StartPacket(peer);

		public static unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => _b.WritePacketBytes(peer, bytes, size);

		public static uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => _b.SendPacketUnicast(peer, guid, priority, reliability, channel);

		public static uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => _b.SendPacketBroadcast(peer, priority, reliability, channel);


		// SHARED
		public static void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => _b.GetStatistics(peer, guid, ref statistics);

		public static string GetStatisticsString(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => IntPtrToString(_b.GetStatisticsStringPtr(peer, guid, verbosityLevel));

		public static int GetAveragePing(IntPtr peer, ulong guid) => _b.GetAveragePing(peer, guid);

		public static int GetLastPing(IntPtr peer, ulong guid) => _b.GetLastPing(peer, guid);

		public static int GetLowestPing(IntPtr peer, ulong guid) => _b.GetLowestPing(peer, guid);

		#endregion

		#region Private Methods

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

			return LoadBackend();
		}

		private static NativeBackend LoadUnityBackend()
		{
			if (PlatformApi.IsUnityIOS)
			{
				throw new InvalidOperationException("Unsupported platform");
				//return new NativeBackend_StaticLib();
			}

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

		private static NativeBackend LoadBackend()
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static string IntPtrToString(IntPtr pointer) => pointer == IntPtr.Zero ? string.Empty : Marshal.PtrToStringAnsi(pointer);

		#endregion
	}

	#region RakNet Statistics

	// STRUCTS
	public enum StatisticsMetric
	{
		/// <summary>
		/// How many bytes per pushed via a call to SendPacketUnicast or SendPacketBroadcast.
		/// </summary>
		USER_MESSAGE_BYTES_PUSHED,

		/// <summary>
		/// How many user message bytes were sent via a call to SendPacketUnicast or SendPacketBroadcast. This is less than or equal to USER_MESSAGE_BYTES_PUSHED.
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