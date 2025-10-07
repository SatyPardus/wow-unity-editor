using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOTV : IChunk
{
    public Vector2[] UVs { get; private set; }

    public MOTV() { }
    public MOTV(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        var count = data.Length / 8;

        UVs = new Vector2[count];
        for (var i = 0; i < count; i++)
        {
            UVs[i] = Utils.ReadVector2(data.Slice(i * 8, 8));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
