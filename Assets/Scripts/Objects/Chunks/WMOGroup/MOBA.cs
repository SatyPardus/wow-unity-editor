using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOBA : IChunk
{
    public class SMOBatch : IChunk
    {
        public Bounds BoundingBox { get; private set; }
        public uint StartIndex { get; private set; }
        public ushort Count { get; private set; }
        public ushort MinIndex { get; private set; }
        public ushort MaxIndex { get; private set; }
        public byte Flags { get; private set; }
        public byte MaterialID { get; private set; }

        public SMOBatch() { }
        public SMOBatch(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            int offset = 0;

            var minX = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            var minY = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            var minZ = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            var maxX = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            var maxY = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            var maxZ = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2)); offset += 2;

            var bounds = new Bounds();
            bounds.min = new Vector3(minX, minY, minZ);
            bounds.max = new Vector3(maxX, maxY, maxZ);
            BoundingBox = bounds;

            StartIndex = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Count = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            MinIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            MaxIndex = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            Flags = data[offset++];
            MaterialID = data[offset++];
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOBatch[] Batches { get; private set; }

    public MOBA() { }
    public MOBA(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        Batches = new SMOBatch[data.Length / 24];

        for (int i = 0; i < Batches.Length; i++)
        {
            Batches[i] = new SMOBatch(data.Slice(i * 24));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
