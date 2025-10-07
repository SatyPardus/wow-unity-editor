using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MPQFile : IDisposable
{
    private MPQ mpq;
    private MemoryMappedViewStream stream;

    public string FilePath { get; private set; }
    public string FileName { get; private set; }
    public byte[] Data { get; set; }

    public MPQFile(byte[] data, string filePath)
    {
        Data = data;
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
    }

    public MPQFile(MPQ mpq, MPQBlockTableEntry blockTableEntry, string filePath)
    {
        this.mpq = mpq;
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);

        Data = new byte[blockTableEntry.UncompressedSize];

        stream = mpq.MemoryMappedFile.CreateViewStream(blockTableEntry.FilePos, blockTableEntry.UncompressedSize, MemoryMappedFileAccess.Read);
        //Debug.Log(mpq.FilePath);
        //Utils.PrintProperties(mpq.Header);
        //Utils.PrintProperties(blockTableEntry);

        byte[] buffer = new byte[blockTableEntry.CompressedSize];
        stream.Read(buffer, 0, buffer.Length);

        var isSingleUnit = blockTableEntry.Flags.HasFlag(MPQFlags.FILE_SINGLE_UNIT);
        var isCompressed = blockTableEntry.Flags.HasFlag(MPQFlags.FILE_COMPRESS) || blockTableEntry.Flags.HasFlag(MPQFlags.FILE_IMPLODE);
        var isPatch = blockTableEntry.Flags.HasFlag(MPQFlags.FILE_PATCH);
        var isEncrypted = blockTableEntry.Flags.HasFlag(MPQFlags.FILE_ENCRYPT);

        
        var lastBlockSize = blockTableEntry.UncompressedSize > 0 ? blockTableEntry.UncompressedSize % mpq.Header.BlockSize : 0;
        if(lastBlockSize == 0)
        {
            lastBlockSize = mpq.Header.BlockSize;
        }

        if (isPatch)
        {
            throw new Exception($"Patch not handled yet");
        }

        uint seed = 0;
        if (isEncrypted)
        {
            seed = MPQEncryption.HashString(FileName, 0x300);
            if (blockTableEntry.Flags.HasFlag(MPQFlags.FILE_FIX_KEY))
            {
                var b = seed;
                seed = (seed + (uint)blockTableEntry.FilePos) ^ blockTableEntry.UncompressedSize;
            }
        }

        List<uint> offsets = new List<uint>();

        if (isSingleUnit)
        {
            offsets.Add(0);
            offsets.Add(blockTableEntry.CompressedSize);
        }
        else if (isCompressed)
        {
            offsets = ReadBlockOffsets(0, (int)((blockTableEntry.UncompressedSize + mpq.Header.BlockSize - 1) / mpq.Header.BlockSize + 1), seed);
        }
        else
        {
            offsets.Add(0);
            for (int i = 1; i < (int)((blockTableEntry.UncompressedSize + mpq.Header.BlockSize - 1) / mpq.Header.BlockSize + 1); i++)
            {
                var val = offsets[i - 1] + mpq.Header.BlockSize;
                if(val > blockTableEntry.UncompressedSize)
                {
                    val = blockTableEntry.UncompressedSize;
                }
                offsets.Add(val);
            }
        }

        //Debug.Log($"Offsets: {String.Join(", ", offsets.ToArray())}");

        int length = 0;
        Span<byte> fileSpan = new Span<byte>(Data);
        for (uint i = 0; i < offsets.Count - 1; i++)
        {
            var size = (int)(offsets[(int)i + 1] - offsets[(int)i]);
            stream.Seek(offsets[(int)i], System.IO.SeekOrigin.Begin);
            stream.Read(Data, (int)i * mpq.Header.BlockSize, size);
            var slice = fileSpan.Slice((int)i * mpq.Header.BlockSize, size);

            if (isEncrypted)
            {
                MPQEncryption.Decrypt(slice, seed + i);
            }

            if (isCompressed &&
                !(size == mpq.Header.BlockSize) &&
                !(i == offsets.Count - 2 && size == lastBlockSize))
            {
                byte[] outBuffer = new byte[mpq.Header.BlockSize];
                var outSpan = new Span<byte>(outBuffer);
                var readBytes = MPQCompression.Decompress(slice, outSpan, blockTableEntry.Flags.HasFlag(MPQFlags.FILE_COMPRESS));
                length += readBytes;
                outSpan.Slice(0, readBytes).CopyTo(fileSpan.Slice((int)i * mpq.Header.BlockSize));
            }
            else
            {
                length += size;
            }

            if(length > blockTableEntry.UncompressedSize)
            {
                break;
            }
        }

        if (length < blockTableEntry.UncompressedSize)
        {
            Debug.LogError($"File is to smol. {length} != {blockTableEntry.UncompressedSize} -> {FilePath}");
        }
    }

    private List<uint> ReadBlockOffsets(long offset, int count, uint seed)
    {
        List<uint> offsets = new List<uint>(count);

        var length = count * 4;
        byte[] buffer = new byte[length];

        stream.Seek(offset, System.IO.SeekOrigin.Begin);
        var bytesRead = stream.Read(buffer, 0, length);
        if(bytesRead != length)
        {
            throw new Exception($"could not read full block offsets");
        }

        //Debug.Log($"Reading {count} from {offset} ({seed})");

        Span<byte> span = new Span<byte>(buffer);

        if (seed != 0)
        {
            MPQEncryption.Decrypt(span, seed - 1);
        }

        for (int i = 0; i < count; i++)
        {
            offsets.Add(BinaryPrimitives.ReadUInt32LittleEndian(span.Slice(i * 4, 4)));
        }

        return offsets;
    }

    public void Save(string folder)
    {
        Directory.CreateDirectory(Path.Combine(folder, Path.GetDirectoryName(FilePath)));
        File.WriteAllBytes(Path.Combine(folder, FilePath), Data);
    }

    public void Dispose()
    {
        Data = null;

        stream?.Dispose();
        stream = null;
    }
}
