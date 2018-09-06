using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Unit
{
    public class HttpHelper
    {
        #region 加密POST请求，解密GET请求(优化中)
        /// <summary>
        /// Get解密请求(未完成)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="HttpWebResponseString"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool DecryptHttpGet(string url, out string HttpWebResponseString, int timeout)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebResponseString = "";
            bool result;
            try
            {
                HttpWebRequest base64ArraySize = WebRequest.Create(url) as HttpWebRequest;
                base64ArraySize.Method = "GET";
                base64ArraySize.ContentType = "application/x-www-form-urlencoded";
                base64ArraySize.UserAgent = HttpHelper.DefaultUserAgent;
                base64ArraySize.Timeout = timeout;
                HttpWebResponse charBuffer = base64ArraySize.GetResponse() as HttpWebResponse;
                HttpHelper.ReadHttpWebResponse(charBuffer);
                result = true;
            }
            catch (Exception s)
            {
                HttpWebResponseString = s.ToString();
                result = false;
            }
            return result;
        }

        /// <summary>
        /// POST加密请求(未完成)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Data"></param>
        /// <param name="HttpWebResponseString"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool EncryptHttpPost(string url, byte[] Data, out string HttpWebResponseString, int timeout)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebResponseString = "";
            Stream bytes = null;
            bool result;
            try
            {
                HttpWebRequest charBuffer = WebRequest.Create(url) as HttpWebRequest;
                charBuffer.Method = "POST";
                charBuffer.ContentType = "application/x-www-form-urlencoded";
                charBuffer.UserAgent = HttpHelper.DefaultUserAgent;
                charBuffer.Timeout = timeout;
                if (Data != null)
                {
                    HttpHelper.requestEncoding.GetString(Data);
                    bytes = charBuffer.GetRequestStream();
                    bytes.Write(Data, 0, Data.Length);
                }
                HttpWebResponse httpWebResponse = charBuffer.GetResponse() as HttpWebResponse;
                HttpWebResponseString = HttpHelper.ReadHttpWebResponse(httpWebResponse);
                result = true;
            }
            catch (Exception ex)
            {
                HttpWebResponseString = ex.ToString();
                result = false;
            }
            finally
            {
                if (bytes != null)
                {
                    bytes.Close();
                }
            }
            return result;
        }
        #endregion


        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="HttpWebResponseString"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool HttpGet(string url, out string HttpWebResponseString, int timeout)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebResponseString = "";
            bool result;
            try
            {
                HttpWebRequest base64ArraySize = WebRequest.Create(url) as HttpWebRequest;
                base64ArraySize.Method = "GET";
                base64ArraySize.ContentType = "application/x-www-form-urlencoded";
                base64ArraySize.UserAgent = HttpHelper.DefaultUserAgent;
                base64ArraySize.Timeout = timeout;
                HttpWebResponse charBuffer = base64ArraySize.GetResponse() as HttpWebResponse;
                HttpWebResponseString = HttpHelper.ReadHttpWebResponse(charBuffer);
                result = true;
            }
            catch (Exception s)
            {
                HttpWebResponseString = s.ToString();
                result = false;
            }
            return result;
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Data"></param>
        /// <param name="HttpWebResponseString"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool HttpPost(string url, byte[] Data, out string HttpWebResponseString, int timeout)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            HttpWebResponseString = "";
            Stream bytes = null;
            bool result;
            try
            {
                HttpWebRequest charBuffer = WebRequest.Create(url) as HttpWebRequest;
                charBuffer.Method = "POST";
                charBuffer.ContentType = "application/x-www-form-urlencoded";
                charBuffer.UserAgent = HttpHelper.DefaultUserAgent;
                charBuffer.Timeout = timeout;
                if (Data != null)
                {
                    HttpHelper.requestEncoding.GetString(Data);
                    bytes = charBuffer.GetRequestStream();
                    bytes.Write(Data, 0, Data.Length);
                }
                HttpWebResponse httpWebResponse = charBuffer.GetResponse() as HttpWebResponse;
                HttpWebResponseString = HttpHelper.ReadHttpWebResponse(httpWebResponse);
                result = true;
            }
            catch (Exception ex)
            {
                HttpWebResponseString = ex.ToString();
                result = false;
            }
            finally
            {
                if (bytes != null)
                {
                    bytes.Close();
                }
            }
            return result;
        }

        // Token: 0x06000003 RID: 3 RVA: 0x00002234 File Offset: 0x00000434
        public static string ReadHttpWebResponse(HttpWebResponse HttpWebResponse)
        {
            Stream stream = null;
            StreamReader streamReader = null;
            string result = null;
            try
            {
                stream = HttpWebResponse.GetResponseStream();
                streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
                result = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }
                if (HttpWebResponse != null)
                {
                    HttpWebResponse.Close();
                }
            }
            return result;
        }

        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        private static Encoding requestEncoding = Encoding.UTF8;
    }

}
