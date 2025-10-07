using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOLR : IChunk
{
    public ushort[] LightRefs { get; private set; }

    public MOLR() { }
    public MOLR(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var count = data.Length / 2;

        LightRefs = new ushort[count];
        for (var i = 0; i < count; i++)
        {
            LightRefs[i] = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(i * 2, 2));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
