using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MODD : IChunk
{
    public class SMODoodadDef : IChunk
    {
        public uint NameIndex { get; private set; }
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public float Scale { get; private set; }
        public Color Color { get; private set; }

        public SMODoodadDef() { }
        public SMODoodadDef(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            NameIndex = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
            Position = Utils.ReadVector3(data.Slice(4, 12));
            Rotation = Utils.ReadQuaternion(data.Slice(16, 16));
            Scale = BitConverter.ToSingle(data.Slice(32, 4));
            Color = Utils.ReadCImVector(data.Slice(36, 4));
            // padding 4
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMODoodadDef[] DoodadDefs { get; private set; }

    public MODD() { }
    public MODD(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        DoodadDefs = new SMODoodadDef[data.Length / 40];
        for (int i = 0; i < DoodadDefs.Length; i++)
        {
            DoodadDefs[i] = new SMODoodadDef(data.Slice(i * 40, 40));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
