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
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UltraPlayer
{
    public partial class FolderPage
    {
        public void ProtocolNavigator(IActivatedEventArgs e)
        {
            // handle uri...
            var args = e as ProtocolActivatedEventArgs;
            var uri = args.Uri.AbsoluteUri;
        }

        public void FileNavigator(FileActivatedEventArgs e)
        {
            // handle files
        }
    }
    public partial class App
    {
        protected override void OnFileActivated(FileActivatedEventArgs e)
        {
            Frame rootFrame = CreateRootFrame();

            if (rootFrame.Content == null)
            {
                if (!rootFrame.Navigate(typeof(FolderPage)))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            var p = rootFrame.Content as FolderPage;
            p.FileNavigator(e);

            Window.Current.Activate();
        }


        protected override void OnActivated(IActivatedEventArgs e)
        {
            if (e.Kind == ActivationKind.Protocol)
            {
                Frame rootFrame = CreateRootFrame();

                if (rootFrame.Content == null)
                {
                    if (!rootFrame.Navigate(typeof(FolderPage)))
                    {
                        throw new Exception("Failed to create initial page");
                    }
                }

                var p = rootFrame.Content as FolderPage;
                p.ProtocolNavigator(e);

                Window.Current.Activate();
            }
        }
    }
}
