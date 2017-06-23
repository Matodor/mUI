using System.IO;

namespace mFramework.UI
{
    public class TrueTypeFontReader
    {
        private readonly BinaryReader _fontStream;

        public TrueTypeFontReader(BinaryReader reader)
        {
            _fontStream = reader;
            _fontStream.BaseStream.Seek(0, SeekOrigin.Begin);

            ReadOffsetTables();
        }

        private ushort _scalarType;

        private void ReadOffsetTables()
        {
            _scalarType = _fontStream.ReadUInt16();
        }
    }
}
