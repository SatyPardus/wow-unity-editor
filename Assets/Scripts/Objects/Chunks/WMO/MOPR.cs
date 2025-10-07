using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOPR : IChunk
{
    public class SMOPortalRef : IChunk
    {
        public ushort PortalIndex { get; private set; }
        public ushort GroupIndex { get; private set; }
        public short Side { get; private set; }
        public ushort Filler { get; private set; }

        public SMOPortalRef() { }
        public SMOPortalRef(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            PortalIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0, 2));
            GroupIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x2, 2));
            Side = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(0x4, 2));
            Filler = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x6, 2));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOPortalRef[] PortalRefs { get; private set; }

    public MOPR() { }
    public MOPR(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        PortalRefs = new SMOPortalRef[data.Length / 8];
        for (int i = 0; i < PortalRefs.Length; i++)
        {
            PortalRefs[i] = new SMOPortalRef(data.Slice(i * 8, 8));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
