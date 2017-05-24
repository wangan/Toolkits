using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Toolkits {


    public static class HttpCookieHelper {

        public static List<CookieItem> GetCookieList(string cookie) {
            List<CookieItem> cookielist = new List<CookieItem>();
            foreach (string item in cookie.Split(new string[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries)) {
                if (Regex.IsMatch(item, @"([\s\S]*?)=([\s\S]*?)$")) {
                    Match m = Regex.Match(item, @"([\s\S]*?)=([\s\S]*?)$");
                    cookielist.Add(new CookieItem() { Key = m.Groups[1].Value, Value = m.Groups[2].Value });
                }
            }
            return cookielist;
        }

        public static string GetCookieValue(string Key, string cookie) {
            foreach (CookieItem item in GetCookieList(cookie)) {
                if (item.Key == Key)
                    return item.Value;
            }
            return "";
        }
        public static string CookieFormat(string key, string value) {
            return string.Format("{0}={1};", key, value);
        }

    }

    public class CookieItem {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}

