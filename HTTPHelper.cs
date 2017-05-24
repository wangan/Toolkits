using System.IO;
using System.Net;
using System.Text;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toolkits {

    /// <summary>
    /// HTTP Helper
    /// </summary>
    public class HTTPHelper {

        /// <summary>
        /// GET HTTP RES & HEADER
        /// </summary>
        public static WebHeaderCollection GetHeader(out string res, string url, string method = "GET", int timeout = -1, string content = null, string contentType = null, List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return Process(out res, url, method, timeout, content, contentType, null, encoding, cfg);
        }

        /// <summary>
        /// GET
        /// </summary>
        public static string AdvGet(string url, int timeout = -1, List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "GET", timeout, null, null, cookies, encoding, cfg);
        }

        /// <summary>
        /// GET
        /// </summary>
        public static string Get(string url, int timeout = -1, string cookie = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "GET", timeout, null, null, cookie, encoding, cfg);
        }

        /// <summary>
        /// GET
        /// </summary>
        public static Task<string> GetAsync(string url, int timeout = -1, string content = null, string contentType = null, string cookie = null, Encoding encoding = null, Config cfg = null) {
            return ProcessAsync(url, "GET", timeout, content, contentType, cookie, encoding, cfg);
        }

        /// <summary>
        /// POST
        /// </summary>
        public static string Post(string url, int timeout = -1, string content = "", string contentType = "", List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "POST", timeout, content, contentType, cookies, encoding, cfg);
        }
                
        /// <summary>
        /// POST
        /// </summary>
        public static async Task<string> PostAsync(string url, int timeout = -1, string content = "", string contentType = "", List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return await ProcessAsync(url, "POST", timeout, content, contentType, cookies, encoding, cfg);
        }


        /// <summary>
        /// PUT
        /// </summary>
        public static string Put(string url, int timeout = -1, string content = "", string contentType = "", List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "PUT", timeout, content, contentType, cookies, encoding, cfg);
        }

        /// <summary>
        /// DELETE
        /// </summary>
        public static string Delete(string url, int timeout = -1, string content = "", string contentType = "", List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            return Process(url, "DELETE", timeout, content, contentType, cookies, encoding, cfg);
        }

        /// <summary>
        /// HTTP Config
        /// </summary>
        public sealed class Config {
            /// <summary>
            /// Ip
            /// </summary>
            public string Ip { get; set; }
            /// <summary>
            /// Port
            /// </summary>
            public int Port { get; set; }
            /// <summary>
            /// Determine whether or not use proxy
            /// </summary>
            public bool UseProxy { get; set; }
            /// <summary>
            /// Determine whether or not use custom endpoint 
            /// If there have many network interface,you can select one as the endpoint
            /// </summary>
            public bool ManualSetIp { get; set; }

            /// <summary>
            /// Http Headers
            /// </summary>
            public Dictionary<string, string> Headers { get; set; }

            /// <summary>
            /// If ManualSetIp is true ,return a selected endpoint
            /// </summary>
            /// <param name="servicePoint"></param>
            /// <param name="remoteEndPoint"></param>
            /// <param name="retryCount"></param>
            /// <returns></returns>
            public IPEndPoint BindIp(ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) {
                return new IPEndPoint(IPAddress.Parse(Ip), Port);
            }
        }

        /// <summary>
        /// Process the http request
        /// </summary>
        /// <returns></returns>
        internal static string Process(string url, string method = null, int timeout = -1, string content = null, string contentType = null, List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);

                if (null != cfg.Headers && cfg.Headers.Count > 0) {
                    foreach (var curKv in cfg.Headers) {
                        request.Headers[curKv.Key] = curKv.Value;
                    }
                }
            }

            request.Accept = "*/*";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //if (!string.IsNullOrEmpty(content))
            //    request.ContentLength = content.Length;

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (null != cookies && cookies.Count > 0) {
                request.CookieContainer = new CookieContainer();
                foreach (var cookie in cookies) {
                    request.CookieContainer.Add(cookie);
                }
            }

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    StreamReader sr = new StreamReader(inStream, encoding);
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Process the http request
        /// </summary>
        /// <returns></returns>
        internal static string Process(string url, string method = null, int timeout = -1, string content = null, string contentType = null, string cookie = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);

                if (null != cfg.Headers && cfg.Headers.Count > 0) {
                    foreach (var curKv in cfg.Headers) {
                        request.Headers[curKv.Key] = curKv.Value;
                    }
                }
            }

            request.Accept = "*/*";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //if (!string.IsNullOrEmpty(content))
            //    request.ContentLength = content.Length;

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (!string.IsNullOrEmpty(cookie))
                request.Headers.Add(HttpRequestHeader.Cookie, cookie);

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    Console.WriteLine(response.StatusCode);

                    StreamReader sr = new StreamReader(inStream, encoding);
                    return sr.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Process the http request
        /// </summary>
        /// <returns></returns>
        internal static WebHeaderCollection Process(out string res, string url, string method = null, int timeout = -1, string content = null, string contentType = null, List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);

                if (null != cfg.Headers && cfg.Headers.Count > 0) {
                    foreach (var curKv in cfg.Headers) {
                        request.Headers[curKv.Key] = curKv.Value;
                    }
                }
            }

            request.Accept = "*/*";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //if (!string.IsNullOrEmpty(content))
            //    request.ContentLength = content.Length;

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (null != cookies && cookies.Count > 0) {
                request.CookieContainer = new CookieContainer();
                foreach (var cookie in cookies) {
                    request.CookieContainer.Add(cookie);
                }
            }

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    StreamReader sr = new StreamReader(inStream, encoding);
                    res = sr.ReadToEnd();

                    return response.Headers;
                }
            }
        }

        private static async Task<string> ProcessAsync(string url, string method = null, int timeout = -1, string content = null, string contentType = null, List<Cookie> cookies = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);

                if (null != cfg.Headers && cfg.Headers.Count > 0) {
                    foreach (var curKv in cfg.Headers) {
                        request.Headers[curKv.Key] = curKv.Value;
                    }
                }
            }

            request.Accept = "*/*";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //if (!string.IsNullOrEmpty(content))
            //    request.ContentLength = content.Length;

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (null != cookies && cookies.Count > 0) {
                request.CookieContainer = new CookieContainer();
                foreach (var cookie in cookies) {
                    request.CookieContainer.Add(cookie);
                }
            }

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    await sw.WriteAsync(content);
                    await sw.FlushAsync();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    StreamReader sr = new StreamReader(inStream, encoding);
                    return await sr.ReadToEndAsync();
                }
            }
        }

        private static async Task<string> ProcessAsync(string url, string method = null, int timeout = -1, string content = null, string contentType = null, string cookie = null, Encoding encoding = null, Config cfg = null) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;

            if (null != cfg) {
                if (cfg.ManualSetIp)
                    request.ServicePoint.BindIPEndPointDelegate = new BindIPEndPoint(cfg.BindIp);

                if (cfg.UseProxy)
                    request.Proxy = new WebProxy(cfg.Ip, cfg.Port);

                if (null != cfg.Headers && cfg.Headers.Count > 0) {
                    foreach (var curKv in cfg.Headers) {
                        if (curKv.Key == "Referer") {
                            request.Referer = curKv.Value;
                        } else if (curKv.Key == "Host") {
                            request.Host = curKv.Value;
                        } else {
                            request.Headers[curKv.Key] = curKv.Value;
                        }
                    }
                }
            }

            request.Accept = "*/*";
            request.Headers["Accept-Encoding"] = "gzip, deflate, br";
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
            //if (!string.IsNullOrEmpty(content))
            //    request.ContentLength = content.Length;

            /* Set ContentType */
            if (!string.IsNullOrEmpty(contentType))
                request.ContentType = contentType;

            /* Set Cookie */
            if (!string.IsNullOrEmpty(cookie))
                request.Headers.Add(HttpRequestHeader.Cookie, cookie);

            /* Set Timeout */
            if (timeout > 0)
                request.Timeout = timeout;

            /* Set Encoding */
            if (encoding == null)
                encoding = Encoding.UTF8;

            /* Set Content*/
            if (!string.IsNullOrEmpty(content)) {
                using (Stream outStream = request.GetRequestStream()) {
                    StreamWriter sw = new StreamWriter(outStream);
                    await sw.WriteAsync(content);
                    await sw.FlushAsync();
                }
            }

            /* Response To String */
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) {
                using (Stream inStream = response.GetResponseStream()) {
                    StreamReader sr = new StreamReader(inStream, encoding);
                    return await sr.ReadToEndAsync();
                }
            }
        }



    }

    public sealed class HTTPDownloader<T> : WebClient {
        public T State { get; set; }
    }
}
