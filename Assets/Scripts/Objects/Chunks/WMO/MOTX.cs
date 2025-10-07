using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOTX : IChunk
{
    public Dictionary<uint, string> TextureNameList { get; private set; }

    public MOTX() { }
    public MOTX(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        TextureNameList = new Dictionary<uint, string>();

        uint currentStart = 0;
        uint offset = 0;

        while (offset < data.Length)
        {
            var currentByte = data[(int)offset];
            if (currentByte == 0)
            {
                if (offset - currentStart > 0)
                {
                    TextureNameList.Add(currentStart, Encoding.ASCII.GetString(data.Slice((int)currentStart, (int)(offset - currentStart))));
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
