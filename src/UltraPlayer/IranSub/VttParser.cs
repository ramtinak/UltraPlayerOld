using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IranSub.Parsers
{
    public class VttParser : ISubtitlesParser
    {

        private readonly string[] _delimiters = new string[] { "-->", "- >", "->" };

        
        public VttParser() { }
        public SubZir ParseStream(Stream vttStream, Encoding encoding)
        {
            if (!vttStream.CanRead || !vttStream.CanSeek)
            {
                var message = string.Format("Stream must be seekable and readable in a subtitles parser. " +
                                   "Operation interrupted; isSeekable: {0} - isReadable: {1}",
                                   vttStream.CanSeek, vttStream.CanSeek);
                throw new ArgumentException(message);
            }

            vttStream.Position = 0;

            var reader = new StreamReader(vttStream, encoding, true);

            var items = new SubZir();
            var vttSubParts = GetVttSubTitleParts(reader).ToList();
            if (vttSubParts.Any())
            {
                foreach (var vttSubPart in vttSubParts)
                {
                    var lines =
                        vttSubPart.Split(new string[] { Environment.NewLine }, StringSplitOptions.None)
                                  .Select(s => s.Trim())
                                  .Where(l => !string.IsNullOrEmpty(l))
                                  .ToList();

                    var item = new SubtitleItem();
                    foreach (var line in lines)
                    {
                        if (item.StartTime == 0 && item.EndTime == 0)
                        {
                            int startTc;
                            int endTc;
                            var success = TryParseTimecodeLine(line, out startTc, out endTc);
                            if (success)
                            {
                                item.StartTime = startTc;
                                item.EndTime = endTc;
                            }
                        }
                        else
                        {
                            item.Lines.Add(line);
                        }
                    }

                    if ((item.StartTime != 0 || item.EndTime != 0) && item.Lines.Any())
                    {
                        items.Add(item);
                    }
                }

                if (items.Any())
                {
                    return items;
                }
                else
                {
                    throw new ArgumentException("Stream is not in a valid VTT format");
                }
            }
            else
            {
                throw new FormatException("Parsing as VTT returned no VTT part.");
            }
        }

        private IEnumerable<string> GetVttSubTitleParts(TextReader reader)
        {
            string line;
            var sb = new StringBuilder();

            while ((line = reader.ReadLine()) != null)
            {
                if (string.IsNullOrEmpty(line.Trim()))
                {
                    var res = sb.ToString().TrimEnd();
                    if (!string.IsNullOrEmpty(res))
                    {
                        yield return res;
                    }
                    sb = new StringBuilder();
                }
                else
                {
                    sb.AppendLine(line);
                }
            }

            if (sb.Length > 0)
            {
                yield return sb.ToString();
            }
        }

        private bool TryParseTimecodeLine(string line, out int startTc, out int endTc)
        {
            var parts = line.Split(_delimiters, StringSplitOptions.None);
            if (parts.Length != 2)
            {
                startTc = -1;
                endTc = -1;
                return false;
            }
            else
            {
                startTc = ParseVttTimecode(parts[0]);
                endTc = ParseVttTimecode(parts[1]);
                return true;
            }
        }

        private int ParseVttTimecode(string s)
        {
            string timeString = string.Empty;
            var match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+[,\\.][0-9]+");
            if (match.Success)
            {
                timeString = match.Value;
            }
            else
            {
                match = Regex.Match(s, "[0-9]+:[0-9]+[,\\.][0-9]+");
                if (match.Success)
                {
                    timeString = "00:" + match.Value;
                }
            }

            if (!string.IsNullOrEmpty(timeString))
            {
                timeString = timeString.Replace(',', '.');
                TimeSpan result;
                if (TimeSpan.TryParse(timeString, out result))
                {
                    var nbOfMs = (int)result.TotalMilliseconds;
                    return nbOfMs;
                } 
            }
            
            return -1;
        }
    }
}