using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IffChunk
{
    public uint BaseOffset { get; private set; }
    public string Token { get; private set; }
    public uint Size { get; private set; }

    public IffChunk(Span<byte> data, uint offset)
    {
        BaseOffset = offset;

        var slice = data.Slice(0x0, 4);
        byte[] reversed = slice.ToArray();
        Array.Reverse(reversed);
        Token = Encoding.ASCII.GetString(reversed);
        Size = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0x4, 4));
    }
}
