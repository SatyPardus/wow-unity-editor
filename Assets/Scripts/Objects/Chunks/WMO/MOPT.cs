using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOPT : IChunk
{
    public class SMOPortal : IChunk
    {
        public ushort StartVertex { get; private set; }
        public ushort Count { get; private set; }
        public Plane Plane { get; private set; }

        public SMOPortal() { }
        public SMOPortal(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            StartVertex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0, 2));
            Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x2, 2));
            Plane = Utils.ReadC4Plane(data.Slice(0x4, 16));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOPortal[] Portals { get; private set; }

    public MOPT() { }
    public MOPT(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Portals = new SMOPortal[data.Length / 20];
        for (int i = 0; i < Portals.Length; i++)
        {
            Portals[i] = new SMOPortal(data.Slice(i * 20, 20));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
