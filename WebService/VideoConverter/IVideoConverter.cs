using System.IO;

namespace VideoConverter
{
    public interface IVideoConverter
    {
        Stream ConvertToWebm(Stream input, string extension);
    }
}