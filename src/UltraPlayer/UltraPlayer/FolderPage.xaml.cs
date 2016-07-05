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

using System;
using System.IO;
using System.Linq;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace UltraPlayer
{
    public sealed partial class FolderPage : Page
    {
        public static FolderPage Current;
        public string Text { get { return text.Text; } set { text.Text = value; } }
        public AdaptiveGridView LV { get { return lv; } }
        public Frame MyFrame { get { return frame; } }
        public FolderPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            Current = this;
            Loaded += FolderPage_Loaded;
        }
        

        private void FolderPage_Loaded(object sender, RoutedEventArgs e)
        {
            lv.Items.Clear();
            if (HelperUP.TokenList != null && HelperUP.TokenList.Any())
                foreach (var item in HelperUP.TokenList)
                {
                    HelperUP.Output( Directory.GetDirectoryRoot(item.Path));

                    lv.Items.Add(item);
                }
        }

        private async void btnAddFolder_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add("*");
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder == null)
                return;

            var randomName = HelperUP.CreateTokenName();
            MToken token = new MToken();
            token.Name = folder.Name;
            token.Path = folder.Path;
            token.Token = randomName;
            if (HelperUP.AddToken(token))
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(token.Token, folder);
                LV.Items.Add(token);
            }
        }

        private async void LV_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e != null && e.ClickedItem != null && e.ClickedItem.GetType() == typeof(MToken))
            {
                var clicked = e.ClickedItem as MToken;
                var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(clicked.Token);
                if (folder == null)
                    return;
                MyFrame.Visibility = Visibility.Visible;
                LV.Visibility = Visibility.Collapsed;
                frame.Navigate(typeof(FileManagerPage1), folder);
            }
        }
    }
}
