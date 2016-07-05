using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class SubViewer20Parser : ISubtitlesParser
    {
        private enum ExpectingLine
        {
            TimeCodes,
            Text
        }
        public SubViewer20Parser() { }
        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            SubZir items = new SubZir();
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d.\d+,\d\d:\d\d:\d\d.\d+$");
            StreamReader st = new StreamReader(stream, encoding, true);
            string reshte = st.ReadToEnd();
            List<string> lines = new List<string>(reshte.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            var sb = new StringBuilder();
            SubtitleItem paragraph = new SubtitleItem();
            ExpectingLine expecting = ExpectingLine.TimeCodes;


            foreach (string line in lines)
            {
                if (regexTimeCodes.IsMatch(line))
                {
                    string[] parts = line.Split(new[] { ':', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 8)
                    {
                        try
                        {
                            int startHours = int.Parse(parts[0]);
                            int startMinutes = int.Parse(parts[1]);
                            int startSeconds = int.Parse(parts[2]);
                            int startMilliseconds = int.Parse(parts[3]);
                            int endHours = int.Parse(parts[4]);
                            int endMinutes = int.Parse(parts[5]);
                            int endSeconds = int.Parse(parts[6]);
                            int endMilliseconds = int.Parse(parts[7]);
                            paragraph.StartTime = int.Parse(new TimeSpan(0, startHours, startMinutes, startSeconds, startMilliseconds).TotalMilliseconds.ToString());
                            paragraph.EndTime = int.Parse(new TimeSpan(0, endHours, endMinutes, endSeconds, endMilliseconds).TotalMilliseconds.ToString());
                            expecting = ExpectingLine.Text;
                        }
                        catch
                        {
                            expecting = ExpectingLine.TimeCodes;
                        }
                    }
                }
                else
                {
                    if (expecting == ExpectingLine.Text)
                    {
                        if (line.Length > 0)
                        {
                            string text = line.Replace("[br]", Environment.NewLine);
                            text = text.Replace("{\\i1}", "<i>");
                            text = text.Replace("{\\i0}", "</i>");
                            text = text.Replace("{\\i}", "</i>");
                            text = text.Replace("{\\b1}", "<b>'");
                            text = text.Replace("{\\b0}", "</b>");
                            text = text.Replace("{\\b}", "</b>");
                            text = text.Replace("{\\u1}", "<u>");
                            text = text.Replace("{\\u0}", "</u>");
                            text = text.Replace("{\\u}", "</u>");
                            //<font color="">... آنچه گذشت</font>[br]{\i1}Ramtin{\b1}'Jokar{\b0}{\i0}[br]{\i1}SalAM{\i0}
                            text = text.Replace("[br]", Environment.NewLine);


                            paragraph.Lines = new List<string>() { text };
                            items.Add(paragraph);
                            paragraph = new SubtitleItem();
                            expecting = ExpectingLine.TimeCodes;
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
                throw new ArgumentException("Stream is not in a valid 'SubViewer 2.0' Subtitle format");
            }
        }

    }
}
