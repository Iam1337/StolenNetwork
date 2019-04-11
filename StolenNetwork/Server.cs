using System;
using System.Collections.Generic;
using System.Text;

namespace StolenNetwork
{
    public abstract class Server : Network
    {
        #region Extensions

        public interface IHandler
        {
            void PacketProcess(Packet packet);

            void ClientConnected(Connection connection);

            void ClientDisconnected(Connection connection, string reason);
        }

        #endregion

        #region Public Vars

        public string Host;

        public ushort Port;

        public ushort ConnectionsCount;

        public IHandler CallbackHandler;

        #endregion

        #region Private Vars

        private ushort _previousConnectionId;

        private List<Connection> _connections = new List<Connection>();

        private Dictionary<ulong, Connection> _connectionsGuids = new Dictionary<ulong, Connection>();

        #endregion

        #region Public Methods

        // SETUP
        public virtual bool Start(string host, ushort port, ushort connectionsCount)
        {
            Host = host;
            Port = port;
            ConnectionsCount = connectionsCount;

            return false;
        }

        public abstract bool IsStarted();

        public abstract void Stop(string reason);

        // LOOP
        public abstract void Tick();

        // METHODS
        public Connection GetConnection(ulong guid)
        {
            _connectionsGuids.TryGetValue(guid, out var connection);

            return connection;
        }
        
        public abstract void Kick(Connection connection, string reason);

        #endregion

        #region Protected Methods

        // CONNECTIONS
        protected abstract void CreateConnection();

        protected void ConnectionDisconnect(Connection connection, string reason)
        {
            if (connection == null)
                return;

            connection.Connected = false;
            connection.State = ConnectionState.Disconnected;

            if (CallbackHandler != null)
                CallbackHandler.ClientDisconnected(connection, reason);

            RemoveConnection(connection);
        }

        protected void AddConnection(Connection connection)
        {
            if (connection == null)
                return;

            connection.ConnectionTime = DateTime.Now;

            _connections.Add(connection);
            _connectionsGuids.Add(connection.Guid, connection);

            if (!Writer.Start())
                return;

            Writer.PacketId((byte) StolenPacketType.Handshake);
            //TODO: Write server info.
            Writer.Send(new PacketInfo(connection));

            if (CallbackHandler != null)
                CallbackHandler.ClientConnected(connection);
        }

        protected void RemoveConnection(Connection connection)
        {
            _connectionsGuids.Remove(connection.Guid);
            _connections.Remove(connection);

            connection.Clean();
        }

        #endregion
    }
}
