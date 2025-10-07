using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MHDR : IChunk
{
    public uint Flags { get; private set; }
    public uint OffsetMCIN { get; private set; }
    public uint OffsetMTEX { get; private set; }
    public uint OffsetMMDX { get; private set; }
    public uint OffsetMMID { get; private set; }
    public uint OffsetMWMO { get; private set; }
    public uint OffsetMWID { get; private set; }
    public uint OffsetMDDF { get; private set; }
    public uint OffsetMODF { get; private set; }
    public uint OffsetMFBO { get; private set; }
    public uint OffsetMH2O { get; private set; }
    public uint OffsetMTXF { get; private set; }

    public MHDR() { }
    public MHDR(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
        OffsetMCIN = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(4, 4));
        OffsetMTEX = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(8, 4));
        OffsetMMDX = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(12, 4));
        OffsetMMID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(16, 4));
        OffsetMWMO = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(20, 4));
        OffsetMWID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(24, 4));
        OffsetMDDF = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(28, 4));
        OffsetMODF = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(32, 4));
        OffsetMFBO = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(36, 4));
        OffsetMH2O = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(40, 4));
        OffsetMTXF = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(44, 4));
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
