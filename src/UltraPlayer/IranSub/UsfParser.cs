using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;

namespace IranSub.Parsers
{
    // parser for Universal Subtitle Format.usf

    //    <?xml version="1.0" encoding="utf-8"?>
    //<USFSubtitles version="1.0">
    //  <metadata>
    //    <title>Universal Subtitle Format</title>
    //    <author>
    //      <name></name>
    //      <email></email>
    //      <url></url>
    //    </author>
    //    <language code="eng">English</language>
    //    <date>2015-09-30</date>
    //    <comment>This is a USF file</comment>
    //  </metadata>
    //  <styles>
    //    <!-- Here we redefine the default style -->
    //    <style name="Default">
    //      <fontstyle face="Arial" size="24" color="#FFFFFF" back-color="#AAAAAA" />
    //      <position alignment="BottomCenter" vertical-margin="20%" relative-to="Window" />
    //    </style>
    //  </styles>
    //  <subtitles>
    //    <subtitle start="00:00:14.014" stop="00:00:20.979">
    //      <text style="Default">تقديم به تمام پارسي زبانان جهان</text>
    //    </subtitle>
    //    <subtitle start="00:00:21.980" stop="00:00:28.987">
    //      <text style="Default">ارائه اي ديگر از تيم ترجمه فارسي سابتايتل
    //WwW.FarsiSubtitle.CoM</text>
    //    </subtitle>
    /// <summary>
    /// 
    /// </summary>
    public class UsfParser : ISubtitlesParser
    {
        public UsfParser() 
        { }
        public SubZir ParseStream(Stream xmlStream, Encoding encoding)
        {

            xmlStream.Position = 0;
            var items = new SubZir();
            StreamReader st = new StreamReader(xmlStream, encoding,true);
            string reshte = st.ReadToEnd();

            XmlDocument document = new XmlDocument();
            document.LoadXml(reshte);
            XmlNodeList list = document.SelectSingleNode("USFSubtitles").SelectSingleNode("subtitles").SelectNodes("subtitle");

            for (int j = 0; j < list.Count; j++)
            {
                try
                {
                    string startText = "", endText = "", text = "";
                    startText = list[j].Attributes.GetNamedItem("start").InnerText;
                    endText = list[j].Attributes.GetNamedItem("stop").InnerText;
                    text = list[j].SelectSingleNode("text").InnerText;

                    int start = ParseSrtTimecode(startText);
                    int end = ParseSrtTimecode(endText);
                    items.Add(new SubtitleItem()
                    {
                        StartTime = start,
                        EndTime = end,
                        Lines = new List<string>() { text }
                    });
                }
                catch { }
            }
            if (items.Any())
            {
                return items;
            }
            else
            {
                throw new ArgumentException("Stream is not in a valid Universal Subtitle XML format");
            }
        }

        private int ParseSrtTimecode(string s)
        {
            var match = Regex.Match(s, "[0-9]+:[0-9]+:[0-9]+[,\\.][0-9]+");
            if (match.Success)
            {
                s = match.Value;
                TimeSpan result;
                if (TimeSpan.TryParse(s.Replace(',', '.'), out result))
                {
                    var nbOfMs = (int)result.TotalMilliseconds;
                    return nbOfMs;
                }
            }
            return -1;
        }

    }
}
