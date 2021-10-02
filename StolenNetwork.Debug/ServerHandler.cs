using System;

namespace StolenNetwork.Debug
{
	public class ServerHandler : Server<Connection>.IHandler
	{
		#region Public Vars

		public string ServerName;

		#endregion

        #region Public Methods

		public void PacketProcess(Packet<Connection> packet)
		{
			if (packet.Type == (byte) PacketType.Ping)
			{
				var writer = packet.Writer;
				var reader = packet.Reader;

				Console.WriteLine("SERVER: " + reader.String());

				if (!writer.Start((byte)PacketType.Ping))
					throw new Exception();

				writer.String("SERVER PONG");
				writer.Send(new PacketInfo(packet.Connection));
			}
		}

		public void ClientConnecting(Connection connection, PacketWriter writer)
		{
			writer.String(ServerName);

			Console.WriteLine($"[SERVER] Client Connecting: {connection.Address}:{connection.Port}");
        }

		public void ClientConnected(Connection connection, PacketReader reader)
		{
			var userName = reader.String();

			Console.WriteLine($"[SERVER] Client Connected: address: {connection.Address}:{connection.Port}, name: {userName}");
        }

		public void ClientDisconnected(Connection connection, string reason)
		{
			Console.WriteLine($"[SERVER] Client Disconnected: {connection.Address}.");
        }

		public void SendedPacketAcked(uint packetId)
		{
			Console.WriteLine($"[SERVER] Packet Acked: {packetId}");
		}

		public void SendedPacketLoss(uint packetId)
		{
			Console.WriteLine($"[SERVER] Packet Loss: {packetId}");
        }

		#endregion
	}
}
