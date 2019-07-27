using System;
using System.Threading;

namespace StolenNetwork.Debug
{
    public enum PacketType
    {
        Ping = StolenPacketType.NUMBER_OF_TYPES,
    }

	public class Program
	{
		private static Server _server;

		private static Client _client;

		static void Main(string[] args)
		{
			// SERVER
			var serverHandler = new ServerHandler();
			serverHandler.ServerName = "BIP Server";

			_server = new Server(serverHandler);
			_server.Start("127.0.0.1", 7000, 20);

			// CLIENT
			var clientHandler = new ClientHandler();
			clientHandler.Username = "BUP Client";

			_client = new Client(clientHandler);
			_client.Connect("127.0.0.1", 7000);

			while (true)
			{
				_client.Tick();
				_server.Tick();

				//if (_client.Connection.State == ConnectionState.Connected)
				//{
				//	var writer = _client.Writer;

				//	writer.Start((byte) PacketType.Ping);
				//	writer.String("CLIENT PING");
				//	var id = writer.Send(new PacketInfo(_client.Connection, PacketReliability.ReliableAck));

				//	Console.WriteLine("CLIENT ID: " + id);
				//}

				if (_server.IsStarted() && _server.Connections.Count > 0)
				{
					var writer = _server.Writer;

					writer.Start((byte) PacketType.Ping);
					writer.String("CLIENT PING");
					var id = writer.Send(new PacketInfo(_server.Connections[0], PacketReliability.ReliableAck));

					Console.WriteLine("SERVER ID: " + id);
				}

				Thread.Sleep(500);
			}
		}
	}
}
