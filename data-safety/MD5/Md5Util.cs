using System.Text;

namespace data_safety.MD5;

public class Md5Util
{
    private const int OptimalChunkSizeMultiplier = 100_000;
    private const uint OptimalChunkSize = MD5Constants.BytesCountPerBits512Block * OptimalChunkSizeMultiplier;
    public MessageDigest Hash { get; private set; }

    public string HashAsString => Hash.ToString();
    
    public uint FuncF(uint B, uint C, uint D) => (B & C) | (~B & D);

    /// <summary>
    /// Elementary function G(B, C, D)
    /// </summary>
    public uint FuncG(uint B, uint C, uint D) => (D & B) | (C & ~D);
    
    public uint FuncH(uint B, uint C, uint D) => B ^ C ^ D;
    
    public uint FuncI(uint B, uint C, uint D) => C ^ (B | ~D);
    
    public void ComputeHash(string message)
    {
        ComputeHash(Encoding.ASCII.GetBytes(message));
    }

    
    public MessageDigest ComputeHash(byte[] message)
    {
        Hash = MessageDigest.InitialValue;

        var paddedMessage = message.Concat(GetMessagePadding((uint)message.Length)).ToArray();

        for (uint bNo = 0; bNo < paddedMessage.Length / MD5Constants.BytesCountPerBits512Block; ++bNo)
        {
            uint[] X = Extract32BitWords(
                    paddedMessage,
                    bNo,
                    MD5Constants.Words32BitArraySize * MD5Constants.BytesPer32BitWord);

            FeedMessageBlockToBeHashed(X);
        }

        return Hash;
    }

    
    public async Task<MessageDigest> ComputeFileHashAsync(string filePath)
    {
        Hash = MessageDigest.InitialValue;
        
        await using (var fs = File.OpenRead(filePath))
        {
            ulong totalBytesRead = 0;
            int currentBytesRead = 0;
            bool isFileEnd = false;
            do
            {
                var chunk = new byte[OptimalChunkSize];

                currentBytesRead = await fs.ReadAsync(chunk, 0, chunk.Length);
                totalBytesRead += (ulong)currentBytesRead;


                if (currentBytesRead < chunk.Length)
                {
                    byte[] lastChunk;

                    if (currentBytesRead == 0)
                    {
                        lastChunk = GetMessagePadding(totalBytesRead);
                    }
                    else
                    {
                        lastChunk = new byte[currentBytesRead];
                        Array.Copy(chunk, lastChunk, currentBytesRead);

                        lastChunk = lastChunk.Concat(GetMessagePadding(totalBytesRead)).ToArray();
                    }

                    chunk = lastChunk;
                    isFileEnd = true;
                }

                for (uint bNo = 0; bNo < chunk.Length / MD5Constants.BytesCountPerBits512Block; ++bNo)
                {
                    uint[] X = Extract32BitWords(
                            chunk,
                            bNo,
                            MD5Constants.Words32BitArraySize * MD5Constants.BytesPer32BitWord);

                    FeedMessageBlockToBeHashed(X);
                }
            } while (!isFileEnd);
        }

        return Hash;
    }

    private void FeedMessageBlockToBeHashed(uint[] X)
    {
        uint F, i, k;
        var blockSize = MD5Constants.BytesCountPerBits512Block;
        var MDq = Hash.Clone();

        // first сycle
        for (i = 0; i < blockSize / 4; ++i)
        {
            k = i;
            F = FuncF(MDq.B, MDq.C, MDq.D);

            MDq.Md5IterationSwap(F, X, i, k);
        }

        // second сycle
        for (; i < blockSize / 2; ++i)
        {
            k = (1 + (5 * i)) % (blockSize / 4);
            F = FuncG(MDq.B, MDq.C, MDq.D);

            MDq.Md5IterationSwap(F, X, i, k);
        }

        // third сycle
        for (; i < blockSize / 4 * 3; ++i)
        {
            k = (5 + (3 * i)) % (blockSize / 4);
            F = FuncH(MDq.B, MDq.C, MDq.D);

            MDq.Md5IterationSwap(F, X, i, k);
        }

        // fourth сycle
        for (; i < blockSize; ++i)
        {
            k = 7 * i % (blockSize / 4);
            F = FuncI(MDq.B, MDq.C, MDq.D);

            MDq.Md5IterationSwap(F, X, i, k);
        }

        Hash += MDq;
    }

    private byte[] GetMessagePadding(ulong messageLength)
    {
        uint paddingLengthInBytes = default;
        var mod = (uint)(messageLength * MD5Constants.BitsPerByte % MD5Constants.Bits512BlockSize);

        // Append Padding Bits
        if (mod == MD5Constants.BITS_448)
        {
            paddingLengthInBytes = MD5Constants.Bits512BlockSize / MD5Constants.BitsPerByte;
        }
        else if (mod > MD5Constants.BITS_448)
        {
            paddingLengthInBytes =
                    (MD5Constants.Bits512BlockSize - mod + MD5Constants.BITS_448) / MD5Constants.BitsPerByte;
        }
        else if (mod < MD5Constants.BITS_448)
        {
            paddingLengthInBytes = (MD5Constants.BITS_448 - mod) / MD5Constants.BitsPerByte;
        }

        var padding = new byte[paddingLengthInBytes + MD5Constants.BitsPerByte];
        padding[0] = MD5Constants.BITS_128;

        // Append Length
        var messageLength64bit = messageLength * MD5Constants.BitsPerByte;

        for (var i = 0; i < MD5Constants.BitsPerByte; ++i)
        {
            padding[paddingLengthInBytes + i] = (byte)(messageLength64bit
                                                       >> (int)(i * MD5Constants.BitsPerByte)
                                                       & MD5Constants.BITS_255);
        }

        return padding;
    }

    public uint[] Extract32BitWords(byte[] message, uint blockNo, uint blockSizeInBytes)
    {
        var messageStartIndex = blockNo * blockSizeInBytes;
        var extractedArray = new uint[blockSizeInBytes / MD5Constants.BytesPer32BitWord];

        for (uint i = 0; i < blockSizeInBytes; i += MD5Constants.BytesPer32BitWord)
        {
            var j = messageStartIndex + i;

            extractedArray[i / MD5Constants.BytesPer32BitWord] = // form 32-bit word from four bytes
                    message[j] // first byte
                    | (((uint)message[j + 1]) << ((int)MD5Constants.BitsPerByte * 1)) // second byte
                    | (((uint)message[j + 2]) << ((int)MD5Constants.BitsPerByte * 2)) // third byte
                    | (((uint)message[j + 3]) << ((int)MD5Constants.BitsPerByte * 3)); // fourth byte
        }

        return extractedArray;
    }
}