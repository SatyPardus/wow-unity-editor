using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class MPQ : IDisposable
{
    public string FilePath { get; private set; }
    public MPQHeader Header { get; private set; }
    public MemoryMappedFile MemoryMappedFile { get; private set; }

    private MemoryMappedViewStream memoryMappedViewStream;
    private List<MPQHashTableEntry> hashTable = new List<MPQHashTableEntry>();
    private List<MPQBlockTableEntry> blockTable = new List<MPQBlockTableEntry>();

    public MPQ(string filePath)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        FilePath = filePath;

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        MemoryMappedFile = MemoryMappedFile.CreateFromFile(fileStream, null, 0, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
        memoryMappedViewStream = MemoryMappedFile.CreateViewStream(0, 0, MemoryMappedFileAccess.Read);

        memoryMappedViewStream.Seek(0, SeekOrigin.Begin);
        byte[] buffer = new byte[512];
        memoryMappedViewStream.Read(buffer, 0, buffer.Length);

        ReadOnlySpan<byte> span = buffer.AsSpan();
        Header = new MPQHeader(span);

        //Utils.PrintProperties(Header);

        ParseHashTable();
        ParseBlockTable();

        memoryMappedViewStream.Dispose();
        memoryMappedViewStream = null;
    }

    private void ParseHashTable()
    {
        memoryMappedViewStream.Seek(Header.HashTablePos, SeekOrigin.Begin);
        byte[] buffer = new byte[Header.HashTableSize * 0x10];
        memoryMappedViewStream.Read(buffer, 0, buffer.Length);

        Span<byte> span = buffer.AsSpan();

        int uintCount = checked((int)(Header.BlockTableSize << 2));
        long realDataLength = checked(sizeof(uint) * uintCount);

        if (span.Length < realDataLength)
        {
            Debug.LogError("COMPRESSED");
        }

        var hashTableKey = MPQEncryption.HashString("(hash table)", 0x300);
        MPQEncryption.Decrypt(span, hashTableKey);

        for (int i = 0; i < Header.HashTableSize; i++)
        {
            var slice = span.Slice(i * 0x10, 0x10);
            var hashTableEntry = new MPQHashTableEntry(slice);
            hashTable.Add(hashTableEntry);
        }
    }

    private void ParseBlockTable()
    {
        memoryMappedViewStream.Seek(Header.BlockTablePos, SeekOrigin.Begin);
        byte[] buffer = new byte[Header.BlockTableSize * 0x10];
        memoryMappedViewStream.Read(buffer, 0, buffer.Length);

        Span<byte> span = buffer.AsSpan();

        int uintCount = checked((int)(Header.BlockTableSize << 2));
        long realDataLength = checked(sizeof(uint) * uintCount);

        if(span.Length < realDataLength)
        {
            Debug.LogError("COMPRESSED");
        }

        byte[] buffer2 = null;
        if (Header.ExtendedBlockTablePos != 0)
        {
            var dataLength = Header.BlockTableSize * sizeof(uint);

            long realDataLength2 = checked(sizeof(ushort) * Header.BlockTableSize);


            memoryMappedViewStream.Seek(Header.ExtendedBlockTablePos, SeekOrigin.Begin);
            buffer2 = new byte[realDataLength2];
            memoryMappedViewStream.Read(buffer2, 0, (int)realDataLength2);

            if(dataLength < realDataLength2)
            {
                Debug.LogError($"Compressed");
            }
        }
        Span<byte> span2 = new Span<byte>(buffer2);

        var tableKey = MPQEncryption.HashString("(block table)", 0x300);
        MPQEncryption.Decrypt(span, tableKey);

        for (int i = 0; i < Header.BlockTableSize; i++)
        {
            var slice = span.Slice(i * 0x10, 0x10);
            ushort offset = 0;
            if(buffer2 != null)
            {
                offset = BinaryPrimitives.ReadUInt16LittleEndian(span2.Slice(i * 2, 2));
            }
            var tableEntry = new MPQBlockTableEntry(slice, offset);
            blockTable.Add(tableEntry);
        }
    }

    public int FindFile(string filePath)
    {
        int index = (int)(MPQEncryption.HashString(filePath, 0x000) % hashTable.Count);
        var name1 = MPQEncryption.HashString(filePath, 0x100);
        var name2 = MPQEncryption.HashString(filePath, 0x200);

        var startIndex = index;
        do
        {
            var entry = hashTable[index];
            if (entry.BlockIndex == 0xFFFFFFFF) break;

            if (entry.BlockIndex != 0xFFFFFFFE && entry.Name1 == name1 && entry.Name2 == name2)
            {
                return (int)entry.BlockIndex;
            }

            if (++index >= hashTable.Count) index = 0;
        }
        while (index != startIndex);

        return -1;
    }

    public MPQFile LoadFile(string filePath)
    {
        var blockIndex = FindFile(filePath);
        if (blockIndex < 0 || blockIndex >= blockTable.Count) return null;

        var blockEntry = blockTable[blockIndex];
        return new MPQFile(this, blockEntry, filePath);
    }

    public void Dispose()
    {
        MemoryMappedFile.Dispose();
        MemoryMappedFile = null;
    }
}
