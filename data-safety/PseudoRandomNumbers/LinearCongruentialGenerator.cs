using System.Numerics;
using System.Text;

namespace data_safety.PseudoRandomNumbers;

public class LinearCongruentialGenerator
{
    private readonly int a;
    private readonly int c;
    private readonly BigInteger m;
    private readonly int sequenceLength;
    private readonly BigInteger X0;
    private BigInteger _Xn;

    public LinearCongruentialGenerator(int a, int X0, int c, BigInteger m, int sequenceLength = 0)
    {
        this.a = a;
        this.X0 = X0;
        this.c = c;
        this.m = m;
        this.sequenceLength = sequenceLength;
        _Xn = X0;
    }
    
    public async Task<LinearCongruentialGeneratorResult> GenerateRandomNumbersAndWriteToFile()
    {
        BigInteger Xn = X0;
        BigInteger prev;
        BigInteger X1 = -1;

        var randomNumbersSequence = new List<string>() {X0.ToString()};
        int period = 0;
        
        await using var writer = new StreamWriter("pseudo-random-numbers.txt", false);
        var randomNumbersSequenceToBeWritten = new StringBuilder();  // Use StringBuilder to accumulate results
        const int batchSize = 1_000_000;
        randomNumbersSequenceToBeWritten.Append(X0.ToString());
        
        while (true)
        {
            prev = Xn;
            Xn = (a * Xn + c) % m;
            period++;
            
            // Store the second number if not already stored
            if (period == 1)
            {
                X1 = Xn;
            }
            
            if (Xn == prev || Xn == X0 || (period > 1 && Xn == X1))
            {
                break;  // Break the loop if a cycle is detected
            }
            
            
            if (randomNumbersSequence.Count < sequenceLength)
            {
                randomNumbersSequence.Add(Xn.ToString());
            }
            
            randomNumbersSequenceToBeWritten.Append(", " + Xn);
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
    
    public BigInteger NextNumber()
    {
        BigInteger currentNumber = this._Xn;
        _Xn = SequenceFormula(currentNumber);
        return currentNumber;
    }
    
    public void Reset() => _Xn = X0;

    private BigInteger SequenceFormula(BigInteger Xn) => (a * Xn + c) % m;
}
