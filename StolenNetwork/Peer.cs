/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.IO;

namespace StolenNetwork
{
    public class Peer
    {
	    #region Static Public Methods

		public static StartupResult Server(string host, ushort port, ushort connectionsCount, out Peer peer)
		{
			var peerPointer = Native.CreateInstance();
			var setupResult = Native.SetupServer(peerPointer, host, port, connectionsCount);
			if (setupResult != 0)
			{
				if (peerPointer != IntPtr.Zero)
					Native.DestroyInstance(peerPointer);

				peer = null;
			}
			else
			{
				peer = new Peer {_peerPointer = peerPointer};
			}

			return (StartupResult) setupResult;
		}

		public static StartupResult Client(string host, ushort port, uint retries, uint retryDelay, uint timeout, out Peer peer)
		{
			var peerPointer = Native.CreateInstance();
			var setupResult = Native.SetupClient(peerPointer, host, port, retries, retryDelay, timeout);
			if (setupResult != 100)
			{
				if (peerPointer != IntPtr.Zero)
					Native.DestroyInstance(peerPointer);

				peer = null;
			}
			else
			{
				peer = new Peer {_peerPointer = peerPointer};
			}

			return (StartupResult) setupResult;
		}

		public static void Destroy(ref Peer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
           
            if (peer._peerPointer != IntPtr.Zero)
            {
                Native.DestroyInstance(peer._peerPointer);
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
	        Native.CloseConnection(_peerPointer, guid);
        }

        // RECEIVE
        public bool IsReceived()
        {
            return Native.IsReceived(_peerPointer);
        }

        public int GetPacketLength()
        {
            return Native.GetPacketLength(_peerPointer);
        }

        public ulong GetPacketGUID()
        {
            return Native.GetPacketGUID(_peerPointer);
        }

        public string GetPacketAddress()
        {
            return Native.GetPacketAddress(_peerPointer);
        }

        public ushort GetPacketPort()
        {
            return Native.GetPacketPort(_peerPointer);
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
                if (!Native.ReadPacketBytes(_peerPointer, bufferPointer))
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
	        Native.StartPacket(_peerPointer);
        }

        public unsafe void PacketWrite(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            fixed (byte* pointer = stream.GetBuffer())
            {
	            Native.WritePacketBytes(_peerPointer, pointer, (uint)stream.Length);
            }
        }

        public uint PacketBroadcast(PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        return Native.SendPacketBroadcast(_peerPointer, priority, reliability, channel);
        }

        public uint PacketSend(ulong guid, PacketPriority priority, PacketReliability reliability, byte channel)
        {
	        return Native.SendPacketUnicast(_peerPointer, guid, priority, reliability, channel);
        }

        // SHARED
        public RakNetStatistics GetStatistics()
        {
            var statistics = new RakNetStatistics();

	        Native.GetStatistics(_peerPointer, 0, ref statistics);

            return statistics;
        }

        public string GetStatisticsString(ulong guid)
        {
            return Native.GetStatisticsString(_peerPointer, guid, VerbosityLevel.High);
        }

        public int GetConnectionAveragePing(ulong guid)
        {
            return Native.GetAveragePing(_peerPointer, guid);
        }

        public int GetConnectionLastPing(ulong guid)
        {
            return Native.GetLastPing(_peerPointer, guid);
        }

        public int GetConnectionLowestPing(ulong guid)
        {
            return Native.GetLowestPing(_peerPointer, guid);
        }

        #endregion
    }
}
