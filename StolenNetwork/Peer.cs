using System;
using System.IO;

namespace StolenNetwork
{
    public class Peer
    {
	    #region Static Public Methods

	    public static Peer Server(string host, ushort port, ushort connectionsCount)
	    {
		    var peerPointer = Native.PEER_CreateInstance();
		    var result = Native.PEER_SetupServer(peerPointer, host, port, connectionsCount);
		    if (result != 0)
		    {
				if (peerPointer != IntPtr.Zero)
				    Native.PEER_DestroyInstance(peerPointer);

				// TODO: Exceptions.

				return null;
		    }

	        return new Peer { _peerPointer = peerPointer };
        }

	    public static Peer Client(string host, ushort port, uint retries, uint retryDelay, uint timeout)
	    {
		    var peerPointer = Native.PEER_CreateInstance();
		    var result = Native.PEER_SetupClient(peerPointer, host, port, retries, retryDelay, timeout);
		    if (result != 100)
		    {
			    if (peerPointer != IntPtr.Zero)
				    Native.PEER_DestroyInstance(peerPointer);

			    // TODO: Exceptions.
                if (result < 100)
			    {

			    }
			    else
			    {
				    
			    }

				return null;
            }

	        return new Peer { _peerPointer = peerPointer };
        }

        public static void Destroy(ref Peer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
           
            if (peer._peerPointer != IntPtr.Zero)
            {
                Native.PEER_DestroyInstance(peer._peerPointer);
            }

            peer._peerPointer = IntPtr.Zero;
            peer = null;
        }

        #endregion

        #region Private Vars

        private IntPtr _peerPointer;

        #endregion

        #region Public Methods

        private Peer()
        { }

        // SERVER
        public void CloseConnection(ulong guid)
        {
	        Native.PEER_CloseConnection(_peerPointer, guid);
        }

        // RECEIVE
        public bool IsReceived()
        {
            return Native.PEER_Receive(_peerPointer);
        }

        public int GetPacketLength()
        {
            return Native.PACKET_GetLength(_peerPointer);
        }

        public ulong GetPacketGUID()
        {
            return Native.PACKET_GetGUID(_peerPointer);
        }

        public string GetPacketAddress()
        {
            return Native.PACKET_GetAddress(_peerPointer);
        }

        public ushort GetPacketPort()
        {
            return Native.PACKET_GetPort(_peerPointer);
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
                if (!Native.PACKET_ReadBytes(_peerPointer, bufferPointer))
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
	        Native.PACKET_StartPacket(_peerPointer);
        }

        public unsafe void PacketWrite(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            fixed (byte* pointer = stream.GetBuffer())
            {
	            Native.PACKET_WriteBytes(_peerPointer, pointer, (uint)stream.Length);
            }
        }

        public uint PacketBroadcast(PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        return Native.PACKET_SendPacketBroadcast(_peerPointer, priority, reliability, channel);
        }

        public uint PacketSend(ulong guid, PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        return Native.PACKET_SendPacketUnicast(_peerPointer, guid, priority, reliability, channel);
        }

        // SHARED
        public RakNetStatistics GetStatistics()
        {
            var statistics = new RakNetStatistics();

	        Native.PEER_GetStatistics(_peerPointer, 0, ref statistics);

            return statistics;
        }

        public string GetStatisticsString(ulong guid)
        {
            return Native.PEER_GetStatisticsString(_peerPointer, guid, VerbosityLevel.High);
        }

        public int GetConnectionAveragePing(ulong guid)
        {
            return Native.PEER_GetAveragePing(_peerPointer, guid);
        }

        public int GetConnectionLastPing(ulong guid)
        {
            return Native.PEER_GetLastPing(_peerPointer, guid);
        }

        public int GetConnectionLowestPing(ulong guid)
        {
            return Native.PEER_GetLowestPing(_peerPointer, guid);
        }

        #endregion
    }
}
