using System;

namespace StolenNetwork.Debug
{
	public class ServerHandler : Server.IHandler
	{
		#region Public Vars

		public string ServerName;

		#endregion

        #region Public Methods

        public void Tick()
	    {

	    }

        public void PacketProcess(Packet packet)
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

			Console.WriteLine($"[SERVER] Client Connecting: {connection.Address}.");
        }

		public void ClientConnected(Connection connection, PacketReader reader)
		{
			var userName = reader.String();

			Console.WriteLine($"[SERVER] Client Connected: {connection.Address}:{userName}.");
        }

		public void ClientDisconnected(Connection connection, string reason)
		{
			Console.WriteLine($"[SERVER] Client Disconnected: {connection.Address}.");
        }

		#endregion
	}
}
