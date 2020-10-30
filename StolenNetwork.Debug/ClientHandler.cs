using System;

namespace StolenNetwork.Debug
{
	public class ClientHandler : Client<Connection>.IHandler
	{
		#region Public Vars

		public string Username;

		#endregion

        #region Public Methods

		public void PacketProcess(Packet<Connection> packet)
		{
			if (packet.Type == (byte)PacketType.Ping)
			{
				var reader = packet.Reader;

				Console.WriteLine("CLIENT: " + reader.String());
			}
        }

		public void ClientConnecting()
		{
			Console.WriteLine($"[CLIENT] Connecting!");
        }

		public void ClientConnected(PacketReader reader, PacketWriter writer)
		{
			// GET SERVER DATA
			var serverName = reader.String();

			// SET USER DATA 
			writer.String(Username);

			Console.WriteLine($"[CLIENT] Connected: name: {serverName}.");
        }

		public void ClientDisconnected(DisconnectType disconnectType, string reason)
		{
			Console.WriteLine($"[CLIENT] Disconnected: {disconnectType}:{reason}.");
        }

		public void SendedPacketAcked(uint packetId)
		{
			Console.WriteLine($"[CLIENT] Packet Acked: {packetId}");
        }

		public void SendedPacketLoss(uint packetId)
		{
			Console.WriteLine($"[CLIENT] Packet Loss: {packetId}");
        }

		#endregion
	}
}
