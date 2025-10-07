using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LightTransport;

public class MCNK : IChunk
{
    public uint IndexX { get; private set; }
    public uint IndexY { get; private set; }
    public Vector3 Position { get; private set; }

    public uint Flags { get; private set; }
    public uint LayerCount { get; private set; }
    public uint DoodadRefCount { get; private set; }
    public uint MapObjRefCount { get; private set; }
    public uint AreaID { get; private set; }
    public ushort LowResHoles { get; private set; }

    public MCVT MCVT { get; private set; }
    public MCNR MCNR { get; private set; }

    public MCNK() { }
    public MCNK(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        int offset = 8;

        Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        IndexX = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        IndexY = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        LayerCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        DoodadRefCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;

        var ofsHeight = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsNormal = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsLayer = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsRefs = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsAlpha = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var sizeAlpha = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsShadow = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var sizeShadow = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;

        AreaID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        MapObjRefCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        LowResHoles = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        offset += 2; // unknown_but_used
        offset += 0x10; // uint2_t[8][8] ReallyLowQualityTextureingMap;
        offset += 0x8; // uint1_t[8][8] noEffectDoodad;

        var ofsSndEmitters = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var sizeSndEmitters = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var ofsLiquid = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        var sizeLiquid = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        Position = Utils.ReadVector3(data.Slice(offset, 12)); offset += 12;
        var ofsMCCV = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        offset += 8; // unknown_but_used

        //Debug.Log($"{IndexX}_{IndexY} -> {Position}");
        //int count = 0;
        //while (offset + 8 < data.Length)
        //{
        //    count++;
        //    if (count > 100) return;

        //    var chunk = new IffChunk(data.Slice((int)offset, 8), (uint)offset);
        //    offset += 8;

        //    switch (chunk.Token)
        //    {
        //        default: Debug.LogError($"Unhandled chunk: {chunk.Token} ({chunk.Size} bytes)"); break;
        //    }
        //    offset += (int)chunk.Size;
        //}

        if (ofsHeight > 0)
        {
            if (!Utils.ValidateChunk(data.Slice(ofsHeight, 8), 0, "MCVT")) throw new Exception($"Invalid chunk (MCVT {offset} {ofsHeight} {String.Join(", ", data.Slice(offset).ToArray().Select(b => $"0x{b.ToString("X2")}"))})");
            MCVT = new MCVT(data.Slice(ofsHeight + 8, BinaryPrimitives.ReadInt32LittleEndian(data.Slice(ofsHeight + 4, 4))));
        }
        if (ofsNormal > 0)
        {
            if (!Utils.ValidateChunk(data.Slice(ofsNormal, 8), 0, "MCNR")) throw new Exception($"Invalid chunk (MCNR)");
            MCNR = new MCNR(data.Slice(ofsNormal + 8, BinaryPrimitives.ReadInt32LittleEndian(data.Slice(ofsNormal + 4, 4))));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
