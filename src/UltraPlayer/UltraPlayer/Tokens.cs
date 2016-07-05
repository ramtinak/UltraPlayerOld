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

using System.Collections.Generic;
using System.IO;
using Windows.Foundation.Metadata;

namespace UltraPlayer
{
    public class MyTokenList : List<MToken>
    {
    }
    public class MToken
    {
        public string Token { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        [System.Runtime.Serialization.IgnoreDataMember()]
        public string OnWhat
        {
            get
            {
                string text = string.Empty;
                string on = Directory.GetDirectoryRoot(Path).Replace(":","").Replace("\\","").ToUpper();
                
                if (ApiInformation.IsApiContractPresent("Windows.Phone.PhoneContract", 1, 0))
                {
                    if (on == "C")
                        on = "Phone";
                    else if (on == "D")
                        on = "SD card";
                }

                text = string.Format("on {0}", on);
                return text; 
            }
        }
        public override string ToString()
        {
            if (Name != null) return Name.ToString();
            else return "";
        }
    }
}
