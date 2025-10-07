using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOGN : IChunk
{
    public Dictionary<int, string> GroupNameList { get; private set; }

    public MOGN() { }
    public MOGN(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        GroupNameList = new Dictionary<int, string>();

        int currentStart = 0;
        int offset = 0;

        while (offset < data.Length)
        {
            var currentByte = data[(int)offset];
            if (currentByte == 0)
            {
                if (offset - currentStart > 0)
                {
                    GroupNameList.Add(currentStart, Encoding.ASCII.GetString(data.Slice((int)currentStart, (int)(offset - currentStart))));
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
