using System;
using System.IO;

namespace StolenNetwork.RakNet
{
    public class RakPeer : IDisposable
    {
	    #region Static Public Methods

	    public static RakPeer Server(string host, ushort port, ushort connectionsCount)
	    {
		    var peer = RakNative.PEER_CreateInstance();
		    var result = RakNative.PEER_SetupServer(peer, host, port, connectionsCount);
		    if (result != 0)
		    {
				if (peer != IntPtr.Zero)
				    RakNative.PEER_DestroyInstance(peer);

			    return null;
		    }

		    return new RakPeer(peer);
        }

	    public static RakPeer Client(string host, ushort port, uint retries, uint retryDelay, uint timeout)
	    {
		    var peer = RakNative.PEER_CreateInstance();
		    var result = RakNative.PEER_SetupClient(peer, host, port, retries, retryDelay, timeout);
		    if (result != 100)
		    {
			    if (peer != IntPtr.Zero)
				    RakNative.PEER_DestroyInstance(peer);

			    return null;
            }

			return new RakPeer(peer);
        }

	    #endregion

        #region Private Vars

        private IntPtr _peer;

        #endregion

        #region Public Methods

        public RakPeer(IntPtr peer)
        {
	        _peer = peer; //RakNative.PEER_CreateInstance();
        }

        // DESTROY
        ~RakPeer()
        {
            Dispose();
        }

        public void Dispose()
        {
            // UNMANAGED
            if (_peer != IntPtr.Zero)
            {
	            RakNative.PEER_DestroyInstance(_peer);

                _peer = IntPtr.Zero;
            }
        }

        // SERVER
		/*
        public int Server(string host, ushort port, ushort maxConnections)
        {
            return RakNative.PEER_SetupServer(_peer, host, port, maxConnections);
        }*/

        public void CloseConnection(ulong guid)
        {
	        RakNative.PEER_CloseConnection(_peer, guid);
        }

		/*
        // CLIENT

        public int Client(string host, ushort port, uint retries, uint retryDelay, uint timeout)
        {
            return RakNative.PEER_SetupClient(_peer, host, port, retries, retryDelay, timeout);
        }
		*/

        // RECEIVE
        public bool IsReceived()
        {
            return RakNative.PEER_Receive(_peer);
        }

        public int GetPacketLength()
        {
            return RakNative.PACKET_GetLength(_peer);
        }

        public ulong GetPacketGUID()
        {
            return RakNative.PACKET_GetGUID(_peer);
        }

        public string GetPacketAddress()
        {
            return RakNative.PACKET_GetAddress(_peer);
        }

        public ushort GetPacketPort()
        {
            return RakNative.PACKET_GetPort(_peer);
        }

        public unsafe bool ReadPacket(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var length = GetPacketLength();

            if (stream.Capacity < length)
                stream.Capacity = length + 32;

            stream.SetLength(stream.Capacity);
            stream.Position = 0;

            fixed (byte* bufferPointer = stream.GetBuffer())
            {
                if (!RakNative.PACKET_ReadBytes(_peer, bufferPointer))
                {
                    return false;
                }
            }

            stream.SetLength(length);
            return true;
        }

        // SEND
        public void PacketStart()
        {
	        RakNative.PACKET_StartPacket(_peer);
        }

        public unsafe void PacketWrite(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            fixed (byte* pointer = stream.GetBuffer())
            {
	            RakNative.PACKET_WriteBytes(_peer, pointer, (uint)stream.Length);
            }
        }

        public void PacketBroadcast(PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        RakNative.PACKET_SendPacketBroadcast(_peer, priority, reliability, channel);
        }

        public void PacketSend(ulong guid, PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        RakNative.PACKET_SendPacketUnicast(_peer, guid, priority, reliability, channel);
        }

        // SHARED
        public RakNetStatistics GetStatistics()
        {
            var statistics = new RakNetStatistics();

	        RakNative.PEER_GetStatistics(_peer, 0, ref statistics);

            return statistics;
        }

        public string GetStatisticsString(ulong guid)
        {
            return RakNative.PEER_GetStatisticsString(_peer, guid, VerbosityLevel.High);
        }

        public int GetConnectionAveragePing(ulong guid)
        {
            return RakNative.PEER_GetAveragePing(_peer, guid);
        }

        public int GetConnectionLastPing(ulong guid)
        {
            return RakNative.PEER_GetLastPing(_peer, guid);
        }

        public int GetConnectionLowestPing(ulong guid)
        {
            return RakNative.PEER_GetLowestPing(_peer, guid);
        }

        #endregion
    }
}
