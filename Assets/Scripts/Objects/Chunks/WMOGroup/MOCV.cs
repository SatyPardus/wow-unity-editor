using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOCV : IChunk
{
    public Color[] VertexColors { get; private set; }

    public MOCV() { }
    public MOCV(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var count = data.Length / 4;

        VertexColors = new Color[count];
        for (var i = 0; i < count; i++)
        {
            VertexColors[i] = Utils.ReadCImVector(data.Slice(i * 4, 4));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
