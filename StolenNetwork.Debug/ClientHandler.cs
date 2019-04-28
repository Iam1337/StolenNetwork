using System;

namespace StolenNetwork.Debug
{
	public class ClientHandler : Client.IHandler
	{
		#region Public Vars

		public string Username;

		#endregion

        #region Public Methods

        public void Tick()
	    {

	    }

		public void PacketProcess(Packet packet)
		{
			
		}

		public void ClientConnecting()
		{
			Console.WriteLine($"[CLIENT] Connected!");
        }

		public void ClientConnected(PacketReader reader, PacketWriter writer)
		{
			// GET SERVER DATA
			var serverName = reader.String();

			// SET USER DATA 
			writer.String(Username);

			Console.WriteLine($"[CLIENT] ClientConnected to {serverName}.");
        }

		public void ClientDisconnected(DisconnectType disconnectType, string reason)
		{
			Console.WriteLine($"[CLIENT] Disconnected: {disconnectType}:{reason}.");
        }

		#endregion
	}
}
