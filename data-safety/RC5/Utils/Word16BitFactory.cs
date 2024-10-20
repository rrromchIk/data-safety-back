using data_safety.RC5.Constants;
using data_safety.RC5.Models;

namespace data_safety.RC5.Utils;

public class Word16BitFactory : IWordFactory
{
    public int BytesPerWord => Word16Bit.BytesPerWord;

    public int BytesPerBlock => BytesPerWord * 2;

    public IWord Create()
    {
        return CreateConcrete();
    }

    public IWord CreateP()
    {
        return CreateConcrete(RC5Constants.P16);
    }

    public IWord CreateQ()
    {
        return CreateConcrete(RC5Constants.Q16);
    }

    public IWord CreateFromBytes(byte[] bytes, int startFromIndex)
    {
        var word = Create();
        word.CreateFromBytes(bytes, startFromIndex);

        return word;
    }

    private static Word16Bit CreateConcrete(ushort value = 0)
    {
        return new Word16Bit
        {
                WordValue = value
        };
    }
}