using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MPQEncryption
{
    private static uint[] encryptionTable = BuildEncryptionTable();

    private static uint[] BuildEncryptionTable()
    {
        int q, r = 0x100001;
        uint seed;

        uint[] encryptionTable = new uint[0x500];

        for (int i = 0; i < 0x100; i++)
            for (int j = 0; j < 5; j++)
            {
                unchecked
                {
                    q = Math.DivRem(r * 125 + 3, 0x2AAAAB, out r);
                    seed = (uint)(r & 0xFFFF) << 16;
                    q = Math.DivRem(r * 125 + 3, 0x2AAAAB, out r);
                    seed |= (uint)(r & 0xFFFF);
                    encryptionTable[0x100 * j + i] = seed;
                }
            }

        return encryptionTable;
    }

    public static uint HashString(string text, int hashOffset)
    {
        uint hash = 0x7FED7FED, seed = 0xEEEEEEEE;
        byte[] buffer = new byte[text.Length];
        char c;
        byte b;

        for (int i = 0; i < text.Length; i++)
            unchecked
            {
                c = text[i]; // The 128 first Unicode characters are the 128 ASCII characters, so it's fine like this
                if (c >= 128)
                    c = '?'; // Replace invalid ascii characters with this...
                b = (byte)c;
                if (b > 0x60 && b < 0x7B)
                    b -= 0x20;
                hash = encryptionTable[hashOffset + b] ^ (hash + seed);
                seed += hash + (seed << 5) + b + 3;
            }
        return hash;
    }

    public static (uint, uint) Decrypt(Span<byte> data, uint hash, uint temp = 0xEEEEEEEE)
    {
        int dataIndex = 0;

        uint buffer = 0;
        var length = data.Length >> 2;
        for (int i = length; i-- != 0; )
        {
            unchecked
            {
                temp += encryptionTable[0x400 + (hash & 0xFF)];
                buffer = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(dataIndex * 4, 4)) ^ (temp + hash);
                temp += buffer + (temp << 5) + 3;
                BinaryPrimitives.WriteUInt32LittleEndian(data.Slice(dataIndex * 4, 4), buffer);
                hash = (hash >> 11) | (0x11111111 + ((hash ^ 0x7FF) << 21));
                dataIndex++;
            }
        }

        return (hash, temp);
    }
}
