using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MOPY : IChunk
{
    public class SMOPoly : IChunk
    {
        public byte Flags { get; private set; }
        public byte MaterialID { get; private set; }

        public SMOPoly() { }
        public SMOPoly(Span<byte> data) => Read(data);

        public void Read(Span<byte> data)
        {
            Flags = data[0];
            MaterialID = data[1];
        }

        public void Write(Span<byte> data)
        {
            throw new NotImplementedException();
        }
    }

    public SMOPoly[] PolyList { get; private set; }

    public MOPY() { }
    public MOPY(Span<byte> data) => Read(data);

    public void Read(Span<byte> data)
    {
        PolyList = new SMOPoly[data.Length / 2];

        for (int i = 0; i < PolyList.Length; i++)
        {
            PolyList[i] = new SMOPoly(data.Slice(i * 2));
        }
    }

    public void Write(Span<byte> data)
    {
        throw new NotImplementedException();
    }
}
