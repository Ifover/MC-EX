using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magic_card
{
    public  class Mydata
    {
        public static string Cookies=Properties.Resources.Cookies;
        public static string Gtk;
        public static string Iuin;
        public static string Uuin;
        public static bool isChanging;
        public const string HideStr = "<\r\n<\r\n<\r\n<\r\n<\r\n<\r\n收\r\n起\r\n选\r\n择\r\n<\r\n<\r\n<\r\n<\r\n<\r\n<";
        public const string ShowStr = ">\r\n>\r\n>\r\n>\r\n>\r\n>\r\n选\r\n择\r\n套\r\n卡\r\n>\r\n>\r\n>\r\n>\r\n>\r\n>";
        public struct MyItem
        {
            public string Name;
            public int Id;
            public string Sit;
            public override string ToString()
            {
                return Name;
            }
        }
        public static string Card_User(string UIN)
        {
            return Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_user_mainpage?g_tk=" + Gtk, UIN, Cookies);
        }//该用户信息
        public static string  Card_Excg(string postdata)
        {
            return Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_user_exchangecard?g_tk=" + Gtk, postdata, Cookies);
        }//换卡
        public static void Card_Buy()
        {
            Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_market_npc_buy?g_tk=" + Gtk, "", Cookies);
        }//买卡
        public static void Card_Sell()
        {
            Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_market_npc_sell?g_tk=" + Gtk, "", Cookies);
        }//卖卡
        public static string Card_Fsid(string ThemeId)
        {
            return Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_user_theme_list?g_tk=" + Gtk, "tid=" + ThemeId + " &uin="+Iuin, Cookies);
        }//获取卡友



    }
}
