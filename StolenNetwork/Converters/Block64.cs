/* Copyright (c) 2021 ExT (V.Sigalkin) */

using System.Runtime.InteropServices;

namespace StolenNetwork.Converters
{
    [StructLayout(LayoutKind.Explicit)]
	internal struct Block64
    {
        #region Static Public Static

        public const int Size = 8;

        #endregion

        #region Public Vars

        [FieldOffset(0)]
        public long Signed;

        [FieldOffset(0)]
        public ulong Unsigned;

        [FieldOffset(0)]
        public double Float;

        [FieldOffset(0)]
        public byte Byte1;

        [FieldOffset(1)]
        public byte Byte2;

        [FieldOffset(2)]
        public byte Byte3;

        [FieldOffset(3)]
        public byte Byte4;

        [FieldOffset(4)]
        public byte Byte5;

        [FieldOffset(5)]
        public byte Byte6;

        [FieldOffset(6)]
        public byte Byte7;

        [FieldOffset(7)]
        public byte Byte8;

        #endregion
    }
}
