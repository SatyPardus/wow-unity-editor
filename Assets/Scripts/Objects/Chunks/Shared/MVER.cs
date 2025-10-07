using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MVER : IChunk
{
    public uint Version { get; private set; }

    public MVER() { }
    public MVER(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Version = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x0, 4));
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
