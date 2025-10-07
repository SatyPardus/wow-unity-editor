using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class MOMT : IChunk
{
    public class SMOMaterial : IChunk
    {
        public uint Flags { get; private set; }
        public uint Shader { get; private set; }
        public uint BlendMode { get; private set; }
        public uint Texture1 { get; private set; }
        public Color SidnColor { get; private set; }
        public Color FrameSidnColor { get; private set; }
        public uint Texture2 { get; private set; }
        public Color DiffColor { get; private set; }
        public uint GroundType { get; private set; }
        public uint Texture3 { get; private set; }
        public Color Color2 { get; private set; }
        public uint Flags2 { get; private set; }

        public SMOMaterial() { }
        public SMOMaterial(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            int offset = 0;

            Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Shader = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            BlendMode = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Texture1 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            SidnColor = Utils.ReadCImVector(data.Slice(offset, 4)); offset += 4;
            FrameSidnColor = Utils.ReadCImVector(data.Slice(offset, 4)); offset += 4;
            Texture2 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            DiffColor = Utils.ReadCImVector(data.Slice(offset, 4)); offset += 4;
            GroundType = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Texture3 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Color2 = Utils.ReadCImVector(data.Slice(offset, 4)); offset += 4;
            Flags2 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOMaterial[] Materials { get; private set; }

    public MOMT() { }
    public MOMT(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Materials = new SMOMaterial[data.Length / 64];

        for (int i = 0; i < Materials.Length; i++)
        {
            Materials[i] = new SMOMaterial(data.Slice(i * 64, 64));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
