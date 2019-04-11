using System;

namespace StolenNetwork.RakNet
{
	public class RakPacketReader : PacketReader
	{
		#region Public Vars

		public RakPacketReader(Network network) : base(network)
		{ }

		public override bool Start()
		{
			throw new NotImplementedException();
		}

		public override byte PacketId()
		{
			throw new NotImplementedException();
		}

		public override byte UInt8()
		{
			throw new NotImplementedException();
		}

		public override ushort UInt16()
		{
			throw new NotImplementedException();
		}

		public override uint UInt32()
		{
			throw new NotImplementedException();
		}

		public override ulong UInt64()
		{
			throw new NotImplementedException();
		}

		public override sbyte Int8()
		{
			throw new NotImplementedException();
		}

		public override short Int16()
		{
			throw new NotImplementedException();
		}

		public override int Int32()
		{
			throw new NotImplementedException();
		}

		public override long Int64()
		{
			throw new NotImplementedException();
		}

		public override bool Bool()
		{
			throw new NotImplementedException();
		}

		public override float Float()
		{
			throw new NotImplementedException();
		}

		public override double Double()
		{
			throw new NotImplementedException();
		}

		public override int Bytes(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
