using System;

namespace StolenNetwork.Debug
{
	public class ClientHandler : Client.IHandler
	{
		#region Public Methods

		public void PacketProcess(Packet packet)
		{
			
		}

		public void ClientConnected()
		{
			Console.WriteLine($"Client Connected!");
        }

		public void ClientDisconnected(DisconnectType disconnectType, string reason)
		{
			Console.WriteLine($"Client Disconnected: {disconnectType}:{reason}.");
        }

		#endregion
	}
}
