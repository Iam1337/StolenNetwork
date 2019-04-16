using System;

namespace StolenNetwork.Debug
{
	public class ClientHandler : Client.IHandler
	{
		#region Public Methods

        public void Tick()
	    {

	    }

		public void PacketProcess(Packet packet)
		{
			
		}

		public void ClientConnected()
		{
			Console.WriteLine($"[CLIENT] Connected!");
        }

		public void ClientDisconnected(DisconnectType disconnectType, string reason)
		{
			Console.WriteLine($"[CLIENT] Disconnected: {disconnectType}:{reason}.");
        }

		#endregion
	}
}
