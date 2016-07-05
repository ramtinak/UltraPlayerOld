/* 
 * <=-!-$ Iranian Programmers $-!-=>
 * 
 * This a tiny source code of Ultra Player UWP
 * 
 * All these codes came from Parse Dev Studio.
 * 
 * Version 1.1.0.0 is free and you can use source codes.
 * Other versions isn't free.
 * 
 * Developer and programmer: Ramtin Jokar
 * 
 * Ramtinak@live.com
 * 
 * [Developed in Parse Dev Studio]
 * 
 * 
 * Follow Us:
 * http://www.win-nevis.com
 * http://www.parsedev.com
 * 
 * 
 * <=-!-$ Iranian Programmers $-!-=>
 * 
 */

using IranSub;
using IranSub.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Windows.Devices.Sensors;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace UltraPlayer
{
    public sealed partial class PlayerPage : Page
    {
        public bool DontUseDefaultSettingsOfSub = false;
        public bool CanChangeVolume = true;
        public double seekDouble = 0;
        public bool showSubtitle = true;
        public DispatcherTimer dt = new DispatcherTimer(), cursorTimer = new DispatcherTimer(),
            positionTimer = new DispatcherTimer();
        public CoreCursor _baseCursor;
        public MediaElement MediaElement { get { return mediaElement; } }
        public CustomMediaTransportControls CustomMTC { get { return customMTC; } }
        public static PlayerPage Current;
        public SubZir SubtitleList;
        public int SubtitleCurrentIndex = 0;
        public SubtitleItem CurrentSubtitleItem;
        public DispatcherTimer timerOrientation = new DispatcherTimer();
        public bool lockOrientationMode = false;
        public StorageFile MovieFile;
        private MediaPlaybackItem item;
        List<ItemSub> subList;

        public PlayerPage()
        {
            this.InitializeComponent();
            Current = this;
            dt.Interval = TimeSpan.FromMilliseconds(1);
            dt.Tick += Dt_Tick;
            positionTimer.Interval = TimeSpan.FromMilliseconds(1);
            timerOrientation.Interval = TimeSpan.FromMilliseconds(100);
            timerOrientation.Tick += TimerOrientation_Tick;
            AllowDrop = true;
            Loaded += PlayerPage_Loaded;
        }

        private void PlayerPage_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer t = new DispatcherTimer();
            t.Interval = TimeSpan.FromSeconds(3);
            t.Tick += (s, ss) =>
             {
                 t.Stop();
                 if (subList != null && subList.Any())
                     for (int i = 0; i < subList.Count; i++)
                     {
                         ItemSub item = subList[i];
                         MenuFlyoutItem mfi = new MenuFlyoutItem();
                         mfi.Text = item.Name;
                         mfi.Tag = item;
                         mfi.Click += Mfi_Click;
                         customMTC.menuFlyoutRelativeSubs.Items.Add(mfi);
                     }
             };
            t.Start();
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            timerOrientation.Stop();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            timerOrientation.Start();
            if (e != null && e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(StorageFile))
                    LaunchedFile(e.Parameter as StorageFile);
                else if (e.Parameter.GetType() == typeof(ItemWithItemSub))
                {
                    ItemWithItemSub iwIs = e.Parameter as ItemWithItemSub;
                    LaunchedFile(iwIs.Item.File);
                    subList = iwIs.ItemSubList;
                }
            }
        }

        private void Mfi_Click(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                try
                {
                    MenuFlyoutItem mfi = sender as MenuFlyoutItem;
                    ItemSub itemSub = mfi.Tag as ItemSub;
                    if (itemSub != null && itemSub.File != null)
                        ImportSubtitle(itemSub.File);
                }
                catch (Exception ex) { HelperUP.Output("mediaElement_DoubleTapped ex: " + ex.Message); }
            }
        }

        private void TimerOrientation_Tick(object sender, object e)
        {
            try
            {
                if (!ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
                    return;
                if (!lockOrientationMode)
                {
                    var currentOrientation = SimpleOrientationSensor.GetDefault().GetCurrentOrientation();
                    switch (currentOrientation)
                    {
                        //Landscape Left
                        case Windows.Devices.Sensors.SimpleOrientation.Rotated90DegreesCounterclockwise:
                            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Landscape;
                            break;
                        //Landscape Right
                        case Windows.Devices.Sensors.SimpleOrientation.Rotated270DegreesCounterclockwise:
                            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.LandscapeFlipped;
                            break;
                        //Portrait Down
                        case Windows.Devices.Sensors.SimpleOrientation.Rotated180DegreesCounterclockwise:
                            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.PortraitFlipped;
                            break;
                        //Portrait Up
                        case Windows.Devices.Sensors.SimpleOrientation.NotRotated:
                            Windows.Graphics.Display.DisplayInformation.AutoRotationPreferences = Windows.Graphics.Display.DisplayOrientations.Portrait;
                            break;
                    }
                }
                else
                {
                    switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
                    {
                        case DisplayOrientations.Landscape:
                            DisplayInformation.AutoRotationPreferences =
    DisplayOrientations.Landscape;
                            break;
                        case DisplayOrientations.LandscapeFlipped:
                            DisplayInformation.AutoRotationPreferences =
    DisplayOrientations.LandscapeFlipped;
                            break;
                        case DisplayOrientations.None:
                            DisplayInformation.AutoRotationPreferences =
    DisplayOrientations.None;
                            break;
                        case DisplayOrientations.Portrait:
                            DisplayInformation.AutoRotationPreferences =
    DisplayOrientations.Portrait;
                            break;
                        case DisplayOrientations.PortraitFlipped:
                            DisplayInformation.AutoRotationPreferences =
    DisplayOrientations.PortraitFlipped;
                            break;
                    }
                }
            }
            catch { }
        }

        public async void LaunchedFile(StorageFile file)
        {
            if (file != null)
            {
                bool movie = false;
                bool subtitles = false;
                foreach (string item in HelperUP.moviesFormat)
                    if (item.ToLower().Equals(file.FileType.ToLower()))
                    {
                        movie = true;
                        break;
                    }

                foreach (string item in HelperUP.subtitlesFormat)
                    if (item.ToLower().Equals(file.FileType.ToLower()))
                    {
                        subtitles = true;
                        break;
                    }

                if (movie)
                {
                    item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));

                    mediaElement.SetPlaybackSource(item);


                    MovieFile = file;


                    string filePath = file.Path;

                    filePath = filePath.Replace(Path.GetExtension(filePath), ".srt");
                    StorageFile file2 = null;
                    try
                    {
                        file2 = await StorageFile.GetFileFromPathAsync(filePath);
                    }
                    catch { return; }
                    if (file2 == null)
                        return;
                    ImportSubtitle(file2);
                }
                else if (subtitles)
                {

                    dt.Stop();
                    ImportSubtitle(file);
                    try
                    {

                        dt.Start();
                    }
                    catch { }
                }
            }
        }

        async public void ImportSubtitle(StorageFile file)
        {
            try
            {

                if (file == null)
                    return;
                Stream stream = await file.OpenStreamForReadAsync();


                var parser = new SubParser();
                var fileName = Path.GetFileName(file.Path);
                SubtitleEncoding Subencoding = HelperUP.GetSubtitleEncoding(stream);
                Encoding encoding = new UTF8Encoding();
                if (Subencoding == SubtitleEncoding.ASCII)
                    encoding = new ASCIIEncoding();
                else if (Subencoding == SubtitleEncoding.Unicode)
                    encoding = new UnicodeEncoding();
                else if (Subencoding == SubtitleEncoding.UTF8)
                    encoding = new UTF8Encoding();
                else if (Subencoding == SubtitleEncoding.Windows1256)
                    encoding = new CustomCP1256Encoding();
                else if (Subencoding == SubtitleEncoding.UTF7)
                    encoding = new UTF7Encoding();
                else if (Subencoding == SubtitleEncoding.UTF32)
                    encoding = Encoding.UTF32;
                else if (Subencoding == SubtitleEncoding.BigEndianUnicode)
                    encoding = Encoding.BigEndianUnicode;
                var mostLikelyFormat = parser.GetMostLikelyFormat(fileName, stream, encoding);

                HelperUP.Output("mostLikelyFormat: " + mostLikelyFormat.Name);
                SubtitleList = parser.ParseStream(await file.OpenStreamForReadAsync(), encoding, mostLikelyFormat);
                dt.Start();
            }
            catch (Exception ex) { HelperUP.Output("ImportSubtitle ex: " + ex.Message); }
        }
        

        private void Dt_Tick(object sender, object e)
        {
            try
            {
                if (SubtitleList != null && SubtitleList.Any() && showSubtitle)
                {
                    var v = (from item in SubtitleList
                             where item != null
                             && item.StartTime + seekDouble <= mediaElement.Position.TotalMilliseconds
                             && item.EndTime + seekDouble >= mediaElement.Position.TotalMilliseconds
                             orderby item descending
                             select item).FirstOrDefault();
                    CurrentSubtitleItem = v;
                    if (v != null)
                    {
                        customMTC.SubtitleText.Blocks.Clear();

                        Paragraph myParagraph = new Paragraph();
                        int nextParagraph = 1;
                        foreach (string item in v.Lines)
                        {
                            if (GetRun(item) != null)
                            {
                                myParagraph.Inlines.Add(GetRun(item.Replace("ي", "ی")));
                                try
                                {
                                    if (v.Lines.Count < nextParagraph && v.Lines[nextParagraph] != null)
                                    {
                                        myParagraph.Inlines.Add(new LineBreak());
                                    }
                                }
                                catch (Exception ex) { HelperUP.Output("nextParagraph ex: " + ex.Message); }
                            }
                            nextParagraph++;
                        }
                        customMTC.SubtitleText.Blocks.Add(myParagraph);
                        customMTC.ShadowBorder.Visibility = customMTC.SubtitleText.Visibility = Windows.UI.Xaml.Visibility.Visible;

                    }
                    else
                    {
                        customMTC.ShadowBorder.Visibility = customMTC.SubtitleText.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        customMTC.SubtitleText.Blocks.Clear();
                    }
                }
                else
                    customMTC.SubtitleText.Blocks.Clear();



            }
            catch (Exception ex) { HelperUP.Output("mediaPlayer_PositionChanged ex: " + ex.Message); }

        }

        Run GetRun(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                text = HelperUP.FixInvalidItalicTags(text);
                text = HelperUP.FixInvalidFontTags(text);
                text = HelperUP.FixInvalidUnderlineTags(text);
                text = HelperUP.FixInvalidBoldTags(text);
                Run run = new Run();
                string pattern = @"<b>(?<bold>.*)</b>";
                MatchCollection reg = Regex.Matches(text, pattern);
                if (reg.Count > 0)
                    run.FontWeight = new Windows.UI.Text.FontWeight { Weight = 700 };

                pattern = @"<i>(?<italic>.*)</i>";
                reg = Regex.Matches(text, pattern);
                if (reg.Count > 0)
                    run.FontStyle = Windows.UI.Text.FontStyle.Italic;

                pattern = @"color=""(?<color>.*)""";
                reg = Regex.Matches(text, pattern);
                if (reg.Count > 0)
                {
                    foreach (Match item in reg)
                    {
                        try
                        {
                            if (item.Groups["color"].ToString().StartsWith("#"))
                            {
                                if (item.Groups["color"].ToString().Length == 7)
                                    run.Foreground = new SolidColorBrush(HelperUP.GetColorFromHex("#ff" + item.Groups["color"].ToString().Substring(1)));
                                else
                                    run.Foreground = new SolidColorBrush(HelperUP.GetColorFromHex(item.Groups["color"].ToString()));
                            }
                            else if (item.Groups["color"].ToString().ToLower().Equals("white"))
                                run.Foreground = new SolidColorBrush(Colors.White);
                            else if (item.Groups["color"].ToString().ToLower().Equals("red"))
                                run.Foreground = new SolidColorBrush(Colors.Red);
                            else if (item.Groups["color"].ToString().ToLower().Equals("cyan"))
                                run.Foreground = new SolidColorBrush(Colors.Cyan);
                            else if (item.Groups["color"].ToString().ToLower().Equals("yellow"))
                                run.Foreground = new SolidColorBrush(Colors.Yellow);
                            else if (item.Groups["color"].ToString().ToLower().Equals("orange"))
                                run.Foreground = new SolidColorBrush(Colors.Orange);
                            else if (item.Groups["color"].ToString().ToLower().Equals("blue"))
                                run.Foreground = new SolidColorBrush(Colors.Blue);
                            else if (item.Groups["color"].ToString().ToLower().Equals("black"))
                                run.Foreground = new SolidColorBrush(Colors.Black);
                            else if (item.Groups["color"].ToString().ToLower().Equals("brown"))
                                run.Foreground = new SolidColorBrush(Colors.Brown);
                            else if (item.Groups["color"].ToString().ToLower().Equals("green"))
                                run.Foreground = new SolidColorBrush(Colors.Green);
                            else if (item.Groups["color"].ToString().ToLower().Equals("pink"))
                                run.Foreground = new SolidColorBrush(Colors.Pink);
                            else
                                HelperUP.Output(string.Format("Color '{0}' not supported", item.Groups["color"].ToString()));
                            // white,red,cyan,yellow,orange,blue,black,brown,green
                        }
                        catch (Exception Exception) { HelperUP.Output("GetRun ex: " + Exception.Message); }
                    }

                }
                text = HelperUP.RemoveHtmlTags(text).Replace("</font>", "").Replace("</i>", "").Replace("</b>", "");
                run.Text = text;
                return run;
            }
            return null;
        }

    }
}
