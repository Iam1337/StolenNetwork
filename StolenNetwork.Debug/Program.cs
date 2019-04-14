using System;
using System.Threading;

using StolenNetwork.RakNet;

namespace StolenNetwork.Debug
{
    public class Program
    {
        private static Server _server;

	    private static Client _client;

        static void Main(string[] args)
        {
	        _server = new RakServer();
	        _server.CallbackHandler = new ServerHandler();
            _server.Start("127.0.0.1", 7000, 20);
			

            _client = new RakClient();
			_client.CallbackHandler = new ClientHandler();
	        _client.Connect("127.0.0.1", 7000);

	        while (true)
	        {
				_client.Tick();
				_server.Tick();

		        if (_client.Connection.Connected)
		        {
			        var writer = _client.Writer;
			        writer.Start();
			        writer.PacketId(4);
			        writer.String("PING");
			        writer.Send(new PacketInfo(_client.Connection));
		        }

		        Thread.Sleep(1000);
	        }
			
        }
    }
}
