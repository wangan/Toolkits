using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Configuration;

namespace Toolkits {


    /// <summary>
    /// 配置
    /// </summary>
    public class ConfigBase {
        public static T GetConfigInfo<T>(string name, T defaultValue) {
            try {
                string value = ConfigurationManager.AppSettings[name];
                if (null != value)
                    return (T)Convert.ChangeType(value, typeof(T));
            } catch { }

            return defaultValue;
        }

        public static T[] GetConfigInfo<T>(string name, T[] defaultValue) {
            try {
                string value = ConfigurationManager.AppSettings[name];
                if (null != value)
                    return value.Split(',').
                        Select(I => (T)Convert.ChangeType(I, typeof(T))).
                        ToArray();
            } catch { }

            return defaultValue;
        }

        public static Dictionary<string, T> GetConfigInfo<T>(string name, Dictionary<string, T> defaultValue) {
            try {
                string value = ConfigurationManager.AppSettings[name];
                if (null != value) {
                    var items = value.Split('|');
                    if (null != items && items.Length > 0) {
                        var res = new Dictionary<string, T>();
                        for (int i = 0; i < items.Length; i++) {
                            var cur = items[i].Split('@');
                            res.Add(cur[0], (T)Convert.ChangeType(cur[1], typeof(T)));
                        }

                        return res;
                    }
                }

            } catch { }

            return defaultValue;
        }
    }
}
