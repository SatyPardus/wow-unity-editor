using System;
using System.IO;
using UnityEngine;

public class WMO
{
    public string Directory { get; private set; }
    public string Name { get; private set; }

    public MVER MVER { get; private set; }
    public MOHD MOHD { get; private set; }
    public MOTX MOTX { get; private set; }
    public MOMT MOMT { get; private set; }
    public MOGN MOGN { get; private set; }
    public MOGI MOGI { get; private set; }
    public MOSB MOSB { get; private set; }
    public MOPV MOPV { get; private set; }
    public MOPT MOPT { get; private set; }
    public MOPR MOPR { get; private set; }
    public MOVV MOVV { get; private set; }
    public MOVB MOVB { get; private set; }
    public MOLT MOLT { get; private set; }
    public MODS MODS { get; private set; }
    public MODN MODN { get; private set; }
    public MODD MODD { get; private set; }
    public MFOG MFOG { get; private set; }
    public MCVP MCVP { get; private set; }

    public WMO(MPQFile file)
    {
        Directory = Path.GetDirectoryName(file.FilePath);
        Name = Path.GetFileNameWithoutExtension(file.FilePath);

        Span<byte> data = new Span<byte>(file.Data);

        int offset = 0;
        while (offset + 8 <= data.Length)
        {
            var chunk = new IffChunk(data.Slice((int)offset, 8), (uint)offset);
            offset += 8;

            switch (chunk.Token)
            {
                case "MVER": MVER = new MVER(data.Slice(offset, (int)chunk.Size)); break;
                case "MOHD": MOHD = new MOHD(data.Slice(offset, (int)chunk.Size)); break;
                case "MOTX": MOTX = new MOTX(data.Slice(offset, (int)chunk.Size)); break;
                case "MOMT": MOMT = new MOMT(data.Slice(offset, (int)chunk.Size)); break;
                case "MOGN": MOGN = new MOGN(data.Slice(offset, (int)chunk.Size)); break;
                case "MOGI": MOGI = new MOGI(data.Slice(offset, (int)chunk.Size)); break;
                case "MOSB": MOSB = new MOSB(data.Slice(offset, (int)chunk.Size)); break;
                case "MOPV": MOPV = new MOPV(data.Slice(offset, (int)chunk.Size)); break;
                case "MOPT": MOPT = new MOPT(data.Slice(offset, (int)chunk.Size)); break;
                case "MOPR": MOPR = new MOPR(data.Slice(offset, (int)chunk.Size)); break;
                case "MOVV": MOVV = new MOVV(data.Slice(offset, (int)chunk.Size)); break;
                case "MOVB": MOVB = new MOVB(data.Slice(offset, (int)chunk.Size)); break;
                case "MOLT": MOLT = new MOLT(data.Slice(offset, (int)chunk.Size)); break;
                case "MODS": MODS = new MODS(data.Slice(offset, (int)chunk.Size)); break;
                case "MODN": MODN = new MODN(data.Slice(offset, (int)chunk.Size)); break;
                case "MODD": MODD = new MODD(data.Slice(offset, (int)chunk.Size)); break;
                case "MFOG": MFOG = new MFOG(data.Slice(offset, (int)chunk.Size)); break;
                case "MCVP": MCVP = new MCVP(data.Slice(offset, (int)chunk.Size)); break;
                default: throw new Exception($"[WMO] Unhandled chunk: {chunk.Token} ({chunk.Size} bytes)");
            }
            offset += (int)chunk.Size;
        }

        //Utils.PrintProperties(MOHD);

        //foreach(var group in MOGI.GroupInfos)
        //{
        //    Debug.Log($"{group.NameOffset}");
        //    if(group.NameOffset != -1)
        //    {
        //        Debug.Log(MOGN.GroupNameList[group.NameOffset]);
        //    }
        //}
    }
}
