using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IranSub.Parsers
{
    public class MicroDvdParser : ISubtitlesParser
    {

        private readonly float defaultFrameRate = 25;
        private readonly char[] _lineSeparators = {'|'};

        
        public MicroDvdParser(){}
        public MicroDvdParser(float defaultFrameRate)
        {
            this.defaultFrameRate = defaultFrameRate;
        }
        
        public SubZir ParseStream(Stream subStream, Encoding encoding)
        {

            if (!subStream.CanRead || !subStream.CanSeek)
            {
                var message = string.Format("Stream must be seekable and readable in a subtitles parser. " +
                                   "Operation interrupted; isSeekable: {0} - isReadable: {1}", 
                                   subStream.CanSeek, subStream.CanSeek);
                throw new ArgumentException(message);
            }


            subStream.Position = 0;
            var reader = new StreamReader(subStream, encoding, true);

            var items = new SubZir();
            var line = reader.ReadLine();

            while (line != null && !IsMicroDvdLine(line))
            {
                line = reader.ReadLine();
            }

            if (line != null)
            {
                float frameRate;

                var firstItem = ParseLine(line, defaultFrameRate);
                if (firstItem.Lines != null && firstItem.Lines.Any())
                {
                    var success = TryExtractFrameRate(firstItem.Lines[0], out frameRate);
                    if (!success)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("Couldn't extract frame rate of sub file with first line {0}. " +
                                          "We use the default frame rate: {1}", line, defaultFrameRate));
                        frameRate = defaultFrameRate;


                        items.Add(firstItem);
                    }
                }
                else
                {
                    frameRate = defaultFrameRate;
                }


                line = reader.ReadLine();
                while (line != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        var item = ParseLine(line, frameRate);
                        items.Add(item); 
                    }
                    line = reader.ReadLine();
                }
            }

            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid MicroDVD format");
            }
        }

        private const string LineRegex = @"^[{\[](-?\d+)[}\]][{\[](-?\d+)[}\]](.*)";

        private bool IsMicroDvdLine(string line)
        {
            return Regex.IsMatch(line, LineRegex);
        }

        private SubtitleItem ParseLine(string line, float frameRate)
        {
            var match = Regex.Match(line, LineRegex);
            if (match.Success && match.Groups.Count > 2)
            {
                var startFrame = match.Groups[1].Value;
                var start = (int)(1000 * double.Parse(startFrame) / frameRate);
                var endTime = match.Groups[2].Value;
                var end = (int)(1000 * double.Parse(endTime) / frameRate);
                var text = match.Groups[match.Groups.Count - 1].Value;
                var lines = text.Split(_lineSeparators);
                //... آنچه گذشت|{y:i}RamtinJokar|{y:i}SalAM
                var nonEmptyLines = lines.Where(l => !string.IsNullOrEmpty(l)).ToList();
                if (nonEmptyLines.Count > 0)
                    for (int i = 0; i < nonEmptyLines.Count;i++ )
                    {
                        nonEmptyLines[i] = nonEmptyLines[i].Replace("|", Environment.NewLine);
                        if (nonEmptyLines[i].Contains("{y:i}"))
                        {
                            nonEmptyLines[i] = nonEmptyLines[i].Replace("{y:i}", "");
                            nonEmptyLines[i] = "<i>" + nonEmptyLines[i] + "</i>";
                        }
                        if (nonEmptyLines[i].Contains("{Y:b}"))
                        {
                            nonEmptyLines[i] = nonEmptyLines[i].Replace("{Y:b}", "");
                            nonEmptyLines[i] = "<b>" + nonEmptyLines[i] + "</b>";
                        }
                    }
                var item = new SubtitleItem
                    {
                        Lines = nonEmptyLines,
                        StartTime = start,
                        EndTime = end
                    };

                return item;
            }
            else
            {
                var message = string.Format("The subtitle file line {0} is " +
                                            "not in the micro dvd format. We stop the process.", line);
                throw new InvalidDataException(message);
            }
        }


        private bool TryExtractFrameRate(string text, out float frameRate)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var success = float.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture,
                                             out frameRate);
                return success;
            }
            else
            {
                frameRate = defaultFrameRate;
                return false;
            }
        }

    }
}