using System;

namespace StolenNetwork.Debug
{
    public class Connection : IConnection
    {
		#region Public Vars

		public ConnectionState State { get; set; }

		public bool IsConnected { get; set; }

		public byte Id { get; set; }

		public ulong Guid { get; set; }

		public string Address { get; set; }

		public ushort Port { get; set; }

		public string DisconnectReason { get; set; }

		public DateTime ConnectionTime { get; set; }

		#endregion

		#region Public Methods

		public void Clean()
		{
			Id = 0;
			Guid = 0;
			Address = null;
			State = ConnectionState.Unconnected;
			Port = 0;
		}

		#endregion
	}
}
