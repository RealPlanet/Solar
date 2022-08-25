using System.IO;
using System.Text;

namespace SolarXUtility
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads a null terminated string from a binary reader.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ReadNullTerminatedString(this BinaryReader stream)
        {
            StringBuilder builder = new StringBuilder();

            char ch;
            while ((ch = stream.ReadChar()) != '\0')
                builder.Append(ch);

            return builder.ToString();
        }
    }
}
