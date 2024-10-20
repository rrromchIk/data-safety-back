using data_safety.RC5.Models.Enums;

namespace data_safety.RC5;

public class RC5Settings
{
    public RoundCountEnum RoundCount { get; set; }

    public WordLengthInBitsEnum WordLengthInBits { get; set; }

    public KeyLengthInBytesEnum KeyLengthInBytes { get; set; }
}