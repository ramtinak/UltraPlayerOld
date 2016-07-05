using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class SoftNiSubParser : ISubtitlesParser
    {
        public SoftNiSubParser() { }
        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            SubZir items = new SubZir();
            //^\d\d:\d\d:\d\d:\d\d\\\d\d:\d\d:\d\d:\d\d$
            Regex regexTimeCodes = new Regex(@"^\d\d:\d\d:\d\d\.\d\d\\\d\d:\d\d:\d\d\.\d\d$");
            StreamReader st = new StreamReader(stream, encoding, true);
            string reshte = st.ReadToEnd();
            List<string> lines = new List<string>(reshte.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));

            var sb = new StringBuilder();
            SubtitleItem p = null;
            int _errorCount = 0;
            foreach (string line in lines)
            {
                string s = line.Trim();
                if (regexTimeCodes.IsMatch(s))
                {

                    var temp = s.Split('\\');
                    if (temp.Length > 1)
                    {
                        string start = temp[0];
                        string end = temp[1];

                        string[] startParts = start.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] endParts = end.Split(new[] { ':', '.' }, StringSplitOptions.RemoveEmptyEntries);
                        if (startParts.Length == 4 && endParts.Length == 4)
                        {
                            try
                            {
                                p = new SubtitleItem();
                                p.StartTime = DecodeTimeCode(startParts);
                                p.EndTime = DecodeTimeCode(endParts);
                                string text = sb.ToString().Trim();

                                Boolean positionTop = false;

                                if (text.StartsWith("}"))
                                {
                                    positionTop = true;

                                    text = text.Remove(0, 1);
                                }

                                text = text.Replace("[", @"<i>");
                                text = text.Replace("]", @"</i>");

                                text = text.Replace("</i>" + Environment.NewLine + "<i>", Environment.NewLine);


                                if (positionTop)
                                    text = "{\\an8}" + text;

                                p.Lines = new List<string>() { text };
                                if (text.Length > 0)
                                    items.Add(p);
                                sb = new StringBuilder();
                            }
                            catch (Exception exception)
                            {
                                _errorCount++;
                                System.Diagnostics.Debug.WriteLine(exception.Message);
                            }
                        }
                    }
                }
                else if (string.IsNullOrWhiteSpace(line) || line.StartsWith("*"))
                {

                }
                else if (p != null)
                {
                    sb.AppendLine(line);
                }
            }


            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid 'SoftNi sub' Subtitle format");
            }
        }


        private static int DecodeTimeCode(string[] parts)
        {
            //00:00:07:12
            string hour = parts[0];
            string minutes = parts[1];
            string seconds = parts[2];
            string frames = parts[3];

            TimeSpan span = new TimeSpan(0, int.Parse(hour), int.Parse(minutes), int.Parse(seconds), int.Parse(frames));
            return int.Parse(span.TotalMilliseconds.ToString());
        }
    }
}

