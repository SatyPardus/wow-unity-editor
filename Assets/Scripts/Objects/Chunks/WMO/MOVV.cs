using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOVV : IChunk
{
    public Vector3[] Vertices { get; private set; }

    public MOVV() { }
    public MOVV(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Vertices = new Vector3[data.Length / 12];
        for (int i = 0; i < Vertices.Length; i++)
        {
            Vertices[i] = Utils.ReadVector3(data.Slice(i * 12));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
