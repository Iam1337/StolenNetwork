using System;

namespace StolenNetwork.RakNet
{
	public class RakPacketWriter : PacketWriter
	{
		#region Public Methods

		public RakPacketWriter(Network network) : base(network)
		{ }

		public override bool Start()
		{
			throw new NotImplementedException();
		}

		public override void Send(PacketInfo info)
		{
			throw new NotImplementedException();
		}

		public override void PacketId(byte packetType)
		{
			throw new NotImplementedException();
		}

		public override void UInt8(byte value)
		{
			throw new NotImplementedException();
		}

		public override void UInt16(ushort value)
		{
			throw new NotImplementedException();
		}

		public override void UInt32(uint value)
		{
			throw new NotImplementedException();
		}

		public override void UInt64(ulong value)
		{
			throw new NotImplementedException();
		}

		public override void Int8(sbyte value)
		{
			throw new NotImplementedException();
		}

		public override void Int16(short value)
		{
			throw new NotImplementedException();
		}

		public override void Int32(int value)
		{
			throw new NotImplementedException();
		}

		public override void Int64(long value)
		{
			throw new NotImplementedException();
		}

		public override void Bool(bool value)
		{
			throw new NotImplementedException();
		}

		public override void Float(float value)
		{
			throw new NotImplementedException();
		}

		public override void Double(double value)
		{
			throw new NotImplementedException();
		}

		public override void Bytes(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
