using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IranSub.Parsers
{
    public class SubParser 
    {
        public readonly Dictionary<SubtitlesFormat, ISubtitlesParser> _subFormatToParser = new Dictionary<SubtitlesFormat, ISubtitlesParser>
            {
                {SubtitlesFormat.SubRipFormat, new SrtParser()},
                {SubtitlesFormat.MicroDvdFormat, new MicroDvdParser()},
                {SubtitlesFormat.SubStationAlphaFormat, new SsaParser()},
                {SubtitlesFormat.TtmlFormat, new TtmlParser()},
                {SubtitlesFormat.WebVttFormat, new VttParser()},
                {SubtitlesFormat.AssFormat, new AssParser()},
                {SubtitlesFormat.UsfFormat, new UsfParser()},
                {SubtitlesFormat.DVDSubtitleFormat, new DvdSubtitleParser()},
                {SubtitlesFormat.SoftNiColonSubFormat, new SoftNiColonSubParser()},
                {SubtitlesFormat.SoftNiSubFormat, new SoftNiSubParser()},
                {SubtitlesFormat.SonyDVDArchitectFormat, new SonyDVDArchitectParser()},
                {SubtitlesFormat.SonyDVDArchitectExplicitDurationFormat, new SonyDVDArchitectExplicitDurationParser()},
                {SubtitlesFormat.SonyDVDArchitectTabsFormat, new SonyDVDArchitectTabsParser()},
                {SubtitlesFormat.SonyDVDArchitectWithLineNumbersFormat, new SonyDVDArchitectWithLineNumbersParser()},
                {SubtitlesFormat.SubViewer10Format, new SubViewer10Parser()},
                {SubtitlesFormat.SubViewer20Format, new SubViewer20Parser()},
                {SubtitlesFormat.JsonFormat, new JSONParser()},
                {SubtitlesFormat.Json2Format, new JSON2Parser()},
                {SubtitlesFormat.Json3Format, new JSON3Parser()},
                {SubtitlesFormat.Json4Format, new JSON4Parser()},
                {SubtitlesFormat.SamiFormat, new SamiParser()}
            };

        
        public SubParser(){}


        
        public SubtitlesFormat GetMostLikelyFormat(string fileName, Stream stream, Encoding encoding)
        {
            string reshte = "";
            stream.Position = 0;
            using (StreamReader st = new StreamReader(stream, encoding, true))
            {
                reshte = st.ReadToEnd();
            }
            if (!string.IsNullOrEmpty(reshte))
            {
                if (IsDvdSubtitle(reshte))
                    return SubtitlesFormat.DVDSubtitleFormat;

                else if (IsSoftNicolonSub(reshte))
                    return SubtitlesFormat.SoftNiColonSubFormat;

                else if (IsSoftNiSub(reshte))
                    return SubtitlesFormat.SoftNiSubFormat;

                else if (IsSonyDVDArchitect(reshte))
                    return SubtitlesFormat.SonyDVDArchitectFormat;

                else if (IsSonyDVDArchitectExplicitDuration(reshte))
                    return SubtitlesFormat.SonyDVDArchitectExplicitDurationFormat;

                else if (IsSonyDVDArchitectTabs(reshte))
                    return SubtitlesFormat.SonyDVDArchitectTabsFormat;

                else if (IsSonyDVDArchitectWithLineNumbers(reshte))
                    return SubtitlesFormat.SonyDVDArchitectWithLineNumbersFormat;

                else if (IsMicroDvd(reshte))
                    return SubtitlesFormat.MicroDvdFormat;

                else if (IsSubViewer10(reshte))
                    return SubtitlesFormat.SubViewer10Format;

                else if (IsSubViewer20(reshte))
                    return SubtitlesFormat.SubViewer20Format;

                else if (IsSubRip(reshte))
                    return SubtitlesFormat.SubRipFormat;

                else if (IsWebVTT(reshte))
                    return SubtitlesFormat.WebVttFormat;


                else if (IsUniversalSubtitle(reshte))
                    return SubtitlesFormat.UsfFormat;


                else if (IsJSONSubtitle(reshte))
                    return SubtitlesFormat.JsonFormat;
                else if (IsJSON2Subtitle(reshte))
                    return SubtitlesFormat.Json2Format;
                else if (IsJSON3Subtitle(reshte))
                    return SubtitlesFormat.Json3Format;
                else if (IsJSON4Subtitle(reshte))
                    return SubtitlesFormat.Json4Format;
                else if (IsSamiSubtitle(reshte))
                    return SubtitlesFormat.SamiFormat;

            }
            var extension = Path.GetExtension(fileName);

            if (!string.IsNullOrEmpty(extension))
            {
                foreach (var format in SubtitlesFormat.SupportedSubtitlesFormats)
                {
                    if (format.Extension != null && Regex.IsMatch(extension, format.Extension, RegexOptions.IgnoreCase))
                    {
                        return format;
                    }
                }
            }


            return null;
        }

        public bool IsThisFileIsASubtitle(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
                foreach (var format in SubtitlesFormat.SupportedSubtitlesFormats)
                    if (format.Extension != null && Regex.IsMatch(extension, format.Extension, RegexOptions.IgnoreCase))
                    {
                        System.Diagnostics.Debug.WriteLine("Yes it's subtitle file: " + extension);
                        return true;
                    }
            return false;
        }

        
        private bool IsDvdSubtitle(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\{T\ \d+:\d+:\d+:\d+$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSoftNicolonSub(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d\\\d\d:\d\d:\d\d:\d\d$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSoftNiSub(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d\.\d\d\\\d\d:\d\d:\d\d\.\d\d$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSonyDVDArchitect(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d[ ]+-[ ]+\d\d:\d\d:\d\d:\d\d");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSonyDVDArchitectExplicitDuration(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d\.\d\d\d[ \t]+\d\d:\d\d:\d\d\.\d\d\d[ \t]+\d\d:\d\d:\d\d\.\d\d\d[ \t]+");

            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSonyDVDArchitectTabs(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d[ \t]+\d\d:\d\d:\d\d:\d\d[ \t]+");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSonyDVDArchitectWithLineNumbers(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d\d\d  \d\d:\d\d:\d\d:\d\d  \d\d:\d\d:\d\d:\d\d");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSubViewer10(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\[\d\d:\d\d:\d\d\]$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSubViewer20(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d.\d+,\d\d:\d\d:\d\d.\d+$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsMicroDvd(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^\{-?\d+}\{-?\d+}.*$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsSubRip(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+[:,]-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+[:,]-?\d+$");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsWebVTT(string subtitle)
        {
            Regex regexTimeCodes = new Regex(@"^-?\d+:-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+\.-?\d+");
            Regex regexTimeCodesMiddle = new Regex(@"^-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+:-?\d+\.-?\d+");
            Regex regexTimeCodesShort = new Regex(@"^-?\d+:-?\d+\.-?\d+\s*-->\s*-?\d+:-?\d+\.-?\d+");
            string[] lines = subtitle.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in lines)
            {
                if (regexTimeCodes.Match(line).Success || regexTimeCodesMiddle.Match(line).Success
                    || regexTimeCodesShort.Match(line).Success)
                    return true;
            }

            return false;
        }

        private bool IsUniversalSubtitle(string subtitle)
        {
            if (subtitle.Contains("<USFSubtitles"))
                return true;
            else
                return false;
        }

        private bool IsJSONSubtitle(string subtitle)
        {
            if (subtitle.Contains("{\"start\":"))
                return true;
            else
                return false;
        }

        private bool IsJSON2Subtitle(string subtitle)
        {
            if (subtitle.Contains("{\"startMillis\":"))
                return true;
            else
                return false;
        }

        private bool IsJSON3Subtitle(string subtitle)
        {
            if (subtitle.Contains("{\"duration\":"))
                return true;
            else
                return false;
        }

        private bool IsJSON4Subtitle(string subtitle)
        {
            if (subtitle.Contains("\"metadata\":"))
                return true;
            else
                return false;
        }

        private bool IsSamiSubtitle(string subtitle)
        {
            if (subtitle.Contains("<sync"))
                return true;
            else return false;
        }







        public SubZir ParseStream(Stream stream)
        {
            return ParseStream(stream, Encoding.UTF8);
        }

        public SubZir ParseStream(Stream stream, Encoding encoding, SubtitlesFormat subFormat = null)
        {
            var dictionary = subFormat != null ?
                _subFormatToParser
                .OrderBy(dic => Math.Abs(string.Compare(dic.Key.Name, subFormat.Name, StringComparison.Ordinal)))
                .ToDictionary(entry => entry.Key, entry => entry.Value):
                _subFormatToParser;

            return ParseStream(stream, encoding, dictionary);
        }


        public SubZir ParseStream(Stream stream, Encoding encoding, Dictionary<SubtitlesFormat, ISubtitlesParser> subFormatDictionary)
        {

            if (!stream.CanRead)
            {
                throw new ArgumentException("Cannot parse a non-readable stream");
            }


            var seekableStream = stream;
            if (!stream.CanSeek)
            {
                seekableStream = StreamHelpers.CopyStream(stream);
                seekableStream.Seek(0, SeekOrigin.Begin);
            }


            subFormatDictionary = subFormatDictionary ?? _subFormatToParser;

            foreach (var subtitlesParser in subFormatDictionary)
            {
                try
                {
                    var parser = subtitlesParser.Value;
                    var items = parser.ParseStream(seekableStream, encoding);


                    if (items.Any())
                    {
                        return items;
                    }
                    else
                    {
                        throw new FormatException(string.Format("Failed to parse as {0}", subtitlesParser.Key));
                    }
                }
                catch(Exception ex)
                {

                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }


            var firstCharsOfFile = LogFirstCharactersOfStream(stream, 500, encoding);
            var message = string.Format("All the subtitles parsers failed to parse the following stream:{0}", firstCharsOfFile);
            throw new ArgumentException(message);
        }
        

        private string LogFirstCharactersOfStream(Stream stream, int nbOfCharactersToPrint, Encoding encoding)
        {
            var message = "";

            if (stream.CanRead)
            {
                if (stream.CanSeek)
                {
                    stream.Position = 0;
                }

                var reader = new StreamReader(stream, encoding, true);

                var buffer = new char[nbOfCharactersToPrint];
                reader.ReadBlock(buffer, 0, nbOfCharactersToPrint);
                message = string.Format("Parsing of subtitle stream failed. Beginning of sub stream:\n{0}",
                                        string.Join("", buffer));
            }
            else
            {
                message = string.Format("Tried to log the first {0} characters of a closed stream",
                                        nbOfCharactersToPrint);
            }
            return message;
        }

    }
}