using System;
using System.IO;
using UnityEngine;

public class WDT
{
    private uint offset;

    public string Name { get; private set; }
    public MVER MVER { get; private set; }
    public MAIN MAIN { get; private set; }
    public MPHD MPHD { get; private set; }
    public MWMO MWMO { get; private set; }

    public WDT(MPQFile file)
    {
        Name = Path.GetFileNameWithoutExtension(file.FileName);

        Span<byte> data = file.Data;
        offset = 0;

        while(offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice((int)offset, 8), offset);
            offset += 8;

            switch (chunk.Token)
            {
                case "MVER": MVER = new MVER(data.Slice((int)offset, (int)chunk.Size)); break;
                case "MAIN": MAIN = new MAIN(data.Slice((int)offset, (int)chunk.Size)); break;
                case "MPHD": MPHD = new MPHD(data.Slice((int)offset, (int)chunk.Size)); break;
                case "MWMO": MWMO = new MWMO(data.Slice((int)offset, (int)chunk.Size)); break;
                default: Debug.LogWarning($"Unhandled chunk: {chunk.Token} ({chunk.Size} bytes)"); break;
            }
            offset += chunk.Size;
        }
    }
}
