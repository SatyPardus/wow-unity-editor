using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MODF : IChunk
{
    public class SMMapObjDef : IChunk
    {
        public uint NameID { get; private set; }
        public uint UniqueID { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Rotation { get; private set; }
        public Bounds Extends { get; private set; }
        public ushort Flags { get; private set; }
        public ushort DoodadSet { get; private set; }
        public ushort NameSet { get; private set; }
        public ushort Scale { get; private set; }

        public SMMapObjDef() { }
        public SMMapObjDef(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            var offset = 0;

            NameID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            UniqueID = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            Position = Utils.ReadVector3(data.Slice(offset, 12)); offset += 12;
            Rotation = Utils.ReadVector3(data.Slice(offset, 12)); offset += 12;
            Extends = Utils.ReadCAaBox(data.Slice(offset, 24)); offset += 24;
            Flags = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            DoodadSet = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            NameSet = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
            Scale = BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset, 2)); offset += 2;
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMMapObjDef[] MapObjects { get; private set; }

    public MODF(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        MapObjects = new SMMapObjDef[data.Length / 0x40];
        for (int i = 0; i < MapObjects.Length; i++)
        {
            MapObjects[i] = new SMMapObjDef(data.Slice(i * 0x40, 0x40));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
