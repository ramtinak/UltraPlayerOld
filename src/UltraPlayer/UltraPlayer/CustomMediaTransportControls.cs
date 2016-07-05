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
using System;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace UltraPlayer
{
    public delegate void MediaModeResult(object sender, object e);
    public sealed class CustomMediaTransportControls : MediaTransportControls
    {
        #region Events
        public event MediaModeResult OnValumeResult;
        public event MediaModeResult OnSeekResult;
        #endregion
        CompositeTransform transformTopTimelineBorder;
        Border darknessBorder, timelineBorder;
        Point startPostionUp, startPositionSubtitle;
        private bool IsDisplayedUpPanel = false;

        public RichTextBlock SubtitleText;

        public Border ShadowBorder;

        StackPanel stackPlayPauseButton;
        AppBarButton PlayPauseButtonCTM, ZoomButtonCTM, FullWindowBTN;
        public DispatcherTimer timerPanel = new DispatcherTimer();
        int intervalTimerPanel = 0;
        Grid lockGrid;

        AppBarButton lockUnLockOrientationButton;
        Button unlockButton, lockButton;
        Flyout subsFlyout;

        MenuFlyoutItem menuShowHideSub, menuOpenSub, menuSubSettings;
        public MenuFlyoutSubItem menuFlyoutRelativeSubs;


        public TextBlock txtShowSomething;
        TextBlock txtDarkness, txtVolume;

        #region Manipulation variables
        const double maxVolume = 1;
        double indexVolume = 1;
        double lastPositionVolume = 0; 

        const double maxDarkness = .8; 
        double indexDarkness = 0;
        double lastPositionDarkness = 0;
        #endregion
        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
            timerPanel.Interval = TimeSpan.FromSeconds(1);
            timerPanel.Tick += TimerPanel_Tick;
            timerPanel.Start();
        }

        private void TimerPanel_Tick(object sender, object e)
        {
            intervalTimerPanel++;
            if (intervalTimerPanel > 4)
            {
                if (stackPlayPauseButton.Visibility == Visibility.Visible)
                {
                    if (IsDisplayedPanel)
                        HidePanel();
                    intervalTimerPanel = 0;
                }

                if (IsDisplayedUpPanel)
                {
                    HideUpPanel();
                    intervalTimerPanel = 0;
                }
            }
        }

        protected override void OnApplyTemplate()
        {
            HelperUP.Output("OnApplyTemplate");

            transformTopTimelineBorder = GetTemplateChild("transformTopTimelineBorder") as CompositeTransform;
            darknessBorder = GetTemplateChild("darknessBorder") as Border;
            SubtitleText = GetTemplateChild("txtSubtitle") as RichTextBlock;
            ShadowBorder = GetTemplateChild("shadowBorder") as Border;
            PlayPauseButtonCTM = GetTemplateChild("PlayPauseButtonCTM") as AppBarButton;
            PlayPauseButtonCTM.Click += PlayPauseButtonCTM_Click;
            ZoomButtonCTM = GetTemplateChild("ZoomButtonCTM") as AppBarButton;
            ZoomButtonCTM.Click += ZoomButtonCTM_Click;
            FullWindowBTN = GetTemplateChild("FullWindowBTN") as AppBarButton;
            FullWindowBTN.Click += FullWindowBTN_Click;
            stackPlayPauseButton = GetTemplateChild("stackPlayPauseButton") as StackPanel;
            stackPlayPauseButton.PointerEntered += StackPlayPauseButton_PointerEntered;
            stackPlayPauseButton.PointerExited += StackPlayPauseButton_PointerExited;
            darknessBorder.Tapped += DarknessBorder_Tapped;

            subsFlyout = GetTemplateChild("subsFlyout") as Flyout;

            timelineBorder = GetTemplateChild("timelineBorder") as Border;
            timelineBorder.PointerEntered += StackPlayPauseButton_PointerEntered;
            timelineBorder.PointerExited += StackPlayPauseButton_PointerExited;

            txtShowSomething = GetTemplateChild("txtShowSomething") as TextBlock;
            txtDarkness = GetTemplateChild("txtDarkness") as TextBlock;
            txtVolume = GetTemplateChild("txtVolume") as TextBlock;

            lockGrid = GetTemplateChild("lockGrid") as Grid;

            lockGrid.Tapped += LockGrid_Tapped;

            lockButton = GetTemplateChild("lockButton") as Button;
            unlockButton = GetTemplateChild("unlockButton") as Button;
            lockUnLockOrientationButton = GetTemplateChild("lockUnLockOrientationButton") as AppBarButton;


            lockButton.Click += LockButton_Click;
            unlockButton.Click += UnlockButton_Click;
            lockUnLockOrientationButton.Click += LockUnLockOrientationButton_Click;

            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
                PlayerPage.Current.MediaElement.CurrentStateChanged += MediaElement_CurrentStateChanged;


            
            menuShowHideSub = GetTemplateChild("menuShowHideSub") as MenuFlyoutItem;
            menuOpenSub = GetTemplateChild("menuOpenSub") as MenuFlyoutItem;
            menuSubSettings = GetTemplateChild("menuSubSettings") as MenuFlyoutItem;
            menuFlyoutRelativeSubs = GetTemplateChild("menuFlyoutRelativeSubs") as MenuFlyoutSubItem;

            menuShowHideSub.Click += MenuShowHideSub_Click;
            menuOpenSub.Click += MenuOpenSub_Click;
            menuSubSettings.Click += MenuSubSettings_Click;

            darknessBorder.ManipulationMode = ManipulationModes.TranslateY | ManipulationModes.TranslateX;
            darknessBorder.ManipulationStarted += manipulationStarted;
            darknessBorder.ManipulationCompleted += manipulationCompleted;
            darknessBorder.ManipulationDelta += manipulationDelta;

            ShadowBorder.ManipulationMode = ManipulationModes.TranslateX;
            ShadowBorder.ManipulationStarted += manipulationStarted;
            ShadowBorder.ManipulationCompleted += manipulationCompleted;
            ShadowBorder.ManipulationDelta += manipulationDelta;



            SubtitleText.ManipulationMode = ManipulationModes.TranslateX;
            SubtitleText.ManipulationStarted += manipulationStarted2;
            SubtitleText.ManipulationCompleted += manipulationCompleted2;
            base.OnApplyTemplate();
        }


        private void MenuSubSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowSomething("In the next update...");
        }

        private async void MenuOpenSub_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null)
                return;

            var filePicker = new Windows.Storage.Pickers.FileOpenPicker();
            filePicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            foreach (string format in HelperUP.subtitlesFormat)
                filePicker.FileTypeFilter.Add(format);
            var file = await filePicker.PickSingleFileAsync();
            if (file == null)
                return;
            if (PlayerPage.Current != null)
                PlayerPage.Current.ImportSubtitle(file);
        
        }

        private void MenuShowHideSub_Click(object sender, RoutedEventArgs e)
        {
            ShowSomething("In the next update...");
        }

        private void FullWindowBTN_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
            {
                if (PlayerPage.Current.MediaElement.IsFullWindow)
                {
                    PlayerPage.Current.MediaElement.IsFullWindow = false;
                    FullWindowBTN.Icon = new SymbolIcon(Symbol.BackToWindow);
                    ShowSomething("Default screen");
                }
                else
                {
                    PlayerPage.Current.MediaElement.IsFullWindow = true;
                    FullWindowBTN.Icon = new SymbolIcon(Symbol.FullScreen);
                    ShowSomething("Full screen");

                }
                HelperUP.Output("IsFullWindow " + PlayerPage.Current.MediaElement.IsFullWindow)
            ;
            }
        }

        private void ZoomButtonCTM_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
            {
                switch (PlayerPage.Current.MediaElement.Stretch)
                {
                    case Stretch.Fill:
                        PlayerPage.Current.MediaElement.Stretch = Stretch.None;
                        ZoomButtonCTM.Icon = new SymbolIcon(Symbol.ZoomOut);
                        ShowSomething("Stretch: None");
                        break;
                    case Stretch.None:
                        PlayerPage.Current.MediaElement.Stretch = Stretch.Uniform;
                        ZoomButtonCTM.Icon = new SymbolIcon(Symbol.ZoomIn);
                        ShowSomething("Stretch: Default (Uniform)");
                        break;
                    case Stretch.Uniform:
                        PlayerPage.Current.MediaElement.Stretch = Stretch.UniformToFill;
                        ZoomButtonCTM.Icon = new SymbolIcon(Symbol.ZoomOut);
                        ShowSomething("Stretch: Uniform to Fill");
                        break;
                    case Stretch.UniformToFill:
                        PlayerPage.Current.MediaElement.Stretch = Stretch.Fill;
                        ZoomButtonCTM.Icon = new SymbolIcon(Symbol.ZoomOut);
                        ShowSomething("Stretch: Fill");

                        break;
                    default:
                        break;
                }

            }
        }

        private void PlayPauseButtonCTM_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
            {
                if (PlayerPage.Current.MediaElement.CurrentState == MediaElementState.Paused)
                    PlayerPage.Current.MediaElement.Play();
                else
                    PlayerPage.Current.MediaElement.Pause();
            }
        }

        private void LockGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (unlockButton.Visibility == Visibility.Collapsed)
                unlockButton.Visibility = Visibility.Visible;
            else
                unlockButton.Visibility = Visibility.Collapsed;
        }

        private void StackPlayPauseButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            timerPanel.Start();
            intervalTimerPanel = 0;
        }

        private void StackPlayPauseButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            timerPanel.Stop();
            intervalTimerPanel = 0;
        }





        private void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            timelineBorder.Visibility =
                stackPlayPauseButton.Visibility = Visibility.Visible;
            lockGrid.Visibility = Visibility.Collapsed;
            ShowSomething("Back button enabled.\r\nShowing panels enabled.");
            if (FileManagerPage1.Current != null)
                FileManagerPage1.Current.lockBackButton = false;

        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            timelineBorder.Visibility =
                stackPlayPauseButton.Visibility =
                Visibility.Collapsed;
            lockGrid.Visibility = Visibility.Visible;
            ShowSomething("Back button disabled.\r\nShowing panels disabled.");
            if (FileManagerPage1.Current != null)
                FileManagerPage1.Current.lockBackButton = true;
        }

        private void LockUnLockOrientationButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)lockUnLockOrientationButton.Tag == "Lock")
            {
                PlayerPage.Current.lockOrientationMode = true;
                lockUnLockOrientationButton.Icon = new BitmapIcon
                {
                    Width = 25,
                    Height = 25,
                    UriSource = new Uri("ms-appx:///Assets/unLock Landscape-48.png")
                };
                lockUnLockOrientationButton.Tag = "UnLock";
                ShowSomething("Auto orientation Locked");

            }
            else
            {
                PlayerPage.Current.lockOrientationMode = false;

                lockUnLockOrientationButton.Icon = new BitmapIcon
                {
                    Width = 25,
                    Height = 25,
                    UriSource = new Uri("ms-appx:///Assets/Lock Landscape-48.png")
                };
                lockUnLockOrientationButton.Tag = "Lock";
                ShowSomething("Auto orientation Unlocked");

            }
        }
        
        private void MediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
            {
                HelperUP.Output("MediaElement_CurrentStateChanged pumped+" + PlayerPage.Current.MediaElement.CurrentState);
                switch (PlayerPage.Current.MediaElement.CurrentState)
                {
                    case MediaElementState.Playing:
                        PlayPauseButtonCTM.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/pauseIcon.png") } ;
                        break;
                    case MediaElementState.Closed:
                    case MediaElementState.Opening:
                    case MediaElementState.Paused:
                    case MediaElementState.Stopped:
                        PlayPauseButtonCTM.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/playIcon.png") };
                        break;
                    default:
                        PlayPauseButtonCTM.Icon = new BitmapIcon { UriSource = new Uri("ms-appx:///Assets/playIcon.png") };
                        break;
                }
            }
        }

        private void DarknessBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            intervalTimerPanel = 0;
            if (lockGrid.Visibility == Visibility.Collapsed)
            {
                if (IsDisplayedPanel)
                    HidePanel();
                else
                    DisplayPanel();
            }
        }

        private void manipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            startPostionUp = e.Position;
            if (e.Position.X > (ActualWidth / 2))
            {
                Storyboard storyBoard = new Storyboard();
                DoubleAnimation da = new DoubleAnimation();
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da.To = 1;
                storyBoard.Children.Add(da);
                Storyboard.SetTarget(da, txtVolume);
                Storyboard.SetTargetProperty(da, "Opacity");
                storyBoard.Begin();
            }
            else
            {
                Storyboard storyBoard = new Storyboard();
                DoubleAnimation da = new DoubleAnimation();
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da.To = 1;
                storyBoard.Children.Add(da);
                Storyboard.SetTarget(da, txtDarkness);
                Storyboard.SetTargetProperty(da, "Opacity");
                storyBoard.Begin();
            }
        }

        private void manipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Position.X > (ActualWidth / 2))
            {
                if (lastPositionVolume < e.Position.Y)
                {
                    if (indexVolume > 0)
                    {
                        indexVolume -= .005;
                        VolumeResult(indexVolume);
                        txtVolume.Text = string.Format("Volume: {0}%", (indexVolume * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                    else
                    {
                        VolumeResult(indexVolume = 0);

                        txtVolume.Text = "Volume muted";
                    }
                }
                else
                {
                    if (indexVolume < maxVolume)
                    {
                        indexVolume += .005;
                        VolumeResult(indexVolume);

                        txtVolume.Text = string.Format("Volume: {0}%", (indexVolume * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                    else
                    {
                        VolumeResult(indexVolume = maxVolume);

                        txtVolume.Text = string.Format("Volume: {0}%", (indexVolume * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                }
                lastPositionVolume = e.Position.Y;
            }
            else
            {
                if (lastPositionDarkness < e.Position.Y)
                {
                    if (indexDarkness < maxDarkness)
                    {
                        indexDarkness += .005;
                        darknessBorder.Opacity = indexDarkness;

                        txtDarkness.Text = string.Format("Brightness: {0}%", ((1 - indexDarkness) * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                    else
                    {
                        darknessBorder.Opacity = indexDarkness = maxDarkness;
                        txtDarkness.Text = string.Format("Brightness: {0}%", ((1 - indexDarkness) * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                }
                else
                {
                    if (indexDarkness > 0)
                    {
                        indexDarkness -= .005;
                        darknessBorder.Opacity = indexDarkness;
                        txtDarkness.Text = string.Format("Brightness: {0}%", ((1 - indexDarkness) * 100).ToString().Split(new string[] { ".", "\\", ",", "/" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    }
                    else
                    {
                        darknessBorder.Opacity = indexDarkness = 0;
                        txtDarkness.Text = "Brightness: 100%";
                    }
                }
                lastPositionDarkness = e.Position.Y;
            }
        }

        private void manipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var endPostionUp = e.Position;
            var actWidth = ActualWidth;
            var actWithNega = actWidth - 300;
            if (e.Position.Y <= 300 && e.Position.Y > -20)
            {
                if (endPostionUp.Y < startPostionUp.Y)
                    HideUpPanel();
                else
                    DisplayUpPanel();
            }
            //if (e.Position.X > (ActualWidth / 2))
            {
                Storyboard storyBoard = new Storyboard();
                DoubleAnimation da = new DoubleAnimation();
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da.To = 0;
                storyBoard.Children.Add(da);
                Storyboard.SetTarget(da, txtVolume);
                Storyboard.SetTargetProperty(da, "Opacity");
                DoubleAnimation da2 = new DoubleAnimation();
                da2.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da2.To = 0;
                storyBoard.Children.Add(da2);
                Storyboard.SetTarget(da2, txtDarkness);
                Storyboard.SetTargetProperty(da2, "Opacity");
                storyBoard.Begin();
            }
        }
        
        private void manipulationStarted2(object sender, ManipulationStartedRoutedEventArgs e)
        {
            startPositionSubtitle = e.Position;
        }

        private void manipulationCompleted2(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var endPostionSubtitle = e.Position;
            if (endPostionSubtitle.X > startPositionSubtitle.X)
            {
                SeekToNextSubtitle();
            }
            else
            {
                SeekToNextSubtitle(false);
            }
        }
        void SeekToNextSubtitle(bool next = true)
        {
            if (PlayerPage.Current == null || PlayerPage.Current != null && PlayerPage.Current.SubtitleList == null)
                return;
            if (PlayerPage.Current.CurrentSubtitleItem == null)
                return;
            var sub = PlayerPage.Current.CurrentSubtitleItem;
            var index = PlayerPage.Current.SubtitleList.FindIndex(item =>
                item.StartTime == sub.StartTime && item.EndTime == sub.EndTime);
            if (next)
            {
                if (index > 0)
                {
                    if (index + 1 <= PlayerPage.Current.SubtitleList.Count)
                    {
                        try
                        {
                            SubtitleItem itemSub = PlayerPage.Current.SubtitleList[index + 1];
                            PlayerPage.Current.MediaElement.Pause();
                            PlayerPage.Current.MediaElement.Position = TimeSpan.FromMilliseconds(itemSub.StartTime);
                            PlayerPage.Current.MediaElement.Play();
                        }
                        catch { }
                    }
                }

            }
            else
            {
                if (index > 0)
                {
                    if (index - 1 <= PlayerPage.Current.SubtitleList.Count)
                    {
                        try
                        {
                            SubtitleItem itemSub = PlayerPage.Current.SubtitleList[index - 1];
                            PlayerPage.Current.MediaElement.Pause();
                            PlayerPage.Current.MediaElement.Position = TimeSpan.FromMilliseconds(itemSub.StartTime);
                            PlayerPage.Current.MediaElement.Play();
                        }
                        catch { }
                    }
                }
            }
        }
        
        public void DisplayUpPanel()
        {
            try
            {
                SlideAnimationUpPanel(false);
            }
            catch (Exception) { }
        }

        public void HideUpPanel()
        {
            try
            {
                SlideAnimationUpPanel(true);
            }
            catch (Exception) { }
        }

        private void SlideAnimationUpPanel(bool hide)
        {
            Storyboard s = new Storyboard();

            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            if (hide)
                da.To = -200;
            else
                da.To = 0;

            s.Children.Add(da);

            Storyboard.SetTarget(da, transformTopTimelineBorder);
            Storyboard.SetTargetProperty(da, "TranslateY");
            s.Completed += (se, e) => { IsDisplayedUpPanel = !IsDisplayedUpPanel; };
            s.Begin();
        }



        bool IsDisplayedPanel = false;
        public void DisplayPanel()
        {
            stackPlayPauseButton.Visibility = Visibility.Visible;
            try
            {
                SlideAnimation(false);
            }
            catch (Exception) { }
        }

        public void HidePanel()
        {
            try
            {
                SlideAnimation(true);
            }
            catch (Exception) { }
        }
        private void SlideAnimation(bool hide)
        {
            Storyboard s = new Storyboard();

            DoubleAnimation da = new DoubleAnimation();
            da.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            if (hide)
                da.To = 0;
            else
                da.To = 1;

            s.Children.Add(da);
            Storyboard.SetTarget(da, stackPlayPauseButton);
            Storyboard.SetTargetProperty(da, "Opacity");
            s.Completed += (se, e) => 
            {
                IsDisplayedPanel = !IsDisplayedPanel;
                if (!IsDisplayedPanel)
                    stackPlayPauseButton.Visibility = Visibility.Collapsed;
            };
            s.Begin();
        }






        private void VolumeResult(double volume)
        {
            if (PlayerPage.Current != null && PlayerPage.Current.MediaElement != null)
                PlayerPage.Current.MediaElement.Volume = volume;
        
            OnValumeResult?.Invoke(this, volume);
        }
        private void SeekResult(double seekTime)
        {
            OnSeekResult?.Invoke(this, seekTime);
        }
        

        #region public methods
        public void SetMarginButton(double d)
        {
            var v = new Thickness();
            v.Bottom = d;
            ShadowBorder.Margin = v;
        }
        public void SetShowBox(bool show)
        {
            if (show)
                ShadowBorder.Background = new SolidColorBrush(HelperUP.GetColorFromHex("#7F000000"));
            else
                ShadowBorder.Background = new SolidColorBrush(Colors.Transparent);
        }
        public void SetFontSize(double d)
        {
            SubtitleText.FontSize = d;
        }
        public void SetCharacterSpacing(int d)
        {
            SubtitleText.CharacterSpacing = d;
        }
        public void SetFontFamily(string fontFamily)
        {
            SubtitleText.FontFamily = new Windows.UI.Xaml.Media.FontFamily(fontFamily);
        }
        public void SetFontStyle(Windows.UI.Text.FontStyle style)
        {
            SubtitleText.FontStyle = style;
        }
        public void SetFontWeight(ushort d)
        {
            SubtitleText.FontWeight = new Windows.UI.Text.FontWeight { Weight = d };
        }
        public void SetForeground(Color color)
        {
            SubtitleText.Foreground = new SolidColorBrush(color);
        }
        public void SetTextAlignment(TextAlignment alignment)
        {
            SubtitleText.TextAlignment = alignment;
        }


        private string AddZero(int time)
        {
            if (time < 10)
                return "0" + time;
            return time.ToString();
        }

        async public void ShowSomething(string text, int timeToShowByMiliSec = 3600)
        {
            if (txtShowSomething == null && string.IsNullOrEmpty(text))
                return;
            try
            {
                txtShowSomething.Visibility = Visibility.Visible;
                txtShowSomething.Text = text;
                Storyboard storyBoard = new Storyboard();
                DoubleAnimation da = new DoubleAnimation();
                da.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da.To = 1;
                storyBoard.Children.Add(da);
                Storyboard.SetTarget(da, txtShowSomething);
                Storyboard.SetTargetProperty(da, "Opacity");
                storyBoard.Begin();
                await Task.Delay(timeToShowByMiliSec);
                storyBoard = new Storyboard();
                DoubleAnimation da2 = new DoubleAnimation();
                da2.Duration = new Duration(TimeSpan.FromMilliseconds(300));
                da2.To = 0;
                storyBoard.Children.Add(da2);
                Storyboard.SetTarget(da2, txtShowSomething);
                Storyboard.SetTargetProperty(da2, "Opacity");
                storyBoard.Begin();
            }
            catch { }
            txtShowSomething.Text = "";
            txtShowSomething.Opacity = 0;
            txtShowSomething.Visibility = Visibility.Collapsed;
        }
        #endregion



    }
}
