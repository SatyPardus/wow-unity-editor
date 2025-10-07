using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ADT
{
    public MCNK[] MCNK { get; private set; }
    public MWMO MWMO { get; private set; }
    public MWID MWID { get; private set; }
    public MODF MODF { get; private set; }

    public ADT(MPQFile file)
    {
        Span<byte> data = new Span<byte>(file.Data);
        uint offset = 0;

        var mverChunk = new IffChunk(data.Slice((int)offset, 8), offset);
        offset += 8;

        if(mverChunk.Token != "MVER")
        {
            throw new Exception($"ADT has invalid chunks (MVER)");
        }

        var mver = new MVER(data.Slice((int)offset, (int)mverChunk.Size));
        offset += mverChunk.Size;
        if (mver.Version != 18)
        {
            throw new Exception($"ADT has invalid version: {mver.Version} (Expected 18)");
        }

        var mhdrChunk = new IffChunk(data.Slice((int)offset, 8), offset);
        offset += 8;
        if (mhdrChunk.Token != "MHDR")
        {
            throw new Exception($"ADT has invalid chunks (MHDR)");
        }

        var mhdr = new MHDR(data.Slice((int)offset, (int)mhdrChunk.Size));

        ReadData(data, mhdr, offset);
    }

    private void ReadData(Span<byte> data, MHDR mhdr, uint offset)
    {
        if (mhdr.OffsetMCIN > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMCIN + offset), "MCIN", out var mcinSize))
        {
            var mcin = new MCIN(data.Slice((int)(mhdr.OffsetMCIN + offset + 8), (int)mcinSize));
            ReadChunks(data, mcin);
        }
        if (mhdr.OffsetMTEX > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMTEX + offset), "MTEX", out var mtexSize))
        {

        }
        if (mhdr.OffsetMMDX > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMMDX + offset), "MMDX", out var mmdxSize))
        {

        }
        if (mhdr.OffsetMMID > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMMID + offset), "MMID", out var mmidSize))
        {

        }
        if (mhdr.OffsetMWMO > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMWMO + offset), "MWMO", out var mwmoSize))
        {
            MWMO = new MWMO(data.Slice((int)(mhdr.OffsetMWMO + offset + 8), (int)mwmoSize));
        }
        if (mhdr.OffsetMWID > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMWID + offset), "MWID", out var mwidSize))
        {
            MWID = new MWID(data.Slice((int)(mhdr.OffsetMWID + offset + 8), (int)mwidSize));
        }
        if (mhdr.OffsetMDDF > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMDDF + offset), "MDDF", out var mddfSize))
        {

        }
        if (mhdr.OffsetMODF > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMODF + offset), "MODF", out var modfSize))
        {
            MODF = new MODF(data.Slice((int)(mhdr.OffsetMODF + offset + 8), (int)modfSize));
        }
        if (mhdr.OffsetMFBO > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMFBO + offset), "MFBO", out var mfboSize))
        {

        }
        if (mhdr.OffsetMH2O > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMH2O + offset), "MH2O", out var mh2oSize))
        {

        }
        if (mhdr.OffsetMTXF > 0 && Utils.ValidateChunk(data, (int)(mhdr.OffsetMTXF + offset), "MTXF", out var mtxfSize))
        {

        }
    }

    private void ReadChunks(Span<byte> data, MCIN mcin)
    {
        MCNK = new MCNK[16 * 16];

        for (int i = 0; i < 16 * 16; i++)
        {
            var chunkInfo = mcin.ChunkInfos[i];

            if (!Utils.ValidateChunk(data, (int)chunkInfo.Offset, "MCNK"))
            {
                throw new Exception($"ADT has invalid chunks (MCNK {i})");
            }

            MCNK[i] = new MCNK(data.Slice((int)chunkInfo.Offset, (int)chunkInfo.Size));
        }
    }
}
