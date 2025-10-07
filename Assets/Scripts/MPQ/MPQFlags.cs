using System;

[Flags]
public enum MPQFlags : uint
{
    NONE               = 0x00000000,
    FILE_IMPLODE       = 0x00000100,
    FILE_COMPRESS      = 0x00000200,
    FILE_ENCRYPT       = 0x00010000,
    FILE_FIX_KEY       = 0x00020000,
    FILE_PATCH         = 0x00100000,
    FILE_SINGLE_UNIT   = 0x01000000,
    FILE_DELETE_MARKER = 0x02000000,
    FILE_SECTOR_CRC    = 0x04000000,
    FILE_EXISTS        = 0x80000000
}