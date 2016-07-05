using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class SonyDVDArchitectTabsParser : ISubtitlesParser
    {
        public SonyDVDArchitectTabsParser() { }


        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            SubZir items = new SubZir();
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d:\d\d[ \t]+\d\d:\d\d:\d\d:\d\d[ \t]+");
            StreamReader st = new StreamReader(stream, encoding, true);
            string reshte = st.ReadToEnd();
            List<string> lines = new List<string>(reshte.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            var sb = new StringBuilder();
            SubtitleItem lastParagraph = null;
            int _errorCount = 0;
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                bool success = false;
                if (line.Length > 26 && line.IndexOf(':') == 2)
                {
                    var match = regexTimeCodes.Match(line);
                    if (match.Success)
                    {
                        string s = line.Substring(0, match.Length);
                        s = s.Replace("\t", ":");
                        s = s.Replace(" ", string.Empty);
                        s = s.Trim().TrimEnd(':').TrimEnd();
                        string[] parts = s.Split(':');
                        if (parts.Length == 8)
                        {
                            int hours = int.Parse(parts[0]);
                            int minutes = int.Parse(parts[1]);
                            int seconds = int.Parse(parts[2]);
                            int milliseconds = int.Parse(parts[3]) * 10;
                            int start = int.Parse(new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalMilliseconds.ToString());

                            hours = int.Parse(parts[4]);
                            minutes = int.Parse(parts[5]);
                            seconds = int.Parse(parts[6]);
                            milliseconds = int.Parse(parts[7]) * 10;
                            int end = int.Parse(new TimeSpan(0, hours, minutes, seconds, milliseconds).TotalMilliseconds.ToString());

                            string text = line.Substring(match.Length).TrimStart();
                            text = text.Replace("|", Environment.NewLine);

                            lastParagraph = new SubtitleItem();
                            lastParagraph.StartTime = start;
                            lastParagraph.EndTime = end;
                            lastParagraph.Lines = new List<string>() { text };
                            items.Add(lastParagraph);
                            success = true;
                        }
                    }
                }
                if (!success)
                    _errorCount++;
            }
            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid 'Sony DVDArchitect Tabs' Subtitle format");
            }
        }

    }
}
