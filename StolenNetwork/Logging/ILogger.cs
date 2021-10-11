/* Copyright (c) 2021 ExT (V.Sigalkin) */

namespace StolenNetwork.Logging
{
    public interface ILogger
    {
	    void Debug(string message);

	    void Info(string message);

	    void Warning(string message);
    }
}
