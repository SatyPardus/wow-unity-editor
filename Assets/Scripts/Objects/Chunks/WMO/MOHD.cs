using System;
using System.Buffers.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOHD : IChunk
{
    public uint TextureCount { get; private set; }
    public uint GroupCount { get; private set; }
    public uint PortalCount { get; private set; }
    public uint LightCount { get; private set; }
    public uint DoodadNameCount { get; private set; }
    public uint DoodadDefCount { get; private set; }
    public uint DoodadSetCount { get; private set; }
    public Color AmbientColor { get; private set; }
    public uint WMOID { get; private set; }
    public Bounds BoundingBox { get; private set; }
    public ushort Flags { get; private set; }
    public ushort NumLOD { get; private set; }

    public MOHD() { }
    public MOHD(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        TextureCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
        GroupCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(4, 4));
        PortalCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(8, 4));
        LightCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(12, 4));
        DoodadNameCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(16, 4));
        DoodadDefCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(20, 4));
        DoodadSetCount = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(24, 4));
        AmbientColor = Utils.ReadColor(data.Slice(28, 4));
        WMOID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(32, 4));
        BoundingBox = Utils.ReadCAaBox(data.Slice(36, 24));
        Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(60, 2));
        NumLOD = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(62, 2));
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
