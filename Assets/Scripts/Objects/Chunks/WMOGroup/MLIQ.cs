using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MLIQ : IChunk
{
    public class SMOWVert : IChunk
    {
        public uint Data { get; private set; }
        public float Height { get; private set; }

        public SMOWVert() { }
        public SMOWVert(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Data = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
            Height = BitConverter.ToSingle(data.Slice(4, 4));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public Vector2Int LiquidVerts { get; private set; }
    public Vector2Int LiquidTiles { get; private set; }
    public Vector3 LiquidCorner { get; private set; }
    public ushort LiquidMaterialID { get; private set; }

    public SMOWVert[] Vertices { get; private set; }
    public byte[] Tiles { get; private set; }

    public MLIQ() { }
    public MLIQ(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var offset = 0;

        LiquidVerts = Utils.ReadVector2Int(data.Slice(offset, 8)); offset += 8;
        LiquidTiles = Utils.ReadVector2Int(data.Slice(offset, 8)); offset += 8;
        LiquidCorner = Utils.ReadVector3(data.Slice(offset, 12)); offset += 12;
        LiquidMaterialID = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;

        Vertices = new SMOWVert[LiquidVerts.x * LiquidVerts.y];
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = new SMOWVert(data.Slice(offset, 8)); offset += 8;
        }

        Tiles = new byte[LiquidTiles.x * LiquidTiles.y];
        for (int i = 0; i < Tiles.Length; i++)
        {
            Tiles[i] = data[offset++];
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
