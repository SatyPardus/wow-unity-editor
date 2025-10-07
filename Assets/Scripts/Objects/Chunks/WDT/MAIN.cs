using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MAIN : IChunk
{
    public bool[] States { get; private set; }

    public MAIN() { }
    public MAIN(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        int offset = 0;

        States = new bool[64 * 64];
        for (int i = 0; i < 64 * 64; i++)
        {
            var flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4));
            States[i] = (flags & 0x1) != 0;
            offset += 8;
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }

    public int GetChunkCount()
    {
        int count = 0;
        for (int i = 0; i < 64 * 64; i++)
        {
            if (States[i])
            {
                count++;
            }
        }
        return count; 
    }

    public bool IsChunkLoaded(int adtX, int adtY)
    {
        if(adtX < 0 || adtX >= 64 || adtY < 0 || adtY >= 64) { return false; }
        return States[adtX + adtY * 64];
    }
}
