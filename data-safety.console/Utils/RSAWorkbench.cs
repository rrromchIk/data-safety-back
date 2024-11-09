using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace data_safet.console.Utils;

public class RSAWorkbench
{
    public static byte[] EncryptMessage(string message, string publicKey)
    {
        using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048);
        rsa.FromXmlString(publicKey);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        return rsa.Encrypt(messageBytes, false); // false for PKCS#1 v1.5 padding
    }

    public static string DecryptMessage(byte[] encryptedMessage, string privateKey)
    {
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.FromXmlString(privateKey); // Import the private key
            byte[] decryptedBytes = rsa.Decrypt(encryptedMessage, false); // false for PKCS#1 v1.5 padding
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }

    public static void MeasureTime(string message)
    {
        var rsa = new RSACryptoServiceProvider(2048);
        var publicKey = rsa.ToXmlString(false); 
        var privateKey = rsa.ToXmlString(true);  
        
        var stopwatch = new Stopwatch();
        Console.WriteLine("RSA using 2048 key.");
        
        stopwatch.Start();
        var encryptedMessage = EncryptMessage(message, publicKey);
        stopwatch.Stop();
        
        Console.WriteLine($"RSA Encrypted Message: {Encoding.UTF8.GetString(encryptedMessage)}");
        Console.WriteLine($"RSA Encryption Time: {stopwatch.ElapsedMilliseconds} ms");
        
        stopwatch.Start();
        var decryptedMessage = DecryptMessage(encryptedMessage, privateKey);
        stopwatch.Stop();
        
        Console.WriteLine($"RSA Decrypted Message: {decryptedMessage}");
        Console.WriteLine($"RSA Decryption Time: {stopwatch.ElapsedMilliseconds} ms");
    }
}