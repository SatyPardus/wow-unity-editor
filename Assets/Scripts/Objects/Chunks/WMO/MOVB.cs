using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOVB : IChunk
{
    public class MOVBBlock : IChunk
    {
        public ushort FirstVertex { get; private set; }
        public ushort Count { get; private set; }

        public MOVBBlock() { }
        public MOVBBlock(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            FirstVertex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x0, 2));
            Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0x2, 2));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public MOVBBlock[] VisibleBlocks { get; private set; }

    public MOVB() { }
    public MOVB(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        VisibleBlocks = new MOVBBlock[data.Length / 4];
        for (int i = 0; i < VisibleBlocks.Length; i++)
        {
            VisibleBlocks[i] = new MOVBBlock(data.Slice(i * 4, 4));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
