using System;

namespace StolenNetwork.Debug
{
	public class ServerHandler : Server.IHandler
	{
		#region Public Vars

		public void PacketProcess(Packet packet)
		{
			
		}

		public void ClientConnected(Connection connection)
		{
			Console.WriteLine($"Client Connected: {connection.Address}.");
		}

		public void ClientDisconnected(Connection connection, string reason)
		{
			Console.WriteLine($"Client Disconnected: {connection.Address}.");
        }

		#endregion
	}
}
