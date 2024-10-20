using System.Text;
using data_safety.MD5;

namespace data_safety.Test;

public class Md5UtilTests
{
    [Theory]
    [InlineData("", "D41D8CD98F00B204E9800998ECF8427E")]
    [InlineData("a", "0CC175B9C0F1B6A831C399E269772661")]
    [InlineData("abc", "900150983CD24FB0D6963F7D28E17F72")]
    [InlineData("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890",
            "57EDF4A22BE3C955AC49DA2E2107B67A")]
    public void HashesString(string input, string expectedHash)
    {
        var hasher = new Md5Util();
        hasher.ComputeHash(input);

        Assert.Equal(hasher.HashAsString.ToUpper(), expectedHash);
    }

    [Theory]
    [InlineData("", "D41D8CD98F00B204E9800998ECF8427E")]
    [InlineData("a", "0CC175B9C0F1B6A831C399E269772661")]
    [InlineData("abc", "900150983CD24FB0D6963F7D28E17F72")]
    [InlineData("message digest", "F96B697D7CB7938D525A2F31AAF161D0")]
    [InlineData("abcdefghijklmnopqrstuvwxyz", "C3FCD3D76192E4007DFB496CCA67E13B")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", "D174AB98D277D9F5A5611C2C9F419D9F")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890",
            "57EDF4A22BE3C955AC49DA2E2107B67A")]
    public void HashesBytes(string input, string expectedHash)
    {
        var customHasher = new Md5Util();
        customHasher.ComputeHash(Encoding.UTF8.GetBytes(input));

        Assert.Equal(customHasher.HashAsString.ToUpper(), expectedHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abc")]
    [InlineData("message digest")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    [InlineData(
            "SYBi4V6eGlUlSLiwvDHCH7vGylhzX6s1ZcL4lCYk2eyNUIWQ9J3WWCA9CGfRMPHqtELFPxn8erNCAiHwTY5aHHhj53gPoYSKxqlXSHTy0wv28cZWvQZPy2QAKH1X7gUnuC9BjhBahfNNBhbRdETzFMlk93CU99fRohu9ZZnk2mTYesRUfwfdiImSYDa3XRYC2bNw6FNiOU2VhBAFgq9J6BOZ0g4PbRgeUy2rXZeEV8lqH1Wdvx5NXL9BZDevOyUuJQY2t4AvYTSusYIFEgyHOORUuV3eI79VwjCsxYSXvksPJNwhf26NvSwgW8QxuW3VvwQ8GhuSL4Qu4EtVN1O5hNWuaD0E88TzzrvdhH0yUobv1okztRS9KeJfnWtYz2NdB3iNReJug5J8GrJP9viKO09BIuBHtpzbvt1sBxVk3Pe4i2NiWWvCn7P3")]
    public void HashesSameAsCryptoImpl(string input)
    {
        var customHasher = new Md5Util();
        var myHash = customHasher.ComputeHash(Encoding.UTF8.GetBytes(input));
        var byteHashUsingAPI = ComputeHashUsingMD5APIToByte(input);
        var stringHashUsingAPI = ComputeHashUsingMD5APIToString(input);

        Assert.Equal(stringHashUsingAPI.ToUpper(), customHasher.HashAsString.ToUpper());
        Assert.True(byteHashUsingAPI.SequenceEqual(myHash.ToByteArray()));
    }

    [Theory]
    [InlineData("testFile1.txt")]
    [InlineData("testFile2.txt")]
    [InlineData("testFile3")]
    public void HashesFileSameAsCryptoImpl(string filePath)
    {
        var message = File.ReadAllBytes(filePath);
        var customHasher = new Md5Util();
        customHasher.ComputeFileHashAsync(filePath).Wait();

        Assert.Equal(ComputeHashUsingMD5APIToString(message).ToUpper(), customHasher.HashAsString.ToUpper());
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("abc")]
    [InlineData("message digest")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    public void ComparesMessageDigests(string input)
    {
        var hasher = new Md5Util();

        var firstMd = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));
        var secondMd = hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

        Assert.True(firstMd.Equals(secondMd));
    }

    public static byte[] ComputeHashUsingMD5APIToByte(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();

        return md5.ComputeHash(Encoding.UTF8.GetBytes(input));
    }

    public static string ComputeHashUsingMD5APIToString(string input)
    {
        return ComputeHashUsingMD5APIToString(Encoding.UTF8.GetBytes(input));
    }

    public static string ComputeHashUsingMD5APIToString(byte[] inputBytes)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        byte[] hashedBytes = md5.ComputeHash(inputBytes);

        var sb = new StringBuilder();
        for (int i = 0; i < hashedBytes.Length; i++)
        {
            sb.Append(hashedBytes[i].ToString("X2"));
        }

        return sb.ToString();
    }
}