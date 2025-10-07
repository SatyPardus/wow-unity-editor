using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MCVT : IChunk
{
    public float[] Heights { get; private set; }

    public MCVT() { }
    public MCVT(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Heights = new float[9 * 9 + 8 * 8];
        for (int i = 0; i < Heights.Length; i++)
        {
            Heights[i] = BitConverter.ToSingle(data.Slice(i * 4, 4));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
