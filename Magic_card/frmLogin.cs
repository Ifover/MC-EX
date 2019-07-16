using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
namespace Magic_card
{
    public partial class frmLogin : Form
    {
        frmEx frm = new frmEx();

        public frmLogin()
        {

            InitializeComponent();

        }
        private void frmLogin_Load(object sender, EventArgs e)
        {
//如果是DEBUG模式则显示调试按钮,切记编译时切换为Release
#if DEBUG
    btnDebug.Visible = true;
#endif
            try
            {
                Myhelp.getHtml("http://appimg2.qq.com/card/index_v3.html");
            }
            catch (Exception)
            {

                MessageBox.Show("未连接网络或者其它什么原因,反正不关我的事\r再见!!!!!!!!!!!!!!!!!!!!", "出错辣!");
                Environment.Exit(0);
            }
            webLoign.ScriptErrorsSuppressed = true; //屏蔽脚本错误
            webLoign.IsWebBrowserContextMenuEnabled = false; //禁用右键菜单
            webLoign.Navigate("http://xui.ptlogin2.qq.com/cgi-bin/xlogin?appid=1600000084&s_url=http://appimg2.qq.com/card/index_v3.html");//跳转到登录页面

        }//载入窗口载入登录网站
        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }//用户取消登录,强制结束进程--退出按钮

        private void webLoign_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Console.WriteLine(webLoign.Url);

            if (!e.Url.Equals(webLoign.Url))
            {
                return;                //not actually complete, do nothing

            }
            if (webLoign.Url.ToString().IndexOf("http://appimg2.qq.com/card/index_v3.html") == 0)  //判断 登录成功后地址会变为游戏地址
            {
                webLoign.Visible = false;       //隐藏登录Browser
                lblLogin.Visible = true;        //显示登录成功标签
                lblLogin.Location = new Point(0, 0);    //登录标签坐标修改
                Rectangle rect = Screen.GetWorkingArea(this);     //获取屏幕物理范围
                this.Size = new Size(lblLogin.Width, lblLogin.Height);  //修改窗体大小
                this.Location = new Point(rect.Width / 2 - this.Width / 2, rect.Height / 2 - this.Height / 2);
                    //↑↑↑修改窗体位置(X:桌面总宽 / 2 - 窗体宽度 / 2 ; Y:桌面总高 / 2  - 窗体高度 / 2)
                Mydata.Cookies = webLoign.Document.Cookie;//登录成功后获取Cookie
                Console.WriteLine(webLoign.Url);
                webLoign.Navigate("about: blank");//将浏览器地址重置,释放内存.应该有效
                Application.DoEvents();
                //Thread.Sleep(10);
                this.Hide();
                frm.Show();
                //Debug.Write("No");

            }
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            Mydata.Cookies = Properties.Resources.Cookies;
            this.Hide();
            frm.Show();
        }

    }
}
