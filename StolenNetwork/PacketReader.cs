using System;
using System.IO;
using System.Text;

using StolenNetwork.Converters;

namespace StolenNetwork
{
    public class PacketReader : IDisposable
    {
        #region Static Private Vars

        private static readonly MemoryStream _buffer = new MemoryStream();

        #endregion

        #region Public Vars

        public long Length => _stream.Length;

        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public virtual long Unread => Length - Position;

        #endregion

        #region Private Vars

        private Network _network;

        private Peer _peer;

        private MemoryStream _stream;

        #endregion

        #region Public Methods

        public PacketReader(Peer peer, Network network)
        {
            _network = network;
            _peer = peer;
            _stream = new MemoryStream();
        }

        public bool Start()
        {
            if (_peer == null)
                return false;

            _peer.ReadPacket(_stream);

            return true;
        }

        public byte PacketId()
        {
            return UInt8();
        }

        public byte UInt8()
        {
            return Unread >= Block8.Size ? ReadBlock8().Unsigned : (byte)0;
        }

        public ushort UInt16()
        {
            return Unread >= Block16.Size ? ReadBlock16().Unsigned : (ushort)0;
        }

        public uint UInt32()
        {
            return Unread >= Block32.Size ? ReadBlock32().Unsigned : 0;
        }

        public ulong UInt64()
        {
            return Unread >= Block64.Size ? ReadBlock64().Unsigned : 0;
        }

        public sbyte Int8()
        {
            return Unread >= Block8.Size ? ReadBlock8().Signed : (sbyte)0;
        }

        public short Int16()
        {
            return Unread >= Block16.Size ? ReadBlock16().Signed : (short)0;
        }

        public int Int32()
        {
            return Unread >= Block32.Size ? ReadBlock32().Signed : 0;
        }

        public long Int64()
        {
            return Unread >= Block64.Size ? ReadBlock64().Signed : 0;
        }

        public bool Bool()
        {
            return Unread >= Block8.Size && _stream.ReadByte() != 0;
        }

        public float Float()
        {
            return Unread >= Block32.Size ? ReadBlock32().Float : 0;
        }

        public double Double()
        {
            return Unread >= Block64.Size ? ReadBlock64().Float : 0;
        }

        public int Bytes(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public byte[] Bytes()
        {
            var size = UInt32();
            if (size == 0 || size > _network.MaxPacketSize)
                return null;

            var buffer = new byte[size];

            return Bytes(buffer, 0, (int)size) != size ? null : buffer;
        }

        public MemoryStream MemoryStream()
        {
            var size = UInt32();
            if (size == 0)
                return null;

            // Не более 10Мб.
            if (size > _network.MaxPacketSize)
                return null;

            if (_buffer.Capacity < size)
                _buffer.Capacity = (int)size;

            _buffer.Position = 0;
            _buffer.SetLength(size);

            return Bytes(_buffer.GetBuffer(), 0, (int)size) != size ? null : _buffer;
        }

        public string String()
        {
            var memoryStream = MemoryStream();
            if (memoryStream == null)
                return string.Empty;

            return Encoding.UTF8.GetString(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
        }

        public void Dispose()
        {
            _stream.Dispose();
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

        private Block8 ReadBlock8()
        {
            var offset = CreateOffset(1);
            var buffer = GetBuffer();

            return new Block8
            {
                Byte1 = buffer[offset]
            };
        }

        private Block16 ReadBlock16()
        {
            var readOffset = CreateOffset(2);
            var readBuffer = GetBuffer();

            return new Block16
            {
                Byte1 = readBuffer[readOffset],
                Byte2 = readBuffer[readOffset + 1]
            };
        }

        private Block32 ReadBlock32()
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

        private Block64 ReadBlock64()
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
