using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MWMO : IChunk
{
    public Dictionary<uint, string> Filenames = new Dictionary<uint, string>();

    public MWMO() { }
    public MWMO(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        uint currentStart = 0;
        uint offset = 0;

        while (offset < data.Length)
        {
            var currentByte = data[(int)offset];
            if (currentByte == 0)
            {
                if (offset - currentStart > 0)
                {
                    Filenames.Add(currentStart, Encoding.ASCII.GetString(data.Slice((int)currentStart, (int)(offset - currentStart))));
                }
                currentStart = offset + 1;
            }
            offset++;
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
