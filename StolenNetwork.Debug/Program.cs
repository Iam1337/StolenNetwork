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
			
	        _server = new Server();
	        _server.CallbackHandler = serverHandler;
            _server.Start("127.0.0.1", 7000, 20);

	        var server = new Server();
	        server.CallbackHandler = serverHandler;
	        var d = server.Start("127.0.0.1", 7000, 20);

            // CLIENT
            var clientHandler = new ClientHandler();
	        clientHandler.Username = "BUP Client 1";

            _client = new Client();
			_client.CallbackHandler = clientHandler;
	        _client.Connect("127.0.0.1", 7000);


	        while (true)
	        {
				_client.Tick();
				_server.Tick();

				/*
				if (_client.Connection.State == ConnectionState.Connected)
		        {
			        var writer = _client.Writer;

			        writer.Start((byte)PacketType.Ping);
			        writer.String("CLIENT PING");
			        writer.Send(new PacketInfo(_client.Connection));
		        }
				*/

				Thread.Sleep(10);
	        }
			
        }
    }
}
