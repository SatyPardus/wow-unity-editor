using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MONR : IChunk
{
    public Vector3[] Normals { get; private set; }

    public MONR() { }
    public MONR(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var count = data.Length / 12;

        Normals = new Vector3[count];
        for (var i = 0; i < count; i++)
        {
            Normals[i] = Utils.ReadVector3(data.Slice(i * 12, 12));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
