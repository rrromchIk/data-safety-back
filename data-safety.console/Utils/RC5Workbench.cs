using System.Diagnostics;
using System.Text;
using data_safety.RC5;
using data_safety.RC5.Models.Enums;

namespace data_safet.console.Utils;

public class RC5Workbench
{
    public static void MeasureTime(string message)
    {
        var rc5 = new RC5Util(new RC5Settings
                {
                        RoundCount = RoundCountEnum.Rounds_12,
                        WordLengthInBits = WordLengthInBitsEnum.Bit32,
                        KeyLengthInBytes = KeyLengthInBytesEnum.Bytes_16
                },
                "key");
        var stopwatch = new Stopwatch();

        Console.WriteLine("RC5 in the CBC-PAD mode.");
        Console.WriteLine("Using RoundCount: 12, WordLength: 32bits, KeyLength: 16bytes");
        
        stopwatch.Start();
        var encryptedMessage = rc5.EncryptCBCPAD(Encoding.UTF8.GetBytes(message));
        stopwatch.Stop();
        
        Console.WriteLine($"RC5 Encrypted Message: {Encoding.UTF8.GetString(encryptedMessage)}");
        Console.WriteLine($"RC5 Encryption Time: {stopwatch.ElapsedMilliseconds} ms");

        stopwatch.Start();
        var decryptedMessage = rc5.DecryptCBCPAD(encryptedMessage);
        stopwatch.Stop();

        Console.WriteLine($"RC5 Decrypted Message: {Encoding.UTF8.GetString(decryptedMessage)}");
        Console.WriteLine($"RC5 Decryption Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}