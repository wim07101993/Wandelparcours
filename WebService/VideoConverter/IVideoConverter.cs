using System.IO;

namespace VideoConverter
{
    public interface IVideoConverter
    {
        Stream ConvertToMp4(Stream input, string extension);
    }
}