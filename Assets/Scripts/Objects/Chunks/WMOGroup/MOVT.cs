using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOVT : IChunk
{
    public Vector3[] Vertices { get; private set; }

    public MOVT() { }
    public MOVT(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var count = data.Length / 12;

        Vertices = new Vector3[count];
        for (var i = 0; i < count; i++)
        {
            Vertices[i] = Utils.ReadVector3(data.Slice(i * 12, 12));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
