using IranSub;
using System;
using System.Collections.Generic;
public class SubZir : List<SubtitleItem> { public string Name { get; set; } }

namespace IranSub
{
    public class SubtitleItem
    {

        public int Number { get; set; }

        public int StartTime { get; set; }

        public int EndTime { get; set; }

        public List<string> Lines { get; set; }

        string text_="";
        public string Text
        {
            get
            {
                if (string.IsNullOrEmpty(text_))
                    return string.Join(Environment.NewLine, Lines);
                else return text_;
            }
            set { text_ = value; } }
        public string TranslatedText { get; set; }

        public SubtitleItem()
        {
            this.Lines = new List<string>();
        }


        public override string ToString()
        {
            var startTs = new TimeSpan(0, 0, 0, 0, StartTime);
            var endTs = new TimeSpan(0, 0, 0, 0, EndTime);

            var res = string.Format("{0} --> {1}: {2}", startTs.ToString("G"), endTs.ToString("G"), string.Join(Environment.NewLine, Lines));
            return res;
        }

    }
}