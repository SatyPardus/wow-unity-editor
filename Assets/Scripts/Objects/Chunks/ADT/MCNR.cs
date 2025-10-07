using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MCNR : IChunk
{
    public Vector3[] Normals { get; private set; }

    public MCNR() { }
    public MCNR(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Normals = new Vector3[9 * 9 + 8 * 8];
        for (int i = 0; i < Normals.Length; i++)
        {
            Normals[i] = new Vector3(
                data[i * 3 + 0] / 255f,
                data[i * 3 + 1] / 255f,
                data[i * 3 + 2] / 255f
            );
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
