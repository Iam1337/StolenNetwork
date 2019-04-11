namespace StolenNetwork
{
    public abstract class Client : Network
    {
        #region Extensions

	    public interface IHandler
        {
		    #region Methods

		    void PacketProcess(Packet packet);

	        void ClientConnected();

            void ClientDisconnected(DisconnectType disconnectType, string reason);
			
		    #endregion
	    }

        #endregion

        #region Public Vars

        public string Host { get; protected set; }

	    public ushort Port { get; protected set; }

	    public Connection Connection { get; protected set; }

	    public IHandler CallbackHandler;

	    public static string DisconnectReason;

	    #endregion

	    #region Protected Methods

	    protected bool connectionAccepted;

	    #endregion

	    #region Public Methods

	    // CONECTING/DISCONECTING
	    public virtual bool Connect(string host, ushort port)
	    {
		    Host = host;
		    Port = port;

		    connectionAccepted = false;

		    DisconnectReason = "Disconnected";

		    return false;
	    }

	    public virtual void Disconnect(string reason, bool sendReason = true)
	    {
			Disconnect(DisconnectType.CustomReason, reason, sendReason);
	    }

	    public abstract bool IsConnected();

	    // GAME LOOP
	    public abstract void Tick();

	    // PING
	    public abstract int GetAveragePing();

	    public abstract int GetLastPing();

	    public abstract int GetLowestPing();

	    #endregion

	    #region Protected Methods

	    protected abstract void Disconnect(DisconnectType disconnectType, string reason, bool sendReason = true);

	    protected void DisconnectedInternal(DisconnectType disconnectType, string reason)
	    {
		    if (CallbackHandler != null)
			    CallbackHandler.ClientDisconnected(disconnectType, reason);
	    }

	    #endregion
    }
}
