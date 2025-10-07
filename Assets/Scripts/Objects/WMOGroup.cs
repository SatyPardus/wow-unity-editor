using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class WMOGroup
{
    public int GroupName { get; private set; }
    public int DescriptiveGroupName { get; private set; }
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

    public MVER MVER { get; private set; }
    public MOPY MOPY { get; private set; }
    public MOVI MOVI { get; private set; }
    public MOVT MOVT { get; private set; }
    public MONR MONR { get; private set; }
    public MOTV MOTV { get; private set; }
    public MOBA MOBA { get; private set; }
    public MOBN MOBN { get; private set; }
    public MOBR MOBR { get; private set; }
    public MOCV MOCV { get; private set; }
    public MODR MODR { get; private set; }
    public MOLR MOLR { get; private set; }
    public MLIQ MLIQ { get; private set; }

    public WMOGroup(MPQFile file)
    {
        Span<byte> data = new Span<byte>(file.Data);

        int offset = 0;
        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice((int)offset, 8), (uint)offset);
            offset += 8;

            try
            {
                switch (chunk.Token)
                {
                    case "MVER": MVER = new MVER(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOGP": ReadHeader(data.Slice(offset, (int)chunk.Size)); break;
                    default: throw new Exception($"[WMOGroup ROOT] Unhandled chunk: {chunk.Token} ({chunk.Size} bytes)"); break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"[WMOGroup] Broken chunk detected! {chunk.Token} (Offset {offset} + {chunk.Size} bytes) in {file.FilePath} ({data.Length} bytes)\n{ex}");
            }
            offset += (int)chunk.Size;
        }

        //Utils.PrintProperties(this);
    }

    private void ReadHeader(Span<byte> data)
    {
        int offset = 0;

        GroupName = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        DescriptiveGroupName = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;

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

        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice((int)offset, 8), (uint)offset);
            offset += 8;

            try
            {
                switch (chunk.Token)
                {
                    case "MOPY": MOPY = new MOPY(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOVI": MOVI = new MOVI(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOVT": MOVT = new MOVT(data.Slice(offset, (int)chunk.Size)); break;
                    case "MONR": MONR = new MONR(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOTV": MOTV = new MOTV(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOBA": MOBA = new MOBA(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOBN": MOBN = new MOBN(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOBR": MOBR = new MOBR(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOCV": MOCV = new MOCV(data.Slice(offset, (int)chunk.Size)); break;
                    case "MODR": MODR = new MODR(data.Slice(offset, (int)chunk.Size)); break;
                    case "MOLR": MOLR = new MOLR(data.Slice(offset, (int)chunk.Size)); break;
                    case "MLIQ": MLIQ = new MLIQ(data.Slice(offset, (int)chunk.Size)); break;
                    default: throw new Exception($"[WMOGroup] Unhandled chunk: {chunk.Token} ({chunk.Size} bytes)");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"[WMOGroup] Broken chunk detected! {chunk.Token} (Offset {offset} + {chunk.Size} bytes) in MOPG ({data.Length} bytes)\n{ex}");
            }
            offset += (int)chunk.Size;
        }

        if (offset < data.Length)
        {
            Debug.LogError($"Unhandled bytes: {data.Length - offset}");
        }
    }
}
