using System;
using System.IO;

using StolenNetwork.Converters;

namespace StolenNetwork.RakNet
{
	public class RakPacketReader : PacketReader
	{
        #region Public Vars

		public override long Length => _stream.Length;

		public override long Position {
			get => _stream.Position;
			set => _stream.Position = value;
		}
		
        #endregion

        #region Private Vars

        private RakPeer _peer;

		private MemoryStream _stream;

        #endregion

        #region Public Methods

        public RakPacketReader(RakPeer peer, Network network) : base(network)
        {
	        _peer = peer;
			_stream = new MemoryStream();
        }
		
		public override bool Start()
		{
			if (_peer == null)
				return false;

			//_stream.Position = 0;
			//_stream.SetLength(0);

			_peer.ReadPacket(_stream);

			return true;
        }

		public override byte PacketId()
		{
			return UInt8();
		}

		public override byte UInt8()
		{
			return Unread >= Block8.Size ? Read8().Unsigned : (byte)0;
		}

		public override ushort UInt16()
		{
			return Unread >= Block16.Size ? Read16().Unsigned : (ushort)0;
		}

		public override uint UInt32()
		{
			return Unread >= Block32.Size ? Read32().Unsigned : 0;
		}

		public override ulong UInt64()
		{
			return Unread >= Block64.Size ? Read64().Unsigned : 0;
		}

		public override sbyte Int8()
		{
			return Unread >= Block8.Size ? Read8().Signed : (sbyte)0;
		}

		public override short Int16()
		{
			return Unread >= Block16.Size ? Read16().Signed : (short)0;
		}

		public override int Int32()
		{
			return Unread >= Block32.Size ? Read32().Signed : 0;
		}

		public override long Int64()
		{
			return Unread >= Block64.Size ? Read64().Signed : 0;
		}

		public override bool Bool()
		{
			return Unread >= Block8.Size && _stream.ReadByte() != 0;
		}

		public override float Float()
		{
			return Unread >= Block32.Size ? Read32().Float : 0;
		}

		public override double Double()
		{
			return Unread >= Block64.Size ? Read64().Float : 0;
		}

		public override int Bytes(byte[] buffer, int offset, int count)
		{
			return _stream.Read(buffer, offset, count);
        }

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _stream.Seek(offset, origin);
        }

        #endregion

        #region Private Methods

		private byte[] GetBuffer()
		{
			return _stream.GetBuffer();
		}

		private long CreateOffset(long offset)
		{
			var position = _stream.Position;

			_stream.Position = position + offset;

			return position;
		}

		private Block8 Read8()
		{
			var offset = CreateOffset(1);
			var buffer = GetBuffer();

			return new Block8
			{
				Byte1 = buffer[offset]
			};
		}

		private Block16 Read16()
		{
			var readOffset = CreateOffset(2);
			var readBuffer = GetBuffer();

			return new Block16
			{
				Byte1 = readBuffer[readOffset],
				Byte2 = readBuffer[readOffset + 1]
			};
		}

		private Block32 Read32()
		{
			var offset = CreateOffset(4);
			var buffer = GetBuffer();

			return new Block32
			{
				Byte1 = buffer[offset],
				Byte2 = buffer[offset + 1],
				Byte3 = buffer[offset + 2],
				Byte4 = buffer[offset + 3]
			};
		}

		private Block64 Read64()
		{
			var offset = CreateOffset(8);
			var buffer = GetBuffer();

			return new Block64
			{
				Byte1 = buffer[offset],
				Byte2 = buffer[offset + 1],
				Byte3 = buffer[offset + 2],
				Byte4 = buffer[offset + 3],
				Byte5 = buffer[offset + 4],
				Byte6 = buffer[offset + 5],
				Byte7 = buffer[offset + 6],
				Byte8 = buffer[offset + 7]
			};
		}

        #endregion
    }
}
