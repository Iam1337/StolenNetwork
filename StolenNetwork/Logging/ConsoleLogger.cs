/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System;

namespace StolenNetwork.Logging
{
    public class ConsoleLogger : TextWriterLogger
    {
		public ConsoleLogger() : base(Console.Out)
		{

		}
    }
}
