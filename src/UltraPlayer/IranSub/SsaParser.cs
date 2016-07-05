using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace IranSub.Parsers
{

    public class SsaParser : ISubtitlesParser
    {

        private const string EventLine = "[Events]";
        private const char Separator = ',';

        private const string StartColumn = "Start";
        private const string EndColumn = "End";
        private const string TextColumn = "Text";
        
        public SubZir ParseStream(Stream ssaStream, Encoding encoding)
        {

            if (!ssaStream.CanRead || !ssaStream.CanSeek)
            {
                var message = string.Format("Stream must be seekable and readable in a subtitles parser. " +
                                   "Operation interrupted; isSeekable: {0} - isReadable: {1}",
                                   ssaStream.CanSeek, ssaStream.CanSeek);
                throw new ArgumentException(message);
            }


            ssaStream.Position = 0;

            var reader = new StreamReader(ssaStream, encoding, true);

            var line = reader.ReadLine();
            var lineNumber = 1;

            while (line != null && line != EventLine)
            {
                line = reader.ReadLine();
                lineNumber++;
            }

            if (line != null)
            {

                var headerLine = reader.ReadLine();
                if (!string.IsNullOrEmpty(headerLine))
                {
                    var columnHeaders = headerLine.Split(Separator).Select(head => head.Trim()).ToList();

                    var startIndexColumn = columnHeaders.IndexOf(StartColumn);
                    var endIndexColumn = columnHeaders.IndexOf(EndColumn);
                    var textIndexColumn = columnHeaders.IndexOf(TextColumn);

                    if (startIndexColumn > 0 && endIndexColumn > 0 && textIndexColumn > 0)
                    {
                        var items = new SubZir();

                        line = reader.ReadLine();
                        while (line != null)
                        {
                            if(!string.IsNullOrEmpty(line))
                            {
                                var columns = line.Split(Separator);
                                var startText = columns[startIndexColumn];
                                var endText = columns[endIndexColumn];


                                var textLine = string.Join(",", columns.Skip(textIndexColumn));

                                var start = ParseSsaTimecode(startText);
                                var end = ParseSsaTimecode(endText);


                                if (start > 0 && end > 0 && !string.IsNullOrEmpty(textLine))
                                {

                                    textLine = textLine.Replace(@"{\c&H&}", "");
                                    textLine = textLine.Replace(@"{\c}", "");
                                    textLine = textLine.Replace(@"\\N", Environment.NewLine);
                                    textLine = textLine.Replace("{\\i1}", "<i>");
                                    textLine = textLine.Replace("{\\i0}", "</i>");
                                    textLine = textLine.Replace("{\\i}", "</i>");
                                    textLine = textLine.Replace("{\\b1}", "<b>'");
                                    textLine = textLine.Replace("{\\b0}", "</b>");
                                    textLine = textLine.Replace("{\\b}", "</b>");
                                    textLine = textLine.Replace("{\\u1}", "<u>");
                                    textLine = textLine.Replace("{\\u0}", "</u>");
                                    textLine = textLine.Replace("{\\u}", "</u>");
                                    textLine = textLine.Replace(@"{\b0\i0}", "");
                                    textLine = textLine.Replace(@"{\b1\i1}", "");
                                    textLine = textLine.Replace(@"{\i0\b0}", "");
                                    textLine = textLine.Replace(@"{\i1\b1}", "");

                                    var item = new SubtitleItem()
                                                            {
                                                                StartTime = start,
                                                                EndTime = end,
                                                                Lines = new List<string>() { textLine }
                                                            };
                                    items.Add(item);
                                }
                            }
                            line = reader.ReadLine();
                        }

                        if (items.Any())
                        {
                            return items;
                        }
                        else
                        {
                            throw new ArgumentException("Stream is not in a valid Ssa format");
                        }
                    }
                    else
                    {
                        var message = string.Format("Couldn't find all the necessary columns " +
                                                    "headers ({0}, {1}, {2}) in header line {3}",
                                                    StartColumn, EndColumn, TextColumn, headerLine);
                        throw new ArgumentException(message);
                    }
                }
                else
                {
                    var message = string.Format("The header line after the line '{0}' was null -> " +
                                                "no need to continue parsing", line);
                    throw new ArgumentException(message);
                }
            }
            else
            {
                var message = string.Format("We reached line '{0}' with line number #{1} without finding to " +
                                            "Event section ({2})", line, lineNumber, EventLine);
                throw new ArgumentException(message);
            }
        }

        private int ParseSsaTimecode(string s)
        {
            TimeSpan result;

            if (TimeSpan.TryParse(s, out result))
            {
                var nbOfMs = (int)result.TotalMilliseconds;
                return nbOfMs;
            }
            else
            {
                return -1;
            }
        }
    }
}
