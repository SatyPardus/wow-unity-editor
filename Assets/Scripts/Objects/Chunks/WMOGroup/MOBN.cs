using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOBN : IChunk
{
    public class CAaBspNode : IChunk
    {
        public ushort Flags { get; private set; }
        public short NegativeChild { get; private set; }
        public short PositiveChild { get; private set; }
        public ushort FaceCount { get; private set; }
        public uint FaceStart { get; private set; }
        public float PlaneDistance { get; private set; }

        public CAaBspNode() { }
        public CAaBspNode(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(0, 2));
            NegativeChild = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(2, 2));
            PositiveChild = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(4, 2));
            FaceCount = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(6, 2));
            FaceStart = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(8, 4));
            PlaneDistance = BitConverter.ToSingle(data.Slice(12, 4));
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public CAaBspNode[] BSPNodes { get; private set; }

    public MOBN() { }
    public MOBN(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        BSPNodes = new CAaBspNode[data.Length / 16];
        for (int i = 0; i < BSPNodes.Length; i++)
        {
            BSPNodes[i] = new CAaBspNode(data.Slice(i * 16, 16));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
