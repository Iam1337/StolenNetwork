using System;
using System.IO;

using StolenNetwork.Converters;

namespace StolenNetwork.RakNet
{
	public class RakPacketWriter : PacketWriter
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

        public RakPacketWriter(RakPeer peer, Network network) : base(network)
		{
			_peer = peer;
			_stream = new MemoryStream();
        }

		public override bool Start()
		{
			if (_peer == null)
				return false;

			_stream.Position = 0;
			_stream.SetLength(0);

			return true;
        }

		public override void Send(PacketInfo info)
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

		public override void PacketId(byte packetType)
		{
			if (packetType > byte.MaxValue - (byte)DefaultPacketType.USER_PACKET_ENUM)
				throw new Exception($"[RakWrap] TODO: Ты не можешь использовать id больше {byte.MaxValue - (byte)DefaultPacketType.USER_PACKET_ENUM}");

			UInt8((byte)(packetType + (byte)DefaultPacketType.USER_PACKET_ENUM));
        }

		public override void UInt8(byte value)
		{
			WriteBlock8(new Block8 { Unsigned = value });
		}

		public override void UInt16(ushort value)
		{
			WriteBlock16(new Block16 { Unsigned = value });
		}

		public override void UInt32(uint value)
		{
			WriteBlock32(new Block32 { Unsigned = value });
		}

		public override void UInt64(ulong value)
		{
			WriteBlock64(new Block64 { Unsigned = value });
		}

		public override void Int8(sbyte value)
		{
			WriteBlock8(new Block8 { Signed = value });
		}

		public override void Int16(short value)
		{
			WriteBlock16(new Block16 { Signed = value });
		}

		public override void Int32(int value)
		{
			WriteBlock32(new Block32 { Signed = value });
		}

		public override void Int64(long value)
		{
			WriteBlock64(new Block64 { Signed = value });
		}

		public override void Bool(bool value)
		{
			_stream.WriteByte((byte)(value ? 1 : 0));
		}

		public override void Float(float value)
		{
			WriteBlock32(new Block32 { Float = value });
		}

		public override void Double(double value)
		{
			WriteBlock64(new Block64 { Float = value });
		}

        public override void Bytes(byte[] buffer, int offset, int length)
		{
			_stream.Write(buffer, offset, length);
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
