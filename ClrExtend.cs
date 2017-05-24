using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Toolkits {
    public static class ClrExtend {
        /// <summary>
        /// System reserved names
        /// </summary>
        static string[] Reserved = new string[] { "CON", "PRN", "AUX", "NUL", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9" };

        static DateTime _BaseDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static bool VerValid(this string ver, out Version version) {
            version = null;
            if (Version.TryParse(ver, out version) && null != version) {
                return true;
            }

            return false;
        }

        public static long Timestamp(this DateTime cur) {
            return (long)cur.Subtract(_BaseDateTime).TotalSeconds;
        }

        public static DateTime DateTime(this long timestamp) {
            return _BaseDateTime.AddSeconds(timestamp).ToLocalTime();
        }

        /// <summary>
        /// Check the path is valid
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ValidPath(this string path) {
            var fileName = Path.GetFileNameWithoutExtension(path);
            if (!string.IsNullOrEmpty(fileName)) {
                if (Reserved.Contains(fileName)) {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check the filename is valid
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ValidFileName(this string fileName) {
            if (!string.IsNullOrEmpty(fileName)) {
                if (Reserved.Contains(fileName)) {
                    return false;
                }
            }

            return true;
        }
        
    }
}
