using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

public class MOGI : IChunk
{
    public class SMOGroupInfo : IChunk
    {
        public uint Flags { get; private set; }
        public Bounds BoundingBox { get; private set; }
        public int NameOffset { get; private set; }

        public SMOGroupInfo() { }
        public SMOGroupInfo(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            int offset = 0;

            Flags = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
            BoundingBox = Utils.ReadCAaBox(data.Slice(offset, 24)); offset += 24;
            NameOffset = BinaryPrimitives.ReadInt32LittleEndian(data.Slice(offset, 4)); offset += 4;
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOGroupInfo[] GroupInfos { get; private set; }

    public MOGI() { }
    public MOGI(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        GroupInfos = new SMOGroupInfo[data.Length / 32];

        for (int i = 0; i < GroupInfos.Length; i++)
        {
            GroupInfos[i] = new SMOGroupInfo(data.Slice(i * 32, 32));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
