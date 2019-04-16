﻿using System;
using System.IO;
using System.Text;

using StolenNetwork.Converters;

namespace StolenNetwork
{
    public class PacketWriter : IDisposable
    {
        #region Static Public Vars

        private static MemoryStream _buffer = new MemoryStream();

        #endregion

        #region Public Vars

        public long Length => _stream.Length;

        public long Position {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        #endregion

        #region Private Vars

        private Network _network;

        private Peer _peer;

        private MemoryStream _stream;

        #endregion

        #region Public Methods

        public PacketWriter(Peer peer, Network network)
        {
            _network = network;
            _peer = peer;
            _stream = new MemoryStream();
        }

        public bool Start()
        {
            if (_peer == null)
                return false;

            _stream.Position = 0;
            _stream.SetLength(0);

            return true;
        }

        public void Send(PacketInfo info)
        {
            if (info.Broadcast)
            {
                _peer.PacketStart();
                _peer.PacketWrite(_stream);
                _peer.PacketBroadcast(info.Priority, info.Reliability, (byte)info.Channel);

                return;
            }

            if (info.Connections != null)
            {
                foreach (var connection in info.Connections)
                {
                    if (connection == info.ExcludeConnection) continue;

                    _peer.PacketStart();
                    _peer.PacketWrite(_stream);
                    _peer.PacketSend(connection.Guid, info.Priority, info.Reliability, (byte)info.Channel);
                }
            }

            if (info.Connection != null)
            {
                _peer.PacketStart();
                _peer.PacketWrite(_stream);
                _peer.PacketSend(info.Connection.Guid, info.Priority, info.Reliability, (byte)info.Channel);
            }
        }

        public void PacketId(byte packetId)
        {
            // TODO: Create range.
            if (packetId < (byte)RakPacketType.NUMBER_OF_TYPES)
                throw new Exception($"[RakWrap] TODO: Ты не можешь использовать id меньше чем {(byte)RakPacketType.NUMBER_OF_TYPES}");

            UInt8(packetId);
        }

        public void UInt8(byte value)
        {
            WriteBlock8(new Block8 { Unsigned = value });
        }

        public void UInt16(ushort value)
        {
            WriteBlock16(new Block16 { Unsigned = value });
        }

        public void UInt32(uint value)
        {
            WriteBlock32(new Block32 { Unsigned = value });
        }

        public void UInt64(ulong value)
        {
            WriteBlock64(new Block64 { Unsigned = value });
        }

        public void Int8(sbyte value)
        {
            WriteBlock8(new Block8 { Signed = value });
        }

        public void Int16(short value)
        {
            WriteBlock16(new Block16 { Signed = value });
        }

        public void Int32(int value)
        {
            WriteBlock32(new Block32 { Signed = value });
        }

        public void Int64(long value)
        {
            WriteBlock64(new Block64 { Signed = value });
        }

        public void Bool(bool value)
        {
            _stream.WriteByte((byte)(value ? 1 : 0));
        }

        public void Float(float value)
        {
            WriteBlock32(new Block32 { Float = value });
        }

        public void Double(double value)
        {
            WriteBlock64(new Block64 { Float = value });
        }

        public void Bytes(byte[] buffer, int offset, int length)
        {
            _stream.Write(buffer, offset, length);
        }

        public void Bytes(byte[] buffer, int length)
        {
            if (buffer == null || buffer.Length == 0 || length == 0)
            {
                UInt32(0);
            }
            else if (length > _network.MaxPacketSize)
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
            else if (buffer.Length > _network.MaxPacketSize)
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

            if (_stream.Length < position + offset)
                _stream.SetLength(position + offset);

            _stream.Position = position + offset;

            return position;
        }

        private void WriteBlock8(Block8 block)
        {
            var offset = CreateOffset(1);
            var buffer = GetBuffer();

            buffer[offset] = block.Byte1;
        }

        private void WriteBlock16(Block16 block)
        {
            var offset = CreateOffset(2);
            var buffer = GetBuffer();

            buffer[offset] = block.Byte1;
            buffer[offset + 1] = block.Byte2;
        }

        private void WriteBlock32(Block32 block)
        {
            var offset = CreateOffset(4);
            var buffer = GetBuffer();

            buffer[offset] = block.Byte1;
            buffer[offset + 1] = block.Byte2;
            buffer[offset + 2] = block.Byte3;
            buffer[offset + 3] = block.Byte4;
        }

        private void WriteBlock64(Block64 block)
        {
            var offset = CreateOffset(8);
            var buffer = GetBuffer();

            buffer[offset] = block.Byte1;
            buffer[offset + 1] = block.Byte2;
            buffer[offset + 2] = block.Byte3;
            buffer[offset + 3] = block.Byte4;
            buffer[offset + 4] = block.Byte5;
            buffer[offset + 5] = block.Byte6;
            buffer[offset + 6] = block.Byte7;
            buffer[offset + 7] = block.Byte8;
        }

        #endregion
    }
}
