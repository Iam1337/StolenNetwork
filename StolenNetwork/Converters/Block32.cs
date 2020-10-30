/* Copyright (c) 2020 ExT (V.Sigalkin) */

using System.Runtime.InteropServices;

namespace StolenNetwork.Converters
{
    [StructLayout(LayoutKind.Explicit)]
	internal struct Block32
    {
        #region Static Public Static

        public const int Size = 4;

        #endregion

        #region Public Vars

        [FieldOffset(0)]
        public int Signed;

        [FieldOffset(0)]
        public uint Unsigned;

        [FieldOffset(0)]
        public float Float;

        [FieldOffset(0)]
        public byte Byte1;

        [FieldOffset(1)]
        public byte Byte2;

        [FieldOffset(2)]
        public byte Byte3;

        [FieldOffset(3)]
        public byte Byte4;

        #endregion
    }
}
