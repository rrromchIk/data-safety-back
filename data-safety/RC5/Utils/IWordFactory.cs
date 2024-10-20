using data_safety.RC5.Models;

namespace data_safety.RC5.Utils;

public interface IWordFactory
{
    int BytesPerWord { get; }
    int BytesPerBlock { get; }
    IWord CreateQ();
    IWord CreateP();
    IWord Create();
    IWord CreateFromBytes(byte[] bytes, int startFrom);
}