using System.Collections.Generic;

namespace IranSub
{
    public class SubtitlesFormat
    {

        public string Name { get; set; }

        public string Extension { get; set; }


        public SubtitlesFormat(){}



        public static SubtitlesFormat SubRipFormat = new SubtitlesFormat()
        {
            Name = "SubRip",
            Extension = @"\.srt"
        };

        public static SubtitlesFormat MicroDvdFormat = new SubtitlesFormat()
        {
            Name = "MicroDvd",
            Extension = @"\.sub"
        };


        public static SubtitlesFormat SubStationAlphaFormat = new SubtitlesFormat()
        {
            Name = "SubStationAlpha",
            Extension = @"\.ssa"
        };

        public static SubtitlesFormat TtmlFormat = new SubtitlesFormat()
        {
            Name = "TTML",
            Extension = @"\.ttml"
        };

        public static SubtitlesFormat WebVttFormat = new SubtitlesFormat()
        {
            Name = "WebVTT",
            Extension = @"\.vtt"
        };

        public static SubtitlesFormat AssFormat = new SubtitlesFormat()
        {
            Name = "AdvancedSubStationAlpha",
            Extension = @"\.ass"
        };

        public static SubtitlesFormat UsfFormat = new SubtitlesFormat()
        {
            Name = "UniversalSub",
            Extension = @"\.usf"
        };

        public static SubtitlesFormat DVDSubtitleFormat = new SubtitlesFormat()
        {
            Name = "DVDSubtitle",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SoftNiColonSubFormat = new SubtitlesFormat()
        {
            Name = "SoftNiColonSub",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SoftNiSubFormat = new SubtitlesFormat()
        {
            Name = "SoftNiSub",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SonyDVDArchitectFormat = new SubtitlesFormat()
        {
            Name = "SonyDVDArchitect",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SonyDVDArchitectExplicitDurationFormat = new SubtitlesFormat()
        {
            Name = "SonyDVDArchitectExplicitDuration",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SonyDVDArchitectTabsFormat = new SubtitlesFormat()
        {
            Name = "SonyDVDArchitectTabs",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SonyDVDArchitectWithLineNumbersFormat = new SubtitlesFormat()
        {
            Name = "Sony DVDArchitect w. line#",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SubViewer10Format = new SubtitlesFormat()
        {
            Name = "SubViewer 1.0",
            Extension = @"\.sub"
        };

        public static SubtitlesFormat SubViewer20Format = new SubtitlesFormat()
        {
            Name = "SubViewer 2.0",
            Extension = @"\.sub"
        };
        public static SubtitlesFormat JsonFormat = new SubtitlesFormat()
        {
            Name = "Json Subtitle",
            Extension = @"\.json"
        };

        public static SubtitlesFormat Json2Format = new SubtitlesFormat()
        {
            Name = "Json2 Subtitle",
            Extension = @"\.json"
        };

        public static SubtitlesFormat Json3Format = new SubtitlesFormat()
        {
            Name = "Json3 Subtitle",
            Extension = @"\.json"
        };

        public static SubtitlesFormat Json4Format = new SubtitlesFormat()
        {
            Name = "Json4 Subtitle",
            Extension = @"\.json"
        };
        public static SubtitlesFormat SamiFormat = new SubtitlesFormat()
        {
            Name = "Sami Subtitle",
            Extension = @"\.smi"
        };

        public static List<SubtitlesFormat> SupportedSubtitlesFormats = new List<SubtitlesFormat>()
            {
                SubRipFormat, 
                MicroDvdFormat,
                SubStationAlphaFormat,
                TtmlFormat,
                WebVttFormat,
                AssFormat,
                UsfFormat,
                DVDSubtitleFormat,
                SoftNiColonSubFormat,
                SoftNiSubFormat,
                SonyDVDArchitectFormat,
                SonyDVDArchitectExplicitDurationFormat,
                SonyDVDArchitectTabsFormat,
                SonyDVDArchitectWithLineNumbersFormat,
                SubViewer10Format,
                SubViewer20Format,
                JsonFormat,
                Json2Format,
                Json3Format,
                Json4Format,
                SamiFormat
            };

    }

    
}
