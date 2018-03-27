namespace WebService.Services.Randomizer
{
    public interface IRandomizer
    {
        char[] Chars { get; }

        double NextDouble();

        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);

        byte[] Next(byte[] buffer);

        string NextString(int length);

        char NextChar();
    }
}