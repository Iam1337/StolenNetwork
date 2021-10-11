/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace StolenNetwork
{
	// This part of project work with this CrabNet fork: https://github.com/iam1337/CrabNet
	// Based on https://github.com/grpc/grpc/blob/master/src/csharp/Grpc.Core/

	internal abstract class NativeBackend
	{
		#region Public Methods

		// PEERS
		public abstract IntPtr CreateInstance();

		public abstract void DestroyInstance(IntPtr peer);


		// SERVER
		public abstract int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		public abstract void CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		public abstract int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		public abstract bool IsReceived(IntPtr peer);

		public abstract int GetPacketLength(IntPtr peer);

		public abstract ulong GetPacketGUID(IntPtr peer);

		public abstract IntPtr GetPacketAddressPtr(IntPtr peer);

		public abstract ushort GetPacketPort(IntPtr peer);

		public abstract unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes);


		// SEND
		public abstract void StartPacket(IntPtr peer);

		public abstract unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size);

		public abstract uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		public abstract uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		public abstract void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		public abstract IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		public abstract int GetAveragePing(IntPtr peer, ulong guid);

		public abstract int GetLastPing(IntPtr peer, ulong guid);

		public abstract int GetLowestPing(IntPtr peer, ulong guid);

		#endregion

		#region Protected Methods

		protected NativeBackend()
		{
			Logs.Info($"Initiate {GetType().Name} ");
		}

		#endregion

		#region Private Vars


		#endregion
	};

	internal class NativeBackend_StaticLib : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "__Internal";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_SharedLib : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "RakNet";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_SharedLib_x64 : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "RakNet.x64";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_SharedLib_x86 : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "RakNet.x86";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_SharedLib_x64_dll : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "RakNet.x64.dll";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_SharedLib_x86_dll : NativeBackend
	{
		#region Private Vars

		private const string kLibrary = "RakNet.x86.dll";

		#endregion

		#region Public Methods

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Import Methods

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_CreateInstance();

		[DllImport(kLibrary)]
		private static extern void PEER_DestroyInstance(IntPtr peer);


		// SERVER
		[DllImport(kLibrary)]
		private static extern int PEER_SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

		[DllImport(kLibrary)]
		private static extern void PEER_CloseConnection(IntPtr peer, ulong guid);


		// CLIENT
		[DllImport(kLibrary)]
		private static extern int PEER_SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


		// RECEIVE
		[DllImport(kLibrary)]
		private static extern bool PEER_Receive(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern int PACKET_GetLength(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ulong PACKET_GetGUID(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern IntPtr PACKET_GetAddressPtr(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern ushort PACKET_GetPort(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe bool PACKET_ReadBytes(IntPtr peer, byte* bytes);


		// SEND
		[DllImport(kLibrary)]
		private static extern void PACKET_StartPacket(IntPtr peer);

		[DllImport(kLibrary)]
		private static extern unsafe void PACKET_WriteBytes(IntPtr peer, byte* bytes, uint size);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

		[DllImport(kLibrary)]
		private static extern uint PACKET_SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


		// SHARED
		[DllImport(kLibrary)]
		private static extern void PEER_GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

		[DllImport(kLibrary)]
		private static extern IntPtr PEER_GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

		[DllImport(kLibrary)]
		private static extern int PEER_GetAveragePing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLastPing(IntPtr peer, ulong guid);

		[DllImport(kLibrary)]
		private static extern int PEER_GetLowestPing(IntPtr peer, ulong guid);

		#endregion
	}

	internal class NativeBackend_Unmanaged : NativeBackend
	{
		#region External

		private static class Windows
		{
			[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern IntPtr LoadLibrary(string filename);

			[DllImport("kernel32.dll")]
			internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
		}

		private static class Linux
		{
			[DllImport("libdl.so")]
			internal static extern IntPtr dlopen(string filename, int flags);

			[DllImport("libdl.so")]
			internal static extern IntPtr dlerror();

			[DllImport("libdl.so")]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);
		}

		private static class MacOSX
		{
			[DllImport("libSystem.dylib")]
			internal static extern IntPtr dlopen(string filename, int flags);

			[DllImport("libSystem.dylib")]
			internal static extern IntPtr dlerror();

			[DllImport("libSystem.dylib")]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);
		}

		/// <summary>
		/// On Linux systems, using dlopen and dlsym results in
		/// DllNotFoundException("libdl.so not found") if libc6-dev
		/// is not installed. As a workaround, we load symbols for
		/// dlopen and dlsym from the current process as on Linux
		/// Mono sure is linked against these symbols.
		/// </summary>
		private static class Mono
		{
			[DllImport("__Internal")]
			internal static extern IntPtr dlopen(string filename, int flags);

			[DllImport("__Internal")]
			internal static extern IntPtr dlerror();

			[DllImport("__Internal")]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);
		}

		/// <summary>
		/// Similarly as for Mono on Linux, we load symbols for
		/// dlopen and dlsym from the "libcoreclr.so",
		/// to avoid the dependency on libc-dev Linux.
		/// </summary>
		private static class CoreCLR
		{
			[DllImport("libcoreclr.so")]
			internal static extern IntPtr dlopen(string filename, int flags);

			[DllImport("libcoreclr.so")]
			internal static extern IntPtr dlerror();

			[DllImport("libcoreclr.so")]
			internal static extern IntPtr dlsym(IntPtr handle, string symbol);
		}

		private unsafe class Delegates
		{
			public delegate IntPtr PEER_CreateInstance_delegate();

			public delegate void PEER_DestroyInstance_delegate(IntPtr peer);


			// SERVER
			public delegate int PEER_SetupServer_delegate(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections);

			public delegate void PEER_CloseConnection_delegate(IntPtr peer, ulong guid);


			// CLIENT
			public delegate int PEER_SetupClient_delegate(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout);


			// RECEIVE
			public delegate bool PEER_Receive_delegate(IntPtr peer);

			public delegate int PACKET_GetLength_delegate(IntPtr peer);

			public delegate ulong PACKET_GetGUID_delegate(IntPtr peer);

			public delegate IntPtr PACKET_GetAddressPtr_delegate(IntPtr peer);

			public delegate ushort PACKET_GetPort_delegate(IntPtr peer);

			public delegate bool PACKET_ReadBytes_delegate(IntPtr peer, byte* bytes);


			// SEND
			public delegate void PACKET_StartPacket_delegate(IntPtr peer);

			public delegate void PACKET_WriteBytes_delegate(IntPtr peer, byte* bytes, uint size);

			public delegate uint PACKET_SendPacketUnicast_delegate(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel);

			public delegate uint PACKET_SendPacketBroadcast_delegate(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel);


			// SHARED
			public delegate void PEER_GetStatistics_delegate(IntPtr peer, ulong guid, ref RakNetStatistics statistics);

			public delegate IntPtr PEER_GetStatisticsStringPtr_delegate(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel);

			public delegate int PEER_GetAveragePing_delegate(IntPtr peer, ulong guid);

			public delegate int PEER_GetLastPing_delegate(IntPtr peer, ulong guid);

			public delegate int PEER_GetLowestPing_delegate(IntPtr peer, ulong guid);
		}

		#endregion

		#region Private Vars

		// flags for dlopen
		private const int RTLD_LAZY = 1;
		private const int RTLD_GLOBAL = 8;

		private readonly string _path;

		private readonly IntPtr _handle;

		private Delegates.PEER_CreateInstance_delegate PEER_CreateInstance;
		private Delegates.PEER_DestroyInstance_delegate PEER_DestroyInstance;
		private Delegates.PEER_SetupServer_delegate PEER_SetupServer;
		private Delegates.PEER_CloseConnection_delegate PEER_CloseConnection;
		private Delegates.PEER_SetupClient_delegate PEER_SetupClient;
		private Delegates.PEER_Receive_delegate PEER_Receive;
		private Delegates.PACKET_GetLength_delegate PACKET_GetLength;
		private Delegates.PACKET_GetGUID_delegate PACKET_GetGUID;
		private Delegates.PACKET_GetAddressPtr_delegate PACKET_GetAddressPtr;
		private Delegates.PACKET_GetPort_delegate PACKET_GetPort;
		private Delegates.PACKET_ReadBytes_delegate PACKET_ReadBytes;
		private Delegates.PACKET_StartPacket_delegate PACKET_StartPacket;
		private Delegates.PACKET_WriteBytes_delegate PACKET_WriteBytes;
		private Delegates.PACKET_SendPacketUnicast_delegate PACKET_SendPacketUnicast;
		private Delegates.PACKET_SendPacketBroadcast_delegate PACKET_SendPacketBroadcast;
		private Delegates.PEER_GetStatistics_delegate PEER_GetStatistics;
		private Delegates.PEER_GetStatisticsStringPtr_delegate PEER_GetStatisticsStringPtr;
		private Delegates.PEER_GetAveragePing_delegate PEER_GetAveragePing;
		private Delegates.PEER_GetLastPing_delegate PEER_GetLastPing;
		private Delegates.PEER_GetLowestPing_delegate PEER_GetLowestPing;

		#endregion

		#region Public Methods

		public NativeBackend_Unmanaged(string[] paths)
		{
			_path = FirstValidLibraryPath(paths);
			_handle = PlatformSpecificLoadLibrary(_path, out var errorMsg);

			if (_handle == IntPtr.Zero)
			{
				throw new IOException($"Error loading native library \"{_path}\". {errorMsg}");
			}


			PEER_CreateInstance = GetMethodDelegate<Delegates.PEER_CreateInstance_delegate>();
			PEER_DestroyInstance = GetMethodDelegate<Delegates.PEER_DestroyInstance_delegate>();
			PEER_SetupServer = GetMethodDelegate<Delegates.PEER_SetupServer_delegate>();
			PEER_CloseConnection = GetMethodDelegate<Delegates.PEER_CloseConnection_delegate>();
			PEER_SetupClient = GetMethodDelegate<Delegates.PEER_SetupClient_delegate>();
			PEER_Receive = GetMethodDelegate<Delegates.PEER_Receive_delegate>();
			PACKET_GetLength = GetMethodDelegate<Delegates.PACKET_GetLength_delegate>();
			PACKET_GetGUID = GetMethodDelegate<Delegates.PACKET_GetGUID_delegate>();
			PACKET_GetAddressPtr = GetMethodDelegate<Delegates.PACKET_GetAddressPtr_delegate>();
			PACKET_GetPort = GetMethodDelegate<Delegates.PACKET_GetPort_delegate>();
			PACKET_ReadBytes = GetMethodDelegate<Delegates.PACKET_ReadBytes_delegate>();
			PACKET_StartPacket = GetMethodDelegate<Delegates.PACKET_StartPacket_delegate>();
			PACKET_WriteBytes = GetMethodDelegate<Delegates.PACKET_WriteBytes_delegate>();
			PACKET_SendPacketUnicast = GetMethodDelegate<Delegates.PACKET_SendPacketUnicast_delegate>();
			PACKET_SendPacketBroadcast = GetMethodDelegate<Delegates.PACKET_SendPacketBroadcast_delegate>();
			PEER_GetStatistics = GetMethodDelegate<Delegates.PEER_GetStatistics_delegate>();
			PEER_GetStatisticsStringPtr = GetMethodDelegate<Delegates.PEER_GetStatisticsStringPtr_delegate>();
			PEER_GetAveragePing = GetMethodDelegate<Delegates.PEER_GetAveragePing_delegate>();
			PEER_GetLastPing = GetMethodDelegate<Delegates.PEER_GetLastPing_delegate>();
			PEER_GetLowestPing = GetMethodDelegate<Delegates.PEER_GetLowestPing_delegate>();
		}

		public override IntPtr CreateInstance() => PEER_CreateInstance();

		public override void DestroyInstance(IntPtr peer) => PEER_DestroyInstance(peer);

		public override int SetupServer(IntPtr peer, string serverHost, ushort serverPort, ushort maxConnections) => PEER_SetupServer(peer, serverHost, serverPort, maxConnections);

		public override void CloseConnection(IntPtr peer, ulong guid) => PEER_CloseConnection(peer, guid);

		public override int SetupClient(IntPtr peer, string serverHost, ushort serverPort, uint retries, uint retryDelay, uint timeout) => PEER_SetupClient(peer, serverHost, serverPort, retries, retryDelay, timeout);

		public override bool IsReceived(IntPtr peer) => PEER_Receive(peer);

		public override int GetPacketLength(IntPtr peer) => PACKET_GetLength(peer);

		public override ulong GetPacketGUID(IntPtr peer) => PACKET_GetGUID(peer);

		public override IntPtr GetPacketAddressPtr(IntPtr peer) => PACKET_GetAddressPtr(peer);

		public override ushort GetPacketPort(IntPtr peer) => PACKET_GetPort(peer);

		public override unsafe bool ReadPacketBytes(IntPtr peer, byte* bytes) => PACKET_ReadBytes(peer, bytes);

		public override void StartPacket(IntPtr peer) => PACKET_StartPacket(peer);

		public override unsafe void WritePacketBytes(IntPtr peer, byte* bytes, uint size) => PACKET_WriteBytes(peer, bytes, size);

		public override uint SendPacketUnicast(IntPtr peer, ulong guid, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketUnicast(peer, guid, priority, reliability, channel);

		public override uint SendPacketBroadcast(IntPtr peer, PacketPriority priority, PacketReliability reliability, byte channel) => PACKET_SendPacketBroadcast(peer, priority, reliability, channel);

		public override void GetStatistics(IntPtr peer, ulong guid, ref RakNetStatistics statistics) => PEER_GetStatistics(peer, guid, ref statistics);

		public override IntPtr GetStatisticsStringPtr(IntPtr peer, ulong guid, VerbosityLevel verbosityLevel) => PEER_GetStatisticsStringPtr(peer, guid, verbosityLevel);

		public override int GetAveragePing(IntPtr peer, ulong guid) => PEER_GetAveragePing(peer, guid);

		public override int GetLastPing(IntPtr peer, ulong guid) => PEER_GetLastPing(peer, guid);

		public override int GetLowestPing(IntPtr peer, ulong guid) => PEER_GetLowestPing(peer, guid);

		#endregion

		#region Private Methods

		private string FirstValidLibraryPath(string[] libraryPathAlternatives)
		{
			foreach (var path in libraryPathAlternatives)
			{
				if (File.Exists(path))
				{
					return path;
				}
			}

			throw new FileNotFoundException($"Error loading native library. Not found in any of the possible locations: {string.Join(",", libraryPathAlternatives)}");
		}

		private static IntPtr PlatformSpecificLoadLibrary(string libraryPath, out string errorMsg)
		{
			if (PlatformApi.IsWindows)
			{
				errorMsg = null;
				var handle = Windows.LoadLibrary(libraryPath);
				if (handle == IntPtr.Zero)
				{
					int win32Error = Marshal.GetLastWin32Error();
					errorMsg = $"LoadLibrary failed with error {win32Error}";

					// add extra info for the most common error ERROR_MOD_NOT_FOUND
					if (win32Error == 126)
					{
						errorMsg += ": The specified module could not be found.";
					}
				}

				return handle;
			}

			if (PlatformApi.IsLinux)
			{
				if (PlatformApi.IsMono)
				{
					return LoadLibraryPosix(Mono.dlopen, Mono.dlerror, libraryPath, out errorMsg);
				}

				if (PlatformApi.IsNetCore)
				{
					return LoadLibraryPosix(CoreCLR.dlopen, CoreCLR.dlerror, libraryPath, out errorMsg);
				}

				return LoadLibraryPosix(Linux.dlopen, Linux.dlerror, libraryPath, out errorMsg);
			}

			if (PlatformApi.IsMacOSX)
			{
				return LoadLibraryPosix(MacOSX.dlopen, MacOSX.dlerror, libraryPath, out errorMsg);
			}

			throw new InvalidOperationException("Unsupported platform.");
		}

		private static IntPtr LoadLibraryPosix(Func<string, int, IntPtr> dlopenFunc, Func<IntPtr> dlerrorFunc, string libraryPath, out string errorMsg)
		{
			errorMsg = null;

			var ret = dlopenFunc(libraryPath, RTLD_GLOBAL + RTLD_LAZY);
			if (ret == IntPtr.Zero)
			{
				errorMsg = Marshal.PtrToStringAnsi(dlerrorFunc());
			}

			return ret;
		}

		private IntPtr LoadSymbol(string symbolName)
		{
			if (PlatformApi.IsWindows)
			{
				// See http://stackoverflow.com/questions/10473310 for background on this.
				if (PlatformApi.Is64Bit)
				{
					return Windows.GetProcAddress(_handle, symbolName);
				}
				else
				{
					// Yes, we could potentially predict the size... but it's a lot simpler to just try
					// all the candidates. Most functions have a suffix of @0, @4 or @8 so we won't be trying
					// many options - and if it takes a little bit longer to fail if we've really got the wrong
					// library, that's not a big problem. This is only called once per function in the native library.
					symbolName = "_" + symbolName + "@";
					for (int stackSize = 0; stackSize < 128; stackSize += 4)
					{
						IntPtr candidate = Windows.GetProcAddress(_handle, symbolName + stackSize);
						if (candidate != IntPtr.Zero)
						{
							return candidate;
						}
					}

					// Fail.
					return IntPtr.Zero;
				}
			}

			if (PlatformApi.IsLinux)
			{
				if (PlatformApi.IsMono)
				{
					return Mono.dlsym(_handle, symbolName);
				}

				if (PlatformApi.IsNetCore)
				{
					return CoreCLR.dlsym(_handle, symbolName);
				}

				return Linux.dlsym(_handle, symbolName);
			}

			if (PlatformApi.IsMacOSX)
			{
				return MacOSX.dlsym(_handle, symbolName);
			}

			throw new InvalidOperationException("Unsupported platform.");
		}

		private T GetMethodDelegate<T>() where T : class
		{
			var methodName = RemoveStringSuffix(typeof(T).Name, "_delegate");

			var ptr = LoadSymbol(methodName);
			if (ptr == IntPtr.Zero)
			{
				throw new MissingMethodException($"The native method \"{methodName}\" does not exist");
			}

			return Marshal.GetDelegateForFunctionPointer<T>(ptr);
		}

		private string RemoveStringSuffix(string str, string toRemove)
		{
			return !str.EndsWith(toRemove) ? str : str.Substring(0, str.Length - toRemove.Length);
		}

		#endregion
	}
}