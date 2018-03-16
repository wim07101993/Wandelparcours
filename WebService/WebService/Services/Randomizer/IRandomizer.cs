namespace WebService.Services.Randomizer
{
    public interface IRandomizer
    {
        double NextDouble();

        int Next();
        int Next(int maxValue);
        int Next(int minValue, int maxValue);

        byte[] Next(byte[] buffer);

        string NextString(int length);
    }
}