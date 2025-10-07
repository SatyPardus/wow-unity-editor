using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MCVP : IChunk
{
    public Plane[] ConvexPlanes { get; private set; }

    public MCVP() { }
    public MCVP(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        ConvexPlanes = new Plane[data.Length / 16];
        for (int i = 0; i < ConvexPlanes.Length; i++)
        {
            ConvexPlanes[i] = Utils.ReadC4Plane(data.Slice(i * 16));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
