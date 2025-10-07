using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOSB : IChunk
{
    public string SkyboxName { get; private set; }

    public MOSB() { }
    public MOSB(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        int currentStart = 0;
        int offset = 0;

        while (offset < data.Length)
        {
            var currentByte = data[(int)offset];
            if (currentByte == 0)
            {
                if (offset - currentStart > 0)
                {
                    SkyboxName = Encoding.ASCII.GetString(data.Slice((int)currentStart, (int)(offset - currentStart)));
                }
                break;
            }
            offset++;
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
