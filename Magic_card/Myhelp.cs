using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Threading;
namespace Magic_card
{
    class Myhelp
    {
        #region 取文本中间内容
        /// <summary>
        /// 取文本中间内容
        /// </summary>
        /// <param name="str">原文本</param>
        /// <param name="leftstr">左边文本</param>
        /// <param name="rightstr">右边文本</param>
        /// <returns>返回中间文本内容</returns>
        public static string getMid(string str, string leftstr, string rightstr)
        {
            int i = str.IndexOf(leftstr) + leftstr.Length;
            string temp = str.Substring(i, str.IndexOf(rightstr, i) - i);
            return temp;
        }
        #endregion
        #region 取文本右边内容
        /// <summary>
        /// 取文本右边内容
        /// </summary>
        /// <param name="str">原文本</param>
        /// <param name="s">左边内容</param>
        /// <returns></returns>
        public static string getRight(string str, string s)//取文本右边 有误
        {
            string temp = str.Substring(str.IndexOf(s) + s.Length, str.Length - (str.IndexOf(s) + s.Length));
            return temp;
        }
        #endregion
        #region 取g_tk
        /// <summary>
        /// 取出gbk
        /// </summary>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static string getGtk(string cookies)
        {
            long hash = 5381;
            string skey;
            try
            {
                skey = ("@" + getMid(cookies, "skey=@", ";"));
            }
            catch (Exception)
            {
                skey = ("@" + getRight(cookies, "skey=@"));

            }

            for (int i = 0; i < skey.Length; i++)
            {
                hash += (hash << 5) + skey[i];
            }
            return Convert.ToString(hash & 0x7fffffff);
        }
        #endregion
        #region 获取自己的信息
        public static string getIuin(string Cookies)
        {
            string iQQid = Convert.ToInt32(getMid(Cookies, "pt2gguin=o", ";")).ToString(); //取出cookies所有者的qq号
            return iQQid;
        }
        #endregion

        #region post
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url">post地址</param>
        /// <param name="postStr">post数据</param>
        /// <param name="cookies">提交时的Cookies</param>
        /// <returns></returns>
        public static string postHtml(string Url,string postStr,string cookies)
        {
            CookieContainer cc = new CookieContainer(); 
            string[] arrCookie = Mydata.Cookies.Split(';');//申请数组分割cookies
            foreach (string sCookie in arrCookie)
            {
                cc.SetCookies(new Uri(Url), sCookie);
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.CookieContainer = cc;
            System.Net.WebProxy proxy = new WebProxy("127.0.0.1", 1081);
            request.Proxy = proxy;
            //request.Proxy = null;
            request.Method = "POST";
            request.Accept = "*/*";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
            request.ContentLength = postStr.Length;
            string retStr = null;

            Stream myRequestStream = request.GetRequestStream(); 
            StreamWriter myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
            myStreamWriter.Write(postStr);
            myStreamWriter.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            retStr = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retStr;


        }

        #endregion
        #region get
        public static string getHtml(string Url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            System.Net.WebProxy proxy = new WebProxy("127.0.0.1", 1081);
            request.Proxy = proxy;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }
        #endregion
        public static string getIname(string postDataback)
        {
            string myinfoBack = null;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(postDataback);
            XmlNode xn = doc.SelectSingleNode("QQHOME");

            XmlNodeList xm = xn.SelectNodes("user");
            foreach (XmlNode item in xm)
            {
                if (item.LocalName == "user")
                {
                    XmlElement xe = (XmlElement)item;
                    myinfoBack = xe.GetAttribute("nick").ToString();
                }
            }
            return myinfoBack;

        }
    }
}
