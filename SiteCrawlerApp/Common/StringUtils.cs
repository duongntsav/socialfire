using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteCrawlerApp.Common
{
    public class StringUtils
    {

        private static Random random = new Random();

        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static bool ListHasDuplicates(List<string> lst)
        {
            bool result = false;
            var distinctList = lst.Distinct().ToList();
            result = (lst.Count() == distinctList.Count());
            return !result;
        }


        public static string DecodeUTF8(string utf8CodeString)
        {
            System.Text.Encoding utf_8 = System.Text.Encoding.UTF8;


            // Convert a string to utf-8 bytes.
            byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes(utf8CodeString);

            // Convert utf-8 bytes to a string.
            return System.Text.Encoding.UTF8.GetString(utf8Bytes);
        }

        public static string DecodeUTF82(string utf8CodeString)
        {
            byte[] bytes = Encoding.Default.GetBytes(utf8CodeString);
            var myString = Encoding.UTF8.GetString(bytes);
            return myString;
        }


        public static string DecodeFromUtf8(string utf8String)
        {
            // read the string as UTF-8 bytes.
            byte[] encodedBytes = Encoding.UTF8.GetBytes(utf8String);

            // convert them into unicode bytes.
            byte[] unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);

            // builds the converted string.
            return Encoding.Unicode.GetString(encodedBytes);
        }


        public static string Utf8ToUtf16(string utf8String)
        {
            // Get UTF-8 bytes by reading each byte with ANSI encoding
            byte[] utf8Bytes = Encoding.Default.GetBytes(utf8String);

            // Convert UTF-8 bytes to UTF-16 bytes
            byte[] utf16Bytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, utf8Bytes);

            // Return UTF-16 bytes as UTF-16 string
            return Encoding.Unicode.GetString(utf16Bytes);
        }

        // redudance path
        public static string removeRedudancePath(string imaagePath)
        {
            string suffix = "";
            if( imaagePath.ToLower().Contains(".png"))
            {
                suffix = ".png";
            } else if (imaagePath.ToLower().Contains(".jpg"))
            {
                suffix = ".jpg";
            } else if (imaagePath.ToLower().Contains(".jpeg"))
            {
                suffix = ".jpeg";
            } else if (imaagePath.ToLower().Contains(".gif")) {
                suffix = ".gif";
            }

            if( !string.IsNullOrEmpty( suffix))
            {
                int pos = imaagePath.ToLower().IndexOf( suffix);
                if(pos + suffix.Length< imaagePath.Length)
                    imaagePath = imaagePath.Substring(0, pos+ suffix.Length);
            }

            return imaagePath;
        }
    }
}
