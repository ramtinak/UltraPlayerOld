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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace UltraPlayer
{
    public class XamlExtensions : DependencyObject
    {
        /// <summary>
        /// Use a bool to trigger UIElement's Visibility value (instead of using a ValueConverter)
        /// Usage: extensions:XamlExtensions.IsVisible="True"
        /// </summary>
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.RegisterAttached(
            "IsVisible", typeof(bool), typeof(XamlExtensions), new PropertyMetadata(true, (o, e) =>
            {
                ((UIElement)o).Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
            }));

        public static void SetIsVisible(DependencyObject element, bool value)
        {
            element.SetValue(IsVisibleProperty, value);
        }

        public static bool GetIsVisible(DependencyObject element) => (bool)element.GetValue(IsVisibleProperty);

        /// <summary>
        /// Use a bool to trigger UIElement's Visibility value (instead of using a ValueConverter)
        /// Usage: extensions:XamlExtensions.IsHidden="True"
        /// </summary>
        public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.RegisterAttached(
            "IsHidden",
            typeof(bool),
            typeof(XamlExtensions),
            new PropertyMetadata(true, (o, e) =>
            {
                ((UIElement)o).Visibility = (bool)e.NewValue ? Visibility.Collapsed : Visibility.Visible;
            }));


        public static void SetIsHidden(DependencyObject element, bool value)
        {
            element.SetValue(IsHiddenProperty, value);
        }

        public static bool GetIsHidden(DependencyObject element) => (bool)element.GetValue(IsHiddenProperty);
    }
}
