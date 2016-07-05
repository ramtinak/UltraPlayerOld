using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class DvdSubtitleParser : ISubtitlesParser
    {
        public DvdSubtitleParser() { }
        SubZir items = new SubZir();

        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            

            Regex regexTimeCodes = new Regex(@"^\{T\ \d+:\d+:\d+:\d+$");
            StreamReader st = new StreamReader(stream, encoding, true);
            string reshte = st.ReadToEnd();

            List<string> lines = new List<string>(reshte.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
            //{T 00:00:14:01
            //تقديم به تمام پارسي زبانان جهان
            //}
            //{T 00:00:20:97

            //}
            items.Clear();
            int _errorCount = 0;
            bool textOn = false;
            string text = string.Empty;
            double start = 0;
            double end = 0;
            foreach (string line in lines)
            {
                if (textOn)
                {
                    if (line.Trim() == "}")
                    {
                        SubtitleItem s = new SubtitleItem();
                        s.StartTime = int.Parse(start.ToString());
                        s.EndTime = int.Parse(end.ToString());
                        s.Lines = new List<string>() { text };
                        items.Add(s);

                        text = string.Empty;
                        start = 0;
                        end = 0;
                        textOn = false;
                    }
                    else
                    {
                        if (text.Length == 0)
                            text = line;
                        else
                            text += Environment.NewLine + line;
                    }
                }
                else
                {
                    if (regexTimeCodes.Match(line).Success)
                    {
                        try
                        {
                            textOn = true;
                            string[] arr = line.Substring(3).Trim().Split(':');
                            if (arr.Length == 4)
                            {
                                int hours = int.Parse(arr[0]);
                                int minutes = int.Parse(arr[1]);
                                int seconds = int.Parse(arr[2]);
                                int milliseconds = int.Parse(arr[3]);
                                if (arr[3].Length == 2)
                                    milliseconds *= 10;
                                TimeSpan t = new TimeSpan(0, hours, minutes, seconds, milliseconds);
                                start = t.TotalMilliseconds;
                            }
                        }
                        catch
                        {
                            textOn = false;
                            _errorCount++;
                        }
                    }
                }
            }

            int index = 1;
            foreach (SubtitleItem s in items)
            {
                foreach (string line in s.Lines)
                {
                    SubtitleItem next = GetParagraphOrDefault(index);
                    if (next != null)
                    {
                        s.EndTime = next.StartTime;
                    }
                }
                index++;
            }

            int counter = 0;
            SubZir customList = new SubZir();
            foreach (var item in items)
            {
                if (counter % 2 == 0)
                    customList.Add(item);
                counter++;
            }
            items.Clear();
            items.AddRange(customList);

            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid 'DVD Subtitle' format");
            }
        }
        
        public SubtitleItem GetParagraphOrDefault(int index)
        {
            if (items == null || items.Count <= index || index < 0)
                return null;

            return items[index];
        }
    }
}
