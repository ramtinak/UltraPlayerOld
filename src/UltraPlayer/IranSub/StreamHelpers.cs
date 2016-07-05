using System.IO;

namespace IranSub
{
    static class StreamHelpers
    {
        public static Stream CopyStream(Stream inputStream)
        {
            var outputStream = new MemoryStream();
            int count;
            do
            {
                var buf = new byte[1024];
                count = inputStream.Read(buf, 0, 1024);
                outputStream.Write(buf, 0, count);
            } while (inputStream.CanRead && count > 0);
            outputStream.ToArray();

            return outputStream;
        }
    }
}
