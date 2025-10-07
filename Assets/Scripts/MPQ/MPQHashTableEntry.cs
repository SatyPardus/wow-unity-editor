using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MPQHashTableEntry
{
    public uint Name1 { get; private set; }
    public uint Name2 { get; private set; }
    public ushort Locale { get; private set; }
    public ushort Platform { get; private set; }
    public uint BlockIndex { get; private set; }

    public MPQHashTableEntry(Span<byte> data)
    {
        Name1 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0, 4));
        Name2 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x04, 4));
        Locale = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x08, 2));
        Platform = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0A, 2));
        BlockIndex = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0C, 4));
    }
}
