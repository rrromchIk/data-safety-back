﻿namespace data_safety.MD5;

internal static class MD5Constants
{
    public const uint BitsPerByte = 8;
    public const uint BytesPer32BitWord = 4;
    public const uint Words32BitArraySize = 16;
    public const uint Bits512BlockSize = 512u;
    public const uint BytesCountPerBits512Block = Bits512BlockSize / BitsPerByte; //64
    public const uint BITS_448 = 448u;
    public const byte BITS_255 = 0b11111111;
    public const byte BITS_128 = 0b10000000;

    /// <summary>
    /// Initial value for A word of MD buffer
    /// </summary>
    public const uint A_MD_BUFFER_INITIAL = 0x67452301;

    /// <summary>
    /// Initial value for B word of MD buffer
    /// </summary>
    public const uint B_MD_BUFFER_INITIAL = 0xefcdab89;

    /// <summary>
    /// Initial value for C word of MD buffer
    /// </summary>
    public const uint C_MD_BUFFER_INITIAL = 0x98badcfe;

    /// <summary>
    /// Initial value for D word of MD buffer
    /// </summary>
    public const uint D_MD_BUFFER_INITIAL = 0x10325476;

    /// <summary>
    /// Lookup table with values based on sin() values
    /// </summary>
    public static readonly uint[] T =
    {
            0xd76aa478, 0xe8c7b756, 0x242070db, 0xc1bdceee,
            0xf57c0faf, 0x4787c62a, 0xa8304613, 0xfd469501,
            0x698098d8, 0x8b44f7af, 0xffff5bb1, 0x895cd7be,
            0x6b901122, 0xfd987193, 0xa679438e, 0x49b40821,
            0xf61e2562, 0xc040b340, 0x265e5a51, 0xe9b6c7aa,
            0xd62f105d, 0x2441453, 0xd8a1e681, 0xe7d3fbc8,
            0x21e1cde6, 0xc33707d6, 0xf4d50d87, 0x455a14ed,
            0xa9e3e905, 0xfcefa3f8, 0x676f02d9, 0x8d2a4c8a,
            0xfffa3942, 0x8771f681, 0x6d9d6122, 0xfde5380c,
            0xa4beea44, 0x4bdecfa9, 0xf6bb4b60, 0xbebfbc70,
            0x289b7ec6, 0xeaa127fa, 0xd4ef3085, 0x4881d05,
            0xd9d4d039, 0xe6db99e5, 0x1fa27cf8, 0xc4ac5665,
            0xf4292244, 0x432aff97, 0xab9423a7, 0xfc93a039,
            0x655b59c3, 0x8f0ccc92, 0xffeff47d, 0x85845dd1,
            0x6fa87e4f, 0xfe2ce6e0, 0xa3014314, 0x4e0811a1,
            0xf7537e82, 0xbd3af235, 0x2ad7d2bb, 0xeb86d391
    };

    /// <summary>
    /// Lookup table with round shift values
    /// </summary>
    public static readonly int[] S =
    {
            7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22,
            5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20,
            4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23,
            6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21
    };
}