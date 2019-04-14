using System;
using System.IO;
using System.Text;

namespace StolenNetwork
{
    public abstract class PacketReader : IDisposable
    {
        #region Static Private Vars

        private static MemoryStream _buffer = new MemoryStream();

        #endregion

        #region Public Vars

        public Network Network { get; }

        public abstract long Length { get; }

        public abstract long Position { get; set; }

        public virtual long Unread => Length - Position;

        #endregion

        #region Public Methods

        public abstract bool Start();

        public abstract byte PacketId();

        public abstract byte UInt8();

        public abstract ushort UInt16();

        public abstract uint UInt32();

        public abstract ulong UInt64();

        public abstract sbyte Int8();

        public abstract short Int16();

        public abstract int Int32();

        public abstract long Int64();

        public abstract bool Bool();

        public abstract float Float();

        public abstract double Double();

        public abstract int Bytes(byte[] buffer, int offset, int count);

	    public abstract long Seek(long offset, SeekOrigin origin);

        public byte[] Bytes()
        {
            var size = UInt32();
            if (size == 0 || size > Network.MaxPacketSize)
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
            if (size > Network.MaxPacketSize)
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

	    public virtual void Dispose()
	    { }

        #endregion

        #region Protected Methods

        protected PacketReader(Network network)
        {
            Network = network;
        }

        #endregion
    }
}
