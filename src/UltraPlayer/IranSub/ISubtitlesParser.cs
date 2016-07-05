using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IranSub.Parsers
{
    public interface ISubtitlesParser
    {
        SubZir ParseStream(Stream stream, Encoding encoding);
    }
}