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
	        _server = new Server();
	        _server.CallbackHandler = new ServerHandler();
            _server.Start("127.0.0.1", 7000, 20);
			

            _client = new Client();
			_client.CallbackHandler = new ClientHandler();
	        _client.Connect("127.0.0.1", 7000);

	        while (true)
	        {
				_client.Tick();
				_server.Tick();

		        if (_client.Connection.IsConnected)
		        {
			        var writer = _client.Writer;
			        writer.Start();
			        writer.PacketId((byte) PacketType.Ping);
			        writer.String("PING");
			        writer.Send(new PacketInfo(_client.Connection));
		        }

		        Thread.Sleep(1000);
	        }
			
        }
    }
}
