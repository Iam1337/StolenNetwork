using System.IO;
using System.Text;

namespace StolenNetwork
{
    public abstract class PacketWriter
    {
        #region Static Public Vars

        private static MemoryStream _buffer = new MemoryStream();

        #endregion

        #region Public Vars

        public Network Network { get; }

        public virtual long Length => 0;

        public virtual long Position => 0;

        #endregion

        #region Public Methods

        public abstract bool Start();

        public abstract void Send(PacketInfo info);

        public abstract void PacketId(byte packetType);

        public abstract void UInt8(byte value);

        public abstract void UInt16(ushort value);

        public abstract void UInt32(uint value);

        public abstract void UInt64(ulong value);

        public abstract void Int8(sbyte value);

        public abstract void Int16(short value);

        public abstract void Int32(int value);

        public abstract void Int64(long value);

        public abstract void Bool(bool value);

        public abstract void Float(float value);

        public abstract void Double(double value);

        public abstract void Bytes(byte[] buffer, int offset, int count);

        public void Bytes(byte[] buffer, int length)
        {
            if (buffer == null || buffer.Length == 0 || length == 0)
            {
                UInt32(0);
            }
            else if (length > Network.MaxPacketSize)
            {
                UInt32(0);
                // TODO: Debug.LogError("BytesWithSize: Too big " + length);
            }
            else
            {
                UInt32((uint)length);
                Bytes(buffer, 0, length);
            }
        }

        public void Bytes(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
            {
                UInt32(0);
            }
            else if (buffer.Length > Network.MaxPacketSize)
            {
                UInt32(0);
                // TODO: Debug.LogError("BytesWithSize: Too big " + length);
            }
            else
            {
                UInt32((uint) buffer.Length);
                Bytes(buffer, 0, buffer.Length);
            }
        }

        public void MemoryStream(MemoryStream memoryStream)
        {
            if (memoryStream == null || memoryStream.Length == 0)
            {
                UInt32(0);
            }
            else
            {
                Bytes(memoryStream.GetBuffer());
            }
        }

        public void String(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                UInt32(0);
            }
            else
            {
                if (_buffer.Capacity < value.Length * 8)
                    _buffer.Capacity = value.Length * 8;

                _buffer.Position = 0;
                _buffer.SetLength(_buffer.Capacity);

                var bytes = Encoding.UTF8.GetBytes(value, 0, value.Length, _buffer.GetBuffer(), 0);

                _buffer.SetLength(bytes);

                MemoryStream(_buffer);
            }
        }

	    public virtual void Dispose()
	    { }

        #endregion

        #region Protected Methods

        protected PacketWriter(Network network)
        {
            Network = network;
        }

        #endregion
    }
}
