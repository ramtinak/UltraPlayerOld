using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IranSub.Parsers
{
    public class SamiParser : ISubtitlesParser
    {
        public SubZir ParseStream(Stream stream, Encoding encoding)
        {
            stream.Position = 0;
            var items = new SubZir();
            var reader = new StreamReader(stream, encoding, true);
            string reshte = reader.ReadToEnd();
            SubZir subZir = new SubZir();
            try
            {
                LoadSubtitle(subZir, reshte);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine("SamiParser ex: " + ex.Message); }
            if (subZir.Any())
            {
                return subZir;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid Sami format");
            }
        }

        public void LoadSubtitle(SubZir subtitle, string contents, List<string> liness = null)
        {
            string allInput = contents;
            string allInputLower = allInput.ToLower();
            if (!allInputLower.Contains("<sync "))
                return;

            int styleStart = allInputLower.IndexOf("<style", StringComparison.Ordinal);

            const string syncTag = "<sync start=";
            const string syncTagEnc = "<sync encrypted=\"true\" start=";
            int syncStartPos = allInputLower.IndexOf(syncTag, StringComparison.Ordinal);
            int index = syncStartPos + syncTag.Length;

            int syncStartPosEnc = allInputLower.IndexOf(syncTagEnc, StringComparison.Ordinal);
            if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncStartPos == -1)
            {
                syncStartPos = syncStartPosEnc;
                index = syncStartPosEnc + syncTagEnc.Length;
            }

            var p = new SubtitleItem();
            while (syncStartPos >= 0)
            {
                string millisecAsString = string.Empty;
                while (index < allInput.Length && @"""'0123456789".Contains(allInput[index]))
                {
                    if (allInput[index] != '"' && allInput[index] != '\'')
                        millisecAsString += allInput[index];
                    index++;
                }

                while (index < allInput.Length && allInput[index] != '>')
                    index++;
                if (index < allInput.Length && allInput[index] == '>')
                    index++;

                int syncEndPos = allInputLower.IndexOf(syncTag, index, StringComparison.Ordinal);
                int syncEndPosEnc = allInputLower.IndexOf(syncTagEnc, index, StringComparison.Ordinal);
                if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncEndPos == -1)
                    syncEndPos = syncEndPosEnc;

                string text;
                if (syncEndPos >= 0)
                    text = allInput.Substring(index, syncEndPos - index);
                else
                    text = allInput.Substring(index);

                string textToLower = text.ToLower();
                if (textToLower.Contains(" class="))
                {
                    var className = new StringBuilder();
                    int startClass = textToLower.IndexOf(" class=", StringComparison.Ordinal);
                    int indexClass = startClass + 7;
                    while (indexClass < textToLower.Length)
                    {
                        className.Append(text[indexClass]);
                        indexClass++;
                    }
                }

                if (text.Contains("ID=\"Source\"") || text.Contains("ID=Source"))
                {
                    int sourceIndex = text.IndexOf("ID=\"Source\"", StringComparison.Ordinal);
                    if (sourceIndex < 0)
                        sourceIndex = text.IndexOf("ID=Source", StringComparison.Ordinal);
                    int st = sourceIndex - 1;
                    while (st > 0 && text.Substring(st, 2).ToUpper() != "<P")
                    {
                        st--;
                    }
                    if (st > 0)
                    {
                        text = text.Substring(0, st) + text.Substring(sourceIndex);
                    }
                    int et = st;
                    while (et < text.Length - 5 && text.Substring(et, 3).ToUpper() != "<P>" && text.Substring(et, 4).ToUpper() != "</P>")
                    {
                        et++;
                    }
                    text = text.Substring(0, st) + text.Substring(et);
                }
                text = text.Replace(Environment.NewLine, " ");
                text = text.Replace("  ", " ");

                text = text.TrimEnd();
                text = Regex.Replace(text, @"<br {0,2}/?>", Environment.NewLine, RegexOptions.IgnoreCase);

                while (text.Contains("  "))
                    text = text.Replace("  ", " ");
                text = text.Replace("</BODY>", string.Empty).Replace("</SAMI>", string.Empty).TrimEnd();

                int endSyncPos = text.ToUpper().IndexOf("</SYNC>", StringComparison.Ordinal);
                if (text.IndexOf('>') > 0 && (text.IndexOf('>') < endSyncPos || endSyncPos == -1))
                    text = text.Remove(0, text.IndexOf('>') + 1);
                text = text.TrimEnd();

                if (text.EndsWith("</sync>", StringComparison.OrdinalIgnoreCase))
                    text = text.Substring(0, text.Length - 7).TrimEnd();

                if (text.EndsWith("</p>", StringComparison.Ordinal) || text.EndsWith("</P>", StringComparison.Ordinal))
                    text = text.Substring(0, text.Length - 4).TrimEnd();

                text = text.Replace("&nbsp;", " ").Replace("&NBSP;", " ");

                if (text.Contains("<font color=") && !text.Contains("</font>"))
                    text += "</font>";
                if (text.StartsWith("<FONT COLOR=") && !text.Contains("</font>") && !text.Contains("</FONT>"))
                    text += "</FONT>";

                if (text.Contains('<') && text.Contains('>'))
                {
                    var total = new StringBuilder();
                    var partial = new StringBuilder();
                    bool tagOn = false;
                    for (int i = 0; i < text.Length; i++)
                    {
                        string tmp = text.Substring(i);
                        if (tmp.StartsWith("<") &&
                            (tmp.StartsWith("<font", StringComparison.Ordinal) ||
                             tmp.StartsWith("<div", StringComparison.Ordinal) ||
                             tmp.StartsWith("<i", StringComparison.Ordinal) ||
                             tmp.StartsWith("<b", StringComparison.Ordinal) ||
                             tmp.StartsWith("<s", StringComparison.Ordinal) ||
                             tmp.StartsWith("</", StringComparison.Ordinal)))
                        {
                            total.Append(WebUtility.HtmlDecode(partial.ToString()));
                            partial = new StringBuilder();
                            tagOn = true;
                            total.Append('<');
                        }
                        else if (text.Substring(i).StartsWith(">") && tagOn)
                        {
                            tagOn = false;
                            total.Append('>');
                        }
                        else if (!tagOn)
                        {
                            partial.Append(text[i]);
                        }
                        else
                        {
                            total.Append(text[i]);
                        }
                    }
                    total.Append(WebUtility.HtmlDecode(partial.ToString()));
                    text = total.ToString();
                }
                else
                {
                    text = WebUtility.HtmlDecode(text);
                }

                string cleanText = text;
                while (cleanText.Contains("  "))
                    cleanText = cleanText.Replace("  ", " ");
                while (cleanText.Contains(Environment.NewLine + " "))
                    cleanText = cleanText.Replace(Environment.NewLine + " ", Environment.NewLine);
                while (cleanText.Contains(" " + Environment.NewLine))
                    cleanText = cleanText.Replace(" " + Environment.NewLine, Environment.NewLine);
                cleanText = cleanText.Trim();

                if (p.Lines.Count > 0 && !string.IsNullOrEmpty(millisecAsString)
                    && p.Lines[0] != null && p.Lines[0].Length > 0)
                {
                    p.EndTime = int.Parse(millisecAsString);
                    subtitle.Add(p);
                    p = new SubtitleItem();
                }

                p.Lines = new List<string> { cleanText };
                int l;
                if (int.TryParse(millisecAsString, out l))
                    p.StartTime = l;

                if (syncEndPos <= 0)
                {
                    syncStartPos = -1;
                }
                else
                {
                    syncStartPos = allInputLower.IndexOf(syncTag, syncEndPos, StringComparison.Ordinal);
                    index = syncStartPos + syncTag.Length;

                    syncStartPosEnc = allInputLower.IndexOf(syncTagEnc, syncEndPos, StringComparison.Ordinal);
                    if ((syncStartPosEnc >= 0 && syncStartPosEnc < syncStartPos) || syncStartPos == -1)
                    {
                        syncStartPos = syncStartPosEnc;
                        index = syncStartPosEnc + syncTagEnc.Length;
                    }
                }
            }
            if (!string.IsNullOrEmpty(p.Lines[0]) && !subtitle.Contains(p)
                                    && p.Lines[0] != null && p.Lines[0].Length > 0)
            {
                p.EndTime = p.StartTime;
                subtitle.Add(p);
            }

            if (subtitle.Count > 0 &&
                (subtitle[subtitle.Count - 1].Lines[0].ToUpper().Trim() == "</BODY>" ||
                subtitle[subtitle.Count - 1].Lines[0].ToUpper().Trim() == "<BODY>"))
                subtitle.RemoveAt(subtitle.Count - 1);

            System.Diagnostics.Debug.WriteLine(subtitle.Count);
        }
    }
}
