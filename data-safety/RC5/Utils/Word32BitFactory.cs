using data_safety.RC5.Constants;
using data_safety.RC5.Models;

namespace data_safety.RC5.Utils;

public class Word32BitFactory : IWordFactory
{
    public int BytesPerWord => Word32Bit.BytesPerWord;

    public int BytesPerBlock => BytesPerWord * 2;

    public IWord Create()
    {
        return CreateConcrete();
    }

    public IWord CreateP()
    {
        return CreateConcrete(RC5Constants.P32);
    }

    public IWord CreateQ()
    {
        return CreateConcrete(RC5Constants.Q32);
    }

    public IWord CreateFromBytes(byte[] bytes, int startFromIndex)
    {
        var word = Create();
        word.CreateFromBytes(bytes, startFromIndex);

        return word;
    }

    public static Word32Bit CreateConcrete(uint value = 0)
    {
        return new Word32Bit
        {
                WordValue = value
        };
    }
}