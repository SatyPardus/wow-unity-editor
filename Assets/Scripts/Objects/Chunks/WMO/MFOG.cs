using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EFog
{
    FOG,
    UNDERWATER_FOG
}

public class MFOG : IChunk
{
    public class SMOFog : IChunk
    {
        public class Fog : IChunk
        {
            public float End { get; private set; }
            public float StartScalar { get; private set; }
            public Color Color { get; private set; }

            public Fog() { }
            public Fog(Span<byte> data) => Read(data);

            public void Read(Span<byte> data)
            {
                End = BitConverter.ToSingle(data.Slice(0, 4));
                StartScalar = BitConverter.ToSingle(data.Slice(4, 4));
                Color = Utils.ReadCImVector(data.Slice(8, 4));
            }

            public void Write(Span<byte> data)
            {
                throw new NotImplementedException();
            }
        }

        public uint Flags { get; private set; }
        public Vector3 Position { get; private set; }
        public float SmallerRadius { get; private set; }
        public float LargerRadius { get; private set; }
        public Fog[] Fogs { get; private set; }

        public SMOFog() { }
        public SMOFog(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Fogs = new Fog[2];

            Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
            Position = Utils.ReadVector3(data.Slice(4, 12));
            SmallerRadius = BitConverter.ToSingle(data.Slice(16, 4));
            SmallerRadius = BitConverter.ToSingle(data.Slice(20, 4));
            for (int i = 0; i < Fogs.Length; i++)
            {
                Fogs[i] = new Fog(data.Slice(24 + i * 12, 12));
            }
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOFog[] Fogs { get; private set; }

    public MFOG() { }
    public MFOG(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Fogs = new SMOFog[data.Length / 48];
        for (int i = 0; i < Fogs.Length; i++)
        {
            Fogs[i] = new SMOFog(data.Slice(i * 48, 48));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
