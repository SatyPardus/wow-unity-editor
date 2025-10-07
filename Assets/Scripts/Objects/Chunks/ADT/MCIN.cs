using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MCIN : IChunk
{
    public class ChunkInfo : IChunk
    {
        public uint Offset { get; private set; }
        public uint Size { get; private set; }

        public ChunkInfo() { }
        public ChunkInfo(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Offset = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0, 4));
            Size = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x4, 4));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public ChunkInfo[] ChunkInfos { get; private set; }

    public MCIN() { }
    public MCIN(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        ChunkInfos = new ChunkInfo[16 * 16];
        for (int i = 0; i < 16 * 16; i++)
        {
            ChunkInfos[i] = new ChunkInfo(data.Slice(i * 16, 8));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
