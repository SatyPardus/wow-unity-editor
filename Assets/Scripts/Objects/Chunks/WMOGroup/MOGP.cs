using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOGP : IChunk
{
    public uint GroupName { get; private set; }
    public uint DescriptiveGroupName { get; private set; }
    public uint Flags { get; private set; }
    public Bounds BoundingBox { get; private set; }
    public ushort PortalStart { get; private set; }
    public ushort PortalCount { get; private set; }
    public ushort TransBatchCount { get; private set; }
    public ushort IntBatchCount { get; private set; }
    public ushort ExtBatchCount { get; private set; }
    public byte[] FogIDs { get; private set; }
    public uint GroupLiquid { get; private set; }
    public uint UniqueID { get; private set; }
    public uint Flags2 { get; private set; }

    public MOGP() { }
    public MOGP(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        int offset = 0;

        GroupName = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        DescriptiveGroupName = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        BoundingBox = Utils.ReadCAaBox(data.Slice(offset, 24)); offset += 24;
        PortalStart = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        PortalCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        TransBatchCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        IntBatchCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        ExtBatchCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        offset += 2; // padding_or_batch_type_d
        FogIDs = new byte[4]
        {
            data[offset++],
            data[offset++],
            data[offset++],
            data[offset++]
        };
        GroupLiquid = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        UniqueID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        Flags2 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        offset += 4; // ukn
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
