using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LightTransport;
using UnityEngine.UIElements;

public static class MPQCompression
{
    public delegate int SpanAction<T>(Span<T> span, Span<T> destination);
    private static readonly Dictionary<MPQCompressionMask, SpanAction<byte>> compressions = new Dictionary<MPQCompressionMask, SpanAction<byte>>()
    {
        { MPQCompressionMask.HUFFMAN, DecompressHuffman },
        { MPQCompressionMask.ZLIB, DecompressZlib },
        { MPQCompressionMask.PKWARE, DecompressPkware },
        { MPQCompressionMask.BZIP2, DecompressBzip2 },
        { MPQCompressionMask.SPARSE, DecompressSparse },
        { MPQCompressionMask.ADPCM_MONO, DecompressAdpcmMono },
        { MPQCompressionMask.ADPCM_STEREO, DecompressAdpcmStereo }
    };

    private static int DecompressHuffman(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    private static int DecompressZlib(Span<byte> span, Span<byte> destination)
    {
        using (var inStream = new MemoryStream(span.ToArray(), 2, span.Length - 6, writable: false))
        using (var outStream = new DeflateStream(inStream, CompressionMode.Decompress))
        {
            return outStream.Read(destination);
        }
    }

    private static int DecompressPkware(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    private static int DecompressBzip2(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    private static int DecompressSparse(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    private static int DecompressAdpcmMono(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    private static int DecompressAdpcmStereo(Span<byte> span, Span<byte> destination)
    {
        throw new NotImplementedException();
    }

    public static int Decompress(Span<byte> buffer, Span<byte> destination, bool complex)
    {
        var type = buffer[0];
        var compressionMask1 = type;
        var compressionMask2 = type;
        var compressionCount = 0;

        buffer = buffer.Slice(1);

        if (!complex)
        {
            throw new Exception($"Simple Decompression not done yet");
        }

        foreach (var mask in (MPQCompressionMask[])Enum.GetValues(typeof(MPQCompressionMask)))
        {
            if((compressionMask1 & (byte)mask) != 0)
            {
                compressionMask2 &= (byte)~mask;
                compressionCount++;
            }
        }

        if(compressionCount == 0 || compressionMask2 != 0)
        {
            Debug.Log($"{String.Join(", ", buffer.ToArray().Select(b => $"0x{b.ToString("X2")}"))}");
            throw new Exception($"Error! 0x{type.ToString("X8")} {compressionMask1} {compressionMask2} {compressionCount}");
        }

        int outSize = 0;
        var compressionIndex = compressionCount - 1;
        foreach(var mask in compressions.Keys)
        {
            if((compressionMask1 & (byte)mask) != 0)
            {
                compressionIndex--;

                outSize = compressions[mask](buffer, destination);
            }
        }

        return outSize;
    }
}
