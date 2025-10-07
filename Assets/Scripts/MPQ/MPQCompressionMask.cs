using System;

[Flags]
public enum MPQCompressionMask
{
    HUFFMAN = 0x01,
    ZLIB = 0x02,
    PKWARE = 0x04,
    BZIP2 = 0x08,
    SPARSE = 0x10,
    ADPCM_MONO = 0x20,
    ADPCM_STEREO = 0x40,
}