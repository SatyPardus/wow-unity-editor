using System;
using System.Buffers.Binary;
using System.Text;

public class MPQHeader
{
    public string Magic { get; private set; }
    public uint HeaderSize { get; private set; }
    public uint ArchiveSize { get; private set; }
    public ushort FormatVersion { get; private set; }
    public ushort BlockSize { get; private set; }
    public long HashTablePos { get; private set; }
    public long BlockTablePos { get; private set; }
    public uint HashTableSize { get; private set; }
    public uint BlockTableSize { get; private set; }

    // Extended
    public long ExtendedBlockTablePos { get; private set; }

    public MPQHeader(ReadOnlySpan<byte> data)
    {
        Magic = Encoding.ASCII.GetString(data.Slice(0, 4));
        HeaderSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x4, 4));
        ArchiveSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x08, 4));
        FormatVersion = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0C, 2));
        BlockSize = (ushort)(512 * (1 << BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0E, 2))));
        HashTablePos = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x10, 4));
        BlockTablePos = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x14, 4));
        HashTableSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x18, 4));
        BlockTableSize = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x1C, 4));

        if(FormatVersion >= 1)
        {
            ExtendedBlockTablePos = BinaryPrimitives.ReadInt64LittleEndian(data.Slice(0x20, 8));
            var hashTablePosHigh = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x28, 2));
            var blockTablePosHigh = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x2A, 2));

            HashTablePos |= ((long)hashTablePosHigh) << 32;
            BlockTablePos |= ((long)blockTablePosHigh) << 32;
        }
    }
}
