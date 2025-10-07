using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MWID : IChunk
{
    public uint[] Offsets { get; private set; }

    public MWID() { }
    public MWID(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Offsets = new uint[data.Length / 4];
        for (int i = 0; i < Offsets.Length; i++)
        {
            Offsets[i] = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(i * 4, 4));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
