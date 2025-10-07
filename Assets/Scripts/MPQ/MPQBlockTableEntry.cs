using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MPQBlockTableEntry
{
    public long FilePos { get; private set; }
    public uint CompressedSize { get; private set; }
    public uint UncompressedSize { get; private set; }
    public MPQFlags Flags { get; private set; }

    public MPQBlockTableEntry(Span<byte> data, ushort offset)
    {
        FilePos = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0, 4)) | ((long)offset << 32);
        CompressedSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x04, 4));
        UncompressedSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x08, 4));
        Flags = (MPQFlags)BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0C, 4));
    }
}
