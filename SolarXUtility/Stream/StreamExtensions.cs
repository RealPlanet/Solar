using System.IO;

namespace SolarXUtility
{
    public static class StreamExtensions
    {
        public static string ReadNullTerminatedString(this BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }
    }
}
