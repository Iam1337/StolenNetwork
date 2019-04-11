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
				Thread.Sleep(1000);
	        }
			
        }
    }
}
