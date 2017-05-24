using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Toolkits {
    public class Util {

        public static string Hash(string str, bool var_length = false) {
            MD5 md5 = new MD5CryptoServiceProvider();
            var md5Buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            if (var_length) {
                string res = "";
                foreach (var cur in md5Buffer) {
                    res += cur.ToString("X");
                }

                return res;
            } else {
                return BitConverter.ToString(md5Buffer).Replace("-", "");
            }
        }

        public static char ASCII() {
            var num = RandomNum(char.MinValue, char.MaxValue);
            char c = Convert.ToChar(num);
            if (!char.IsControl(c) && char.IsLetterOrDigit(c)) {
                return c;
            } else {
                return ASCII();
            }
        }

        /// <summary>
        /// Get a random number between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomNum(int min, int max) {
            return (new Random(GuidAsInt()).Next(min, max));
        }

        /// <summary>
        /// get an random seed
        /// </summary>
        public static int GuidAsInt() {
            return BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);
        }

        public static string SHA1(byte[] buffer) {
            using (SHA1Managed sha = new SHA1Managed()) {
                byte[] checksum = sha.ComputeHash(buffer);
                string checksumStr = BitConverter.ToString(checksum).Replace("-", string.Empty);

                return checksumStr;
            }
        }
    }
}
