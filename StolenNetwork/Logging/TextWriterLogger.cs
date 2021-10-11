/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;
using System.IO;
using System.Globalization;

namespace StolenNetwork.Logging
{
    public class TextWriterLogger : ILogger
    {
	    #region Private Vars

        private const string kDateTime_Format = "MMdd HH:mm:ss.ffffff";

        private readonly TextWriter _textWriter;

        #endregion

        #region Public Methods

        public TextWriterLogger(TextWriter textWriter)
        {
	        _textWriter = textWriter ?? throw new NullReferenceException(nameof(textWriter));
        }

        #endregion

        #region Interface Methods
        
        void ILogger.Debug(string message) => Write("D", message);

        void ILogger.Info(string message) => Write("I", message);

        void ILogger.Warning(string message) => Write("W", message);

        #endregion

        #region Private Methods

        private void Write(string prefix, string message)
        {
            _textWriter.WriteLine($"{prefix}{DateTime.Now.ToString(kDateTime_Format, CultureInfo.InvariantCulture)} {message}");
        }

        #endregion
    }
}
