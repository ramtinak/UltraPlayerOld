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
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace UltraPlayer
{
    public class ItemWithItemSub
    {
        public Item Item { get; set; }
        public ItemSub ItemSub { get; set; }
        public List<ItemSub> ItemSubList { get; set; }
    }

    public class ItemSub
    {
        public string Name { get; set; }
        public string Path { get; set; }
        private StorageFile file_ = null;

        [System.Runtime.Serialization.IgnoreDataMember()]
        public StorageFile File
        {
            get { return file_; }
            set { file_ = value; SetInfoByFile(true); }
        }

        private void SetInfoByFile(bool v)
        {
            Name = File.Name;
            Path = File.Path;
        }
    }

    public class Item
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Duration { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }
        public string FolderCount { get; set; }
        public string DateModified { get; set; }
        public string Key
        {
            get
            {
                return Name.Substring(0, 1).ToUpper();
            }
        }


        private StorageFile file_ = null;
        private StorageFolder folder_ = null;

        [System.Runtime.Serialization.IgnoreDataMember()]
        public BitmapImage Thumbnail { get; set; }
        [System.Runtime.Serialization.IgnoreDataMember()]
        public StorageFile File
        {
            get { return file_; }
            set { file_ = value; SetInfoByFile(true); }
        }
        [System.Runtime.Serialization.IgnoreDataMember()]
        public StorageFolder Folder
        {
            get { return folder_; }
            set { folder_ = value; SetInfoByFolder(true); }
        }



        public async void SetThumbnail()
        {
            try
            {
                StorageItemThumbnail thumb = null;
                if (File != null)
                    thumb = await File.GetThumbnailAsync(ThumbnailMode.VideosView);

                if (Folder != null)
                {
                    BitmapImage img2 = new BitmapImage(new Uri("ms-appx:///Assets/folder.png"));
                    Thumbnail = img2;
                    return;
                }
                if (thumb == null)
                    return;
                BitmapImage img = new BitmapImage();
                img.SetSource(thumb.AsStream().AsRandomAccessStream());
                Thumbnail = img;
            }
            catch (Exception ex){ Debug.WriteLine("SetThumbnail ex: "+ ex.Message); }
        }
        public async Task SetThumbnailAsync()
        {
            try
            {
                StorageItemThumbnail thumb = null;
                if (File != null)
                    thumb = await File.GetThumbnailAsync(ThumbnailMode.VideosView);
                if (Folder != null)
                {
                    BitmapImage img2 = new BitmapImage(new Uri("ms-appx:///Assets/folder.png"));
                    Thumbnail = img2;
                    return;
                }
                if (thumb == null)
                    return;
                BitmapImage img = new BitmapImage();
                img.SetSource(thumb.AsStream().AsRandomAccessStream());
                Thumbnail = img;
            }
            catch (Exception ex) { Debug.WriteLine("SetThumbnailAsync ex: " + ex.Message); }
        }

        public async void SetInfoByFolder(bool setThumnail = false)
        {
            if (Folder == null)
                return;
            Name = Folder.Name;
            Path = Folder.Path;

            if (setThumnail)
                SetThumbnail();
            Duration = string.Empty;
            Type = string.Empty;
            Size = string.Empty;
            try
            {
                FolderCount = (await Folder.GetFilesAsync()).Count.ToString() + " videos";
                Duration = FolderCount;
            }
            catch (Exception ex) { Debug.Write("SetInfoByFolder ex: " + ex.Message); }

        }
        public async Task SetInfoByFolderAsync(bool setThumnail = false)
        {
            if (Folder == null)
                return;
            Name = Folder.Name;
            Path = Folder.Path;

            if (setThumnail)
                SetThumbnail();
            Duration = string.Empty;
            Type = string.Empty;
            Size = string.Empty;
            try
            {
                FolderCount = (await Folder.GetFilesAsync()).Count.ToString() + " videos";
                Duration = FolderCount;
            }
            catch (Exception ex) { Debug.Write("SetInfoByFolderAsync ex: " + ex.Message); }
        }




        public /*async*/ void SetInfoByFile(bool setThumnail = false)
        {
            if (File == null)
                return;
            Name = File.Name;
            Path = File.Path;

            BitmapImage img2 = new BitmapImage(new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png"));
            Thumbnail = img2;

            Type = File.FileType;
            FolderCount = string.Empty;
        }
        public async Task SetInfoByFileAsync(bool setThumnail = false)
        {
            if (File == null)
                return;
            Name = File.Name;
            Path = File.Path;

            if (setThumnail)
                SetThumbnail();
            try
            {
                var info = await File.Properties.GetVideoPropertiesAsync();
                Duration = string.Format("{0}:{1}:{2}", info.Duration.Hours.ToString("00"),
                                  info.Duration.Minutes.ToString("00"), info.Duration.Seconds.ToString("00"));
            }
            catch (Exception ex) { Debug.WriteLine("SetInfoByFile.GetVideoPropertiesAsync ex: " + ex.Message); }

            try
            {
                BasicProperties basicProperties = await File.GetBasicPropertiesAsync();
                Size = HelperUP.CalculateBytes(basicProperties.Size);
                DateModified = basicProperties.DateModified.ToString();
            }
            catch (Exception ex) { Debug.WriteLine("SetInfoByFile.GetBasicPropertiesAsync ex: " + ex.Message); }


            Type = File.FileType;
            FolderCount = string.Empty;
        }
        
    }

    public class ItemCategory
    {
        public string Type { get; set; }
        public string Key
        {
            get
            {
                return CategoryName.Substring(0, 1).ToUpper();

            }
        }
        public string CategoryName { get; set; }
        public List<Item> Items { get; set; }
    }
}
