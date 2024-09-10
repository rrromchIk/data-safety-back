using System.Numerics;
using System.Text;

namespace data_safety.Utils;

public class LinearCongruentialGenerator
{
    public async Task<LinearCongruentialGeneratorResult> GenerateRandomNumbersAndWriteToFile(int a,
            int X0,
            int c, 
            BigInteger m, 
            int sequenceLength)
    {
        BigInteger Xn = X0;
        BigInteger prev;
        var randomNumbersSequence = new List<string>() {X0.ToString()};
        int period = 0;
        
        await using var writer = new StreamWriter("result.txt", false);
        var randomNumbersSequenceToBeWritten = new StringBuilder();  // Use StringBuilder to accumulate results
        const int batchSize = 1_000_000;
        randomNumbersSequenceToBeWritten.Append(X0.ToString());
        
        while (true) {
            prev = Xn;
            Xn = (a * Xn + c) % m;
            period++;
            
            if (Xn == prev || Xn == X0)
            {
                break;  // Break the loop if a cycle is detected
            }
            
            if (randomNumbersSequence.Count < sequenceLength)
            {
                randomNumbersSequence.Add(Xn.ToString());
            }
            
            randomNumbersSequenceToBeWritten.Append(", " + Xn.ToString());
            if (period % batchSize == 0)
            {
                await writer.WriteAsync(randomNumbersSequenceToBeWritten.ToString());
                randomNumbersSequenceToBeWritten.Clear();
                await writer.FlushAsync();
            }
        }

        if (randomNumbersSequenceToBeWritten.Length > 0)
        {
            await writer.WriteAsync(randomNumbersSequenceToBeWritten.ToString());
            await writer.FlushAsync();
        }
        
        return new LinearCongruentialGeneratorResult(randomNumbersSequence, period);
    }
}
