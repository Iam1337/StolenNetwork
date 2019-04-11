using System.Runtime.InteropServices;

namespace StolenNetwork.Converters
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Block16
    {
        #region Static Public Static

        public const int Size = 2;

        #endregion

        #region Public Vars

        [FieldOffset(0)]
        public short Signed;

        [FieldOffset(0)]
        public ushort Unsigned;

        [FieldOffset(0)]
        public byte Byte1;

        [FieldOffset(1)]
        public byte Byte2;

        #endregion
    }
}
