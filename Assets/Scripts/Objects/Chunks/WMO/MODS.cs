using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MODS : IChunk
{
    public class SMODoodadSet : IChunk
    {
        public string Name { get; private set; }
        public uint StartIndex { get; private set; }
        public uint Count { get; private set; }

        public SMODoodadSet() { }
        public SMODoodadSet(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Name = Encoding.ASCII.GetString(data.Slice(0, 20));
            StartIndex = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(20, 4));
            Count = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(24, 4));
            // padding 4
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMODoodadSet[] DoodadSets { get; private set; }

    public MODS() { }
    public MODS(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        DoodadSets = new SMODoodadSet[data.Length / 30];
        for (int i = 0; i < DoodadSets.Length; i++)
        {
            DoodadSets[i] = new SMODoodadSet(data.Slice(i * 30, 30));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
