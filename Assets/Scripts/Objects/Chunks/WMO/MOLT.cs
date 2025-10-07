using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class MOLT : IChunk
{
    public class SMOLight : IChunk
    {
        public byte Type { get; private set; }
        public byte UseAtten { get; private set; }
        public Color Color { get; private set; }
        public Vector3 Position { get; private set; }
        public float Intensity { get; private set; }
        public Quaternion Rotation { get; private set; }
        public float AttenStart { get; private set; }
        public float AttenEnd { get; private set; }


        public SMOLight() { }
        public SMOLight(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Type = data[0];
            UseAtten = data[1];
            Color = Utils.ReadCImVector(data.Slice(4, 4));
            Position = Utils.ReadVector3(data.Slice(8, 12));
            Intensity = BitConverter.ToSingle(data.Slice(20, 4));
            Rotation = Utils.ReadQuaternion(data.Slice(24, 16));
            AttenStart = BitConverter.ToSingle(data.Slice(40, 4));
            AttenEnd = BitConverter.ToSingle(data.Slice(44, 4));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOLight[] Lights { get; private set; }

    public MOLT() { }
    public MOLT(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Lights = new SMOLight[data.Length / 48];
        for (int i = 0; i < Lights.Length; i++)
        {
            Lights[i] = new SMOLight(data.Slice(i * 48, 48));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
