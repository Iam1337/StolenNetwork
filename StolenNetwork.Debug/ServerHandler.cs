using System;

namespace StolenNetwork.Debug
{
	public class ServerHandler : Server.IHandler
	{
		#region Public Vars

		public void PacketProcess(Packet packet)
		{
			if (packet.Type == 4)
			{
				Console.WriteLine(packet.Reader.String());
			}
		}

		public void ClientConnected(Connection connection)
		{
			Console.WriteLine($"[SERVER] Client Connected: {connection.Address}.");
		}

		public void ClientDisconnected(Connection connection, string reason)
		{
			Console.WriteLine($"[SERVER] Client Disconnected: {connection.Address}.");
        }

		#endregion
	}
}
