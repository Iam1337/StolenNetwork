/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System;

namespace StolenNetwork
{
    public interface IConnection
    {
		#region Vars

		ConnectionState State { get; set; }

		bool IsConnected { get; set; }

		byte Id { get; set; }

		ulong Guid { get; set; }

		string Address { get; set; }

		ushort Port { get; set; }

		string DisconnectReason { get; set; }

		DateTime ConnectionTime { get; set; }

		#endregion
	}
}
