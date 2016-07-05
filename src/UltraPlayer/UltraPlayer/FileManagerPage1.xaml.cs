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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
namespace UltraPlayer
{
    public sealed partial class FileManagerPage1 : Page
    {
        public StorageFolder CurrentFolder;
        List<ItemSub> subList = new List<ItemSub>();
        public bool lockBackButton = false;
        public static FileManagerPage1 Current;
        public FileManagerPage1()
        {
            this.InitializeComponent();
            Current = this;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            SystemNavigationManager.GetForCurrentView().BackRequested -= backRequested;
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            pb.Visibility = Visibility.Visible;
            pb.IsActive = true;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += backRequested;
            if (e != null && e.Parameter != null && e.Parameter.GetType() == typeof(StorageFolder))
            {
                CurrentFolder = e.Parameter as StorageFolder;
                if(FolderPage.Current!= null)
                    FolderPage.Current.Text = CurrentFolder.Name;
                var folders = await CurrentFolder.GetFoldersAsync();
                var files = await CurrentFolder.GetFilesAsync();
                SetFD(folders, files);
            }
            pb.Visibility = Visibility.Collapsed;
            pb.IsActive = false;
        }
        void SetFD(IReadOnlyList<StorageFolder> folders, IReadOnlyList<StorageFile> files)
        {
            var catList = new List<ItemCategory>();
            List<Item> list = new List<Item>();
            foreach (var item in folders)
                list.Add(new Item { Folder = item });
            var carGroups = list.OrderBy(e => e.Key).GroupBy(e => e.Key);
            var foldersCategory = new ItemCategory();
            foldersCategory.Items = new List<Item>();
            foldersCategory.CategoryName = "Folders";
            foreach (System.Linq.IGrouping<string, Item> item in carGroups)
                foldersCategory.Items.AddRange(item.ToList<Item>());
            if (foldersCategory.Items != null && foldersCategory.Items.Any())
                catList.Add(foldersCategory);
            SetFiles(files, catList);
        }
        void SetFiles(IReadOnlyList<StorageFile> files, List<ItemCategory> cats)
        {
            var catList = new List<ItemCategory>();
            subList.Clear();
            List<Item> list = new List<Item>();
            var othersCategory = new ItemCategory();
            othersCategory.Items = new List<Item>();
            othersCategory.CategoryName = "@";

            var numsCategory = new ItemCategory();
            numsCategory.Items = new List<Item>();
            numsCategory.CategoryName = "#";

            foreach (var item in files)
            {
                string ext = Path.GetExtension(item.Path).ToLower();
                if (HelperUP.moviesFormat.Any(s => s.Contains(ext)))
                    list.Add(new Item { File = item });
                if (HelperUP.subtitlesFormat.Any(s => s.Contains(ext)))
                    subList.Add(new ItemSub { File = item });
            }
            var carGroups = list.OrderBy(e => e.Key).GroupBy(e => e.Key);
            foreach (System.Linq.IGrouping<string, Item> item in carGroups)
            {
                string name = item.Key;

                if (Regex.IsMatch(name, @"^\d+"))
                    numsCategory.Items.AddRange(item.ToList<Item>());
                else if (Regex.IsMatch(name, @"^[A-Za-z\d]+"))
                    catList.Add(new ItemCategory() { CategoryName = item.Key, Items = item.ToList<Item>() });
                else
                    othersCategory.Items.AddRange(item.ToList<Item>());
            }

            var newCatList = new List<ItemCategory>();

            newCatList.AddRange(cats);



            if (numsCategory.Items != null && numsCategory.Items.Any())
                newCatList.Add(numsCategory);

            if (catList != null && catList.Any())
                newCatList.AddRange(catList);

            if (othersCategory.Items != null && othersCategory.Items.Any())
                newCatList.Add(othersCategory);

            groupItemsViewSource.Source = newCatList;
            ZoomedOutGridView.ItemsSource = newCatList;
        }
        private void backRequested(object sender, BackRequestedEventArgs e)
        {
            if (lockBackButton)
            {
                e.Handled = true;
                return;
            }
            Debug.WriteLine(((Frame)Window.Current.Content).GetType());
            if (Window.Current.Content != null && 
                ((Frame)Window.Current.Content).Content.GetType() == typeof(PlayerPage))
            {
                if (((Frame)Window.Current.Content).CanGoBack)
                {
                    e.Handled = true;
                    HelperUP.Output("reqa");
                    ((Frame)Window.Current.Content).GoBack();

                    return;
                }
            }

            if (FolderPage.Current != null && FolderPage.Current.MyFrame != null)
            {
                if (FolderPage.Current.MyFrame.CanGoBack)
                {
                    e.Handled = true;
                    FolderPage.Current.MyFrame.GoBack();
                    if (!FolderPage.Current.MyFrame.CanGoBack)
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                HelperUP.Output("req");
                if (!FolderPage.Current.MyFrame.CanGoBack)
                {
                    if (FolderPage.Current.MyFrame.Visibility == Visibility.Visible)
                    {
                        e.Handled = true;
                        FolderPage.Current.MyFrame.Visibility = Visibility.Collapsed;
                        FolderPage.Current.LV.Visibility = Visibility.Visible;
                        FolderPage.Current.Text = "";
                    }
                }
            }
        }


        private void SemanticZoom_ViewChangeStarted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (e.SourceItem.Item == null) return;

            e.DestinationItem = new SemanticZoomLocation { Item = e.SourceItem.Item };
        }

        private void ZoomedOutGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (e != null && e.ClickedItem != null && e.ClickedItem.GetType() == typeof(Item))
            {
                var item = e.ClickedItem as Item;
                if (item.Folder != null)
                    FolderPage.Current.MyFrame.Navigate(typeof(FileManagerPage1), item.Folder);
                else if (item.File != null)
                {
                    HelperUP.Output("sublistCount:" + subList.Count);
                    ((Frame)Window.Current.Content).Navigate(typeof(PlayerPage), new ItemWithItemSub { ItemSubList = subList, Item = item });
                }
            }
        }
    }
}
