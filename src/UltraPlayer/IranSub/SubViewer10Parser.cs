using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class SubViewer10Parser : ISubtitlesParser
    {
        private enum ExpectingLine
        {
            TimeStart,
            Text,
            TimeEnd,
        }
        public SubViewer10Parser() { }

        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            SubZir items = new SubZir();
            Regex regexTimeCodes = new Regex(@"^\[\d\d:\d\d:\d\d\]$");
            StreamReader st = new StreamReader(stream, encoding, true);
            string reshte = st.ReadToEnd();
            List<string> lines = new List<string>(reshte.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            var sb = new StringBuilder();
            SubtitleItem paragraph = new SubtitleItem();

            ExpectingLine expecting = ExpectingLine.TimeStart;

            int _errorCount = 0;
            foreach (string line in lines)
            {
                if (line.StartsWith("[") && regexTimeCodes.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':', ']', '[', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3)
                    {
                        try
                        {
                            int startHours = int.Parse(parts[0]);
                            int startMinutes = int.Parse(parts[1]);
                            int startSeconds = int.Parse(parts[2]);
                            var tc = int.Parse(new TimeSpan(0, startHours, startMinutes, startSeconds, 0).TotalMilliseconds.ToString());
                            if (expecting == ExpectingLine.TimeStart)
                            {
                                paragraph = new SubtitleItem();
                                paragraph.StartTime = tc;
                                expecting = ExpectingLine.Text;
                            }
                            else if (expecting == ExpectingLine.TimeEnd)
                            {
                                paragraph.EndTime = tc;
                                expecting = ExpectingLine.TimeStart;
                                items.Add(paragraph);
                                paragraph = new SubtitleItem();
                            }
                        }
                        catch
                        {
                            _errorCount++;
                            expecting = ExpectingLine.TimeStart;
                        }
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string text = line.Replace("|", Environment.NewLine);
                            paragraph.Lines = new List<string>() { text };
                            expecting = ExpectingLine.TimeEnd;
                        }
                    }
                }
            }

            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid 'SubViewer 1.0' Subtitle format");
            }
        }

    }
}
