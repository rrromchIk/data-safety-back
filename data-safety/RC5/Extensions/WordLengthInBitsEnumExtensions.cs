using data_safety.RC5.Models;
using data_safety.RC5.Models.Enums;
using data_safety.RC5.Utils;

namespace data_safety.RC5.Extensions;

public static class WordLengthInBitsEnumExtensions
{
    public static IWordFactory GetWordFactory(this WordLengthInBitsEnum wordLengthInBits)
    {
        IWordFactory wordFactory = null;

        wordFactory = wordLengthInBits switch
        {
                WordLengthInBitsEnum.Bit16 => new Word16BitFactory(),
                WordLengthInBitsEnum.Bit32 => new Word32BitFactory(),
                WordLengthInBitsEnum.Bit64 => new Word64BitFactory(),
                _ => throw new ArgumentException($"Invalid {nameof(WordLengthInBitsEnum)} value."),
        };
        return wordFactory;
    }
}