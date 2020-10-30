/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System.Runtime.InteropServices;

namespace StolenNetwork.Converters
{
    [StructLayout(LayoutKind.Explicit)]
	internal struct Block8
    {
        #region Static Public Static

        public const int Size = 1;

        #endregion

        #region Public Vars

        [FieldOffset(0)]
        public sbyte Signed;

        [FieldOffset(0)]
        public byte Unsigned;

        [FieldOffset(0)]
        public byte Byte1;

        #endregion
    }
}
