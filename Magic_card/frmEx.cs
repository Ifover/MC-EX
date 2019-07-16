using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using System.Linq;

namespace Magic_card
{
    public partial class frmEx : Form
    {
        int Chose_Exchange_Theme = 0;//选中的要交换的套卡
        int iCount = 0; //已搜索卡友计数
        Dictionary<int, ThemeTemplet> _themeDic = new Dictionary<int, ThemeTemplet>();
        Dictionary<int, CardTemplet> _cardDic = new Dictionary<int, CardTemplet>();
        Stopwatch stw = new Stopwatch();
        //DataSet ds = new DataSet();
        public delegate void MyMethod();
        public frmEx()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }
        private void FrmEx_Load(object sender, EventArgs e)
        {
            #region 获取信息の块(每次登录都会重新获取)//待编辑
            Mydata.Gtk = Myhelp.getGtk(Mydata.Cookies); //gtk
            Mydata.isChanging = false;
            Mydata.Iuin = Myhelp.getIuin(Mydata.Cookies);//登录者QQ
            string Iname = Myhelp.getIname(Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_user_mainpage?g_tk=" + Mydata.Gtk, "uin=" + Mydata.Iuin, Mydata.Cookies));
            //Debug.WriteLine(Iname);
            tsmiUser.Text = Iname == "" ? Mydata.Iuin : Iname; //如果有ID就ID,没有就用QQ
            gBoxI.Text = Iname == "" ? Mydata.Iuin : Iname;//如果有ID就ID,没有就用QQ
            gBoxI.Text += ".卡箱 [点此刷新]";
            #endregion
            gboxThemes.Location = new Point(-190, 27);
            ThreadStart myThreaddelegate = new ThreadStart(Load_Card);
            Thread tre = new Thread(myThreaddelegate)
            {
                IsBackground = true   // 设置为后台线程，在主窗口退出时线程强制退出
            };
            tre.Start();

        }//载入窗口时
        private void DtsLogout_Click(object sender, EventArgs e)
        {
            this.Hide();
            frmLogin frm = new frmLogin();
            frm.Show();
        }//切换帐号

        #region 应该不会去动的东西
        private void frmEx_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }//关闭程序时结束整个程序的进程
        private void cbTop_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = cbTop.Checked;
        }//置顶
        private void lblHide_Click(object sender, EventArgs e)
        {
            lblHide.Enabled = false;
            if (lblHide.Text == Mydata.ShowStr)
            {
                Application.DoEvents();
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(5);
                    gboxThemes.Location = new Point(gboxThemes.Location.X + 10, 27);

                }
                lblHide.Text = Mydata.HideStr;
                lblHide.Enabled = true;
            }
            else
            {
                Application.DoEvents();
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(5);
                    gboxThemes.Location = new Point(gboxThemes.Location.X - 10, 27);

                }
                lblHide.Text = Mydata.ShowStr;
                lblHide.Enabled = true;

            }
        }//显示和收起[选择套卡]
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (tssState.Text == "卡片信息加载完毕!!!")
            {
                I_boxInfo();
                timer1.Enabled = false;
            }
        }//等待卡片加载完毕后刷新我的卡箱
        #endregion
        #region 套卡容器(选择套卡后)
        private void lblSH(int id)
        {
            if (id == -1) return;
            Chose_Exchange_Theme = id;
            Mydata.MyItem item;
            lbCards.Items.Clear();
            string[] themeCards = _themeDic[id].Cards.Split(','); //分割套卡内的卡片ID
            for (int i = 0; i < themeCards.Length; i++)
            {
                item.Name = _cardDic[Convert.ToInt32(themeCards[i])].Name + ":" + _cardDic[Convert.ToInt32(themeCards[i])].Price; //  名字:价格
                item.Id = Convert.ToInt32(themeCards[i]); //ID
                item.Sit = null;
                lbCards.Items.Add((object)item);

            }

            lblHide.Enabled = false;
            lblHide.Text = Mydata.ShowStr;
            for (int i = 0; i < 20; i++)
            {
                Thread.Sleep(1);
                Application.DoEvents();
                gboxThemes.Location = new Point(gboxThemes.Location.X - 10, 27);
            }
            lblHide.Enabled = true;

        }//在列表框显示卡片
        private void tvFX_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int tvFxTag = Convert.ToInt32(tvFX.SelectedNode.Tag.ToString());
            lblSH(tvFxTag);
        }//发行套卡
        private void tvXJ_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int tvXJTag = Convert.ToInt32(tvXJ.SelectedNode.Tag.ToString());
            lblSH(tvXJTag);
        }//下架套卡
        private void tvSK_AfterSelect(object sender, TreeViewEventArgs e)
        {
            int tvSKTag = Convert.ToInt32(tvSK.SelectedNode.Tag.ToString());
            lblSH(tvSKTag);
        }//闪卡
        #endregion
        #region 卡箱刷新
        private void I_boxInfo()
        {
            lvIbox.Items.Clear();
            lvIbox.BeginUpdate();
            //Debug.Write(groupBox1.Location);
            string IinfoTemp = Mydata.Card_User("uin=" + Mydata.Iuin); //提交内容将返回的临时文本保存在postdata
            //Debug.Write(IinfoTemp);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(IinfoTemp);
            XmlNode xn = doc.SelectSingleNode("QQHOME");
            XmlNodeList xnl = xn.ChildNodes;
            ListViewGroup Exbox_lvg = new ListViewGroup
            {
                Header = "换卡箱",  //设置组的标题。 
                HeaderAlignment = HorizontalAlignment.Left
            };  //创建换卡箱分组 
            lvIbox.Groups.Add(Exbox_lvg);

            ListViewGroup Ofbox_lvg = new ListViewGroup
            {
                Header = "保险箱",
                HeaderAlignment = HorizontalAlignment.Left   //组标题居中对齐 
            };  //创建保险箱分组 
            lvIbox.Groups.Add(Ofbox_lvg);
            this.lvIbox.ShowGroups = true;

            foreach (XmlNode xnx in xnl)
            {
                #region 换卡箱
                if (xnx.LocalName == "changebox")
                {
                    XmlElement xe = (XmlElement)xnx;
                    XmlNodeList xnls = xe.ChildNodes;
                    Mydata.MyItem mi;
                    foreach (XmlElement xcx in xnls)
                    {
                        int status = Convert.ToInt32(xcx.GetAttribute("status"));//不等于0,该卡不存在
                        if (status == 0)
                        {
                            string slot = xcx.GetAttribute("slot"); //卡位
                            int unlock = Convert.ToInt32(xcx.GetAttribute("unlock"));//卡锁
                            int id = Convert.ToInt32(xcx.GetAttribute("id"));  //ID
                            int type = Convert.ToInt32(xcx.GetAttribute("type"));
                            int st = Convert.ToInt32(xcx.GetAttribute("st"));
                            //string cardLock = unlock > 0 ? "[锁]" : null;

                            mi.Id = id;
                            mi.Name = _cardDic[id].Name;
                            mi.Sit = slot;

                            ListViewItem lvi = new ListViewItem
                            {
                                Text = _cardDic[id].Name
                            };
                            lvi.SubItems.Add(_cardDic[id].Price.ToString());
                            lvi.SubItems.Add(_themeDic[_cardDic[id].ThemeID].Name);
                            lvi.ForeColor = unlock == 0 ? Color.Black : Color.Red;

                            Exbox_lvg.Items.Add(lvi);
                            this.lvIbox.Items.Add(lvi).Tag = mi;

                        }
                    }
                    int myMaxexcur = Convert.ToInt32(xe.GetAttribute("cur")); //换卡箱最大箱位
                    Exbox_lvg.Header = "换卡箱  " + Exbox_lvg.Items.Count + "/" + myMaxexcur.ToString();  //设置组的标题。 

                }
                #endregion
                #region 保险箱
                if (xnx.LocalName == "storebox")
                {
                    XmlElement xe = (XmlElement)xnx;
                    XmlNodeList xnls = xe.ChildNodes;
                    Mydata.MyItem mi;
                    foreach (XmlElement xcx in xnls)
                    {
                        if (xcx.LocalName == "card")
                        {
                            string slot = xcx.GetAttribute("slot");
                            int id = Convert.ToInt32(xcx.GetAttribute("id"));
                            int type = Convert.ToInt32(xcx.GetAttribute("type"));
                            int st = Convert.ToInt32(xcx.GetAttribute("st"));

                            mi.Name = _cardDic[id].Name + ":" + _cardDic[id].Price;
                            mi.Id = id;
                            mi.Sit = slot;

                            ListViewItem lvi = new ListViewItem
                            {
                                Text = _cardDic[id].Name
                            };
                            lvi.SubItems.Add(_cardDic[id].Price.ToString());
                            lvi.SubItems.Add(_themeDic[_cardDic[id].ThemeID].Name);

                            Ofbox_lvg.Items.Add(lvi);
                            this.lvIbox.Items.Add(lvi).Tag = (object)mi;
                        }
                    }
                    int myMaxofcur = Convert.ToInt32(xe.GetAttribute("cur"));//保险箱最大箱位
                    Ofbox_lvg.Header = "保险箱  " + Ofbox_lvg.Items.Count + "/" + myMaxofcur.ToString();  //设置组的标题。 
                }
                #endregion
            }
            lvIbox.EndUpdate();
            tssEXchange.Text = null;
        }//获取我的卡片信息
        private void U_boxInfo()
        {
            lvUbox.Items.Clear();   //清空对方卡箱(安全起见)
            lvUbox.BeginUpdate();
            gBoxU.Text = Mydata.Uuin + ".换卡箱 [点我刷新]"; //可刷新对方卡箱
            string IinfoTemp = Mydata.Card_User("opuin=" + Mydata.Uuin + "&uin =" + Mydata.Iuin); //获取对方的信息保存在IinfoTemp
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(IinfoTemp);
            XmlNode xn = doc.SelectSingleNode("QQHOME");
            XmlNodeList xnl = xn.ChildNodes;
            Mydata.MyItem mi;
            foreach (XmlNode xnx in xnl)
            {
                if (xnx.LocalName == "changebox")
                {
                    XmlElement xe = (XmlElement)xnx;
                    XmlNodeList xnls = xe.ChildNodes;
                    foreach (XmlElement xcx in xnls)
                    {
                        int status = Convert.ToInt32(xcx.GetAttribute("status")); //不等于0,该卡不存在
                        if (status == 0)
                        {
                            int id = Convert.ToInt32(xcx.GetAttribute("id")); //ID
                            string slot = xcx.GetAttribute("slot"); //卡位
                            int unlock = Convert.ToInt32(xcx.GetAttribute("unlock")); //卡锁
                            int type = Convert.ToInt32(xcx.GetAttribute("type"));
                            int st = Convert.ToInt32(xcx.GetAttribute("st"));

                            mi.Id = id;
                            mi.Name = _cardDic[id].Name;
                            mi.Sit = slot;

                            ListViewItem lvi = new ListViewItem
                            {
                                Text = _cardDic[id].Name // + cardLock;
                            };
                            lvi.SubItems.Add(_cardDic[id].Price.ToString());
                            lvi.SubItems.Add(_themeDic[_cardDic[id].ThemeID].Name);
                            lvi.ForeColor = unlock == 0 ? Color.Black : Color.Red;
                            this.lvUbox.Items.Add(lvi).Tag = mi;
                        }
                    }
                }
            }
            lvUbox.EndUpdate();
            tssEXchange.Text = null;
        }//获取卡友卡片信息
        private void gBoxI_Click(object sender, EventArgs e)
        {
            I_boxInfo();
        }//我的卡箱刷新
        private void gBoxU_Click(object sender, EventArgs e)
        {
            U_boxInfo();
        }//对方卡箱刷新

        #endregion
        #region 线程启动停止
        private void btnStartSearch_Click(object sender, EventArgs e)
        {

            if (btnStartSearch.Text == "开始搜索")
            {
                stw.Reset();
                stw.Start();

                Mydata.isChanging = false;
                threadStart();
                btnStartSearch.Text = "停止搜索";
            }
            else
            {
                stw.Stop();
                Debug.Write("用时:" + stw.Elapsed.ToString() + "\r\n");
                //tbTest.Text = "用时:" + stw.Elapsed.ToString();
                threadStop();
                btnStartSearch.Text = "开始搜索";
            }
        }
        Thread[] findThreads=new Thread[2];
        private void threadStart()
        {
            this.Size = new Size(440, 408);
            cbTop.Left = 366;
            findThreads[0] = new Thread(new ThreadStart(Get_CardFriends));
            findThreads[1] = new Thread(new ThreadStart(Get_CardFriends));
            findThreads[0].Start();
            findThreads[1].Start();
        }
        private void threadStop()
        {
            findThreads[0].Abort();
            findThreads[1].Abort();
        }
        #endregion
        private void Load_Card()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(327, 93);
            Rectangle rect = Screen.GetWorkingArea(this);     //获取显示器的分辨率
            this.Location = new Point(rect.Width / 2 - this.Width / 2, rect.Height / 2 - this.Height / 2);
            lblState.Location = new Point(0, 0);
            #region 卡片信息处理
            string CARD_IFFO = Myhelp.getHtml("http://appimg2.qq.com/card/mk/card_info_v3.js");  //有网时
            string CARD_MINI = Myhelp.postHtml("http://card.show.qq.com/cgi-bin/card_mini_get?g_tk=" + Mydata.Gtk, "uin=" + Mydata.Iuin, Mydata.Cookies);//已收集的套卡

            Regex regStr = new Regex("id=\"(.*)\" num=\"(.*)\"", RegexOptions.IgnoreCase); //已收集套卡信息的正则
            MatchCollection mats = regStr.Matches(CARD_MINI);


            regStr = new Regex(@"(\d{2,4}),'(.*|[\(].*[\)])',(\d),(\d{10}),.*\[(.*)\],(.),.*\d{10},(\d{1,10}),", RegexOptions.IgnoreCase); //套卡信息的正则
            MatchCollection mat = regStr.Matches(CARD_IFFO);
            for (int i = 0; i < mat.Count; i++)
            {
                int id = Convert.ToInt32(mat[i].Groups[1].ToString());      //--------------------
                string name = mat[i].Groups[2].ToString();
                int diff = Convert.ToInt32(mat[i].Groups[3].ToString());
                int time = Convert.ToInt32(mat[i].Groups[4].ToString());    //  各种的数据
                string cards = mat[i].Groups[5].ToString();
                int type = Convert.ToInt32(mat[i].Groups[6].ToString());
                int offtime = Convert.ToInt32(mat[i].Groups[7].ToString());//----------------------
                MyMethod mmd = delegate ()
                  {
                      #region 加载套卡 必须优化,代码乱的要死
                      bool xxx = false;
                      switch (type)
                      {

                          case 0:
                          case 2:
                              #region
                              xxx = false;
                              for (int x = 0; x < mats.Count; x++)
                              {
                                  if (id == Convert.ToInt32(mats[x].Groups[1].ToString()))
                                  {
                                      xxx = true;
                                      break;
                                  }
                              }
                              if (xxx)
                              {
                                  tvFX.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                                  tvFX.Nodes[diff.ToString()].Nodes[0].BackColor = Color.FromArgb(128, 255, 128);
                              }
                              else
                              {
                                  tvFX.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                              }

                              #endregion
                              break;
                          case 1:
                          case 5:
                              #region
                              xxx = false;
                              for (int x = 0; x < mats.Count; x++)
                              {
                                  if (id == Convert.ToInt32(mats[x].Groups[1].ToString()))
                                  {
                                      xxx = true;
                                      break;
                                  }
                              }
                              if (xxx)
                              {
                                  tvXJ.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                                  tvXJ.Nodes[diff.ToString()].Nodes[0].BackColor = Color.FromArgb(128, 255, 128);
                              }
                              else
                              {
                                  tvXJ.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                              }

                              #endregion
                              break;
                          case 9:
                              #region
                              xxx = false;
                              for (int x = 0; x < mats.Count; x++)
                              {
                                  if (id == Convert.ToInt32(mats[x].Groups[1].ToString()))
                                  {
                                      xxx = true;
                                      break;
                                  }
                              }
                              if (xxx)
                              {
                                  tvSK.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                                  tvSK.Nodes[diff.ToString()].Nodes[0].BackColor = Color.FromArgb(128, 255, 128);
                              }
                              else
                              {
                                  tvSK.Nodes[diff.ToString()].Nodes.Insert(0, name).Tag = id; //添加子节点的同时添加它的数据(ID)
                              }

                              #endregion
                              break;
                          default:
                              break;
                      }
                      #endregion
                  };
                Invoke(mmd);
                _themeDic.Add(id, new ThemeTemplet(id, name, diff, time, cards, type, offtime)); //将信息存入该字典中
                //ds.Tables["Theme"].Rows.Add(new object[] { id, name });
            }
            regStr = new Regex(@"\[(.*),(.*),'(.*)',(\d{2,4}),", RegexOptions.IgnoreCase);//卡片信息的正则
            mat = regStr.Matches(CARD_IFFO);
            for (int i = 0; i < mat.Count; i++)
            {
                int id = Convert.ToInt32(mat[i].Groups[1].ToString());
                int themeid = Convert.ToInt32(mat[i].Groups[2].ToString());
                string name = mat[i].Groups[3].ToString();
                int price = Convert.ToInt32(mat[i].Groups[4].ToString());
                _cardDic.Add(id, new CardTemplet(id, themeid, name, price));
            }


            tssState.BackColor = Color.FromArgb(128, 255, 128);
            tssState.Text = "卡片信息加载完毕!!!";
            lblState.Visible = false;
            #endregion
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.Size = new Size(440, 408);
            cbTop.Left = 366;
            this.Location = new Point(rect.Width / 2 - this.Width / 2, rect.Height / 2 - this.Height / 2);
            lblHide.Enabled = true;
            btnTst.Enabled = true;
            Console.Beep();
            Debug.WriteLine(groupBox1.Location.Y);

        }//获取和载入卡片
        #region 卡友的各种计算
        private void Get_CardFriends()
        {
            while (true)
            {
                string Card_Fsid = Mydata.Card_Fsid(Chose_Exchange_Theme.ToString());
                //获取卡友[开始]-------------------------------
                Regex regStr = new Regex(@"(?<!0\d*)[1-9]\d{4,10}", RegexOptions.IgnoreCase); //100个卡友的正则
                MatchCollection mat = regStr.Matches(Card_Fsid);

                for (int i = 0; i < mat.Count; i++)
                {
                    tssState.Text = "累计搜索 [" + iCount++.ToString() + "]";  //计数
                    tssState.BackColor = Color.FromKnownColor(KnownColor.Control);
                    string IinfoTemp = Mydata.Card_User("opuin=" + mat[i] + "&uin =" + Mydata.Iuin); //获取对方的信息保存在IinfoTemp
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(IinfoTemp);
                    XmlNode x1 = doc.SelectSingleNode("QQHOME");
                    XmlNodeList x2 = x1.ChildNodes;
                    foreach (XmlNode x3 in x2)
                    {
                        if (x3.LocalName == "changebox" && Get_Exch(x3.Attributes[3].Value)) 
                        {
                            //Debug.WriteLine(x3.Attributes[3].Value);
                            XmlElement xChangebox = (XmlElement)x3;
                            XmlNodeList xnls = xChangebox.ChildNodes;
                            foreach (XmlElement xcx in xnls)
                            {
                                int status = Convert.ToInt32(xcx.GetAttribute("status")); //不等于0,该卡不存在
                                if (status == 0)
                                {
                                    int id = Convert.ToInt32(xcx.GetAttribute("id")); //ID
                                    foreach (Mydata.MyItem item in lbCards.SelectedItems) //判断对方的卡是否有我需要的
                                    {
                                        if (item.Id == id) //↓↓↓找到了需要的↓↓↓
                                        {
                                            //Debug.WriteLine(mat[i].ToString());
                                            Mydata.Uuin = mat[i].ToString();
                                            U_boxInfo();
                                            tssState.Text = "已找到 【" + _cardDic[id].Name + "】";
                                            tssState.BackColor = Color.Green;
                                            this.Size = new Size(700, 408);
                                            cbTop.Left = 626;
                                            btnStartSearch.Text = "开始搜索";
                                            Console.Beep();
                                            Mydata.isChanging = true;
                                            threadStop();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                //Uuins.Add(mat[i].ToString());
            }
            //MessageBox.Show(uuidStr);
            //---------------------------------------

        }//查询是否找到自己要的卡
        private bool Get_Exch(string EXch)
        {
            int type = _themeDic[Chose_Exchange_Theme].Type;
                /*
             	0=未下架,可以购买 
	            1=下架,可以变卡或变卡 
	            2=抽卡随机获得,无法购买? 
	            5=通过活动,不能变卡和购买 
	            9=闪卡
                */
            switch (type)
            {
                case 0:
                    return EXch == "0,0,0,0" ? true : false;
                case 1:
                    return EXch == "0,0,0,0" ? true : false;
                case 2:
                    return true;
                case 5:
                    return true;
                default:
                    return EXch == "0,0,0,0" ? true : false;
            }
        }
        #endregion
        private void lbCards_Click(object sender, EventArgs e)
        {
            btnStartSearch.Enabled = lbCards.SelectedItems.Count <= 0 ? false : true;
            if (lbCards.Text == "你还没有选择套卡") btnStartSearch.Enabled = false; 
        }
        #region 右键菜单中心
        #region 组套卡菜单
        private void tsmiSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbCards.Items.Count; i++)
            {
                lbCards.SetSelected(i, true);
            }
            lbCards_Click(sender, e);
        }//全选
        private void tsmiCancel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lbCards.Items.Count; i++)
            {
                lbCards.SetSelected(i, false);
            }
            lbCards_Click(sender, e);
        }//取消

        #endregion 我的卡箱菜单
        private void tsmiReload_Click(object sender, EventArgs e)
        {
            I_boxInfo();
        }//刷新
        #endregion
        private void btnTst_Click_1(object sender, EventArgs e)
        {
            Mydata.Uuin = tbTest.Text;
            U_boxInfo();
        }
        #region 双方选中面值计算
        private void lvUbox_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            lvItemsChecked();
        }
        private void lvIbox_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            lvItemsChecked();
        }
        private void lvItemsChecked()
        {
            if (Mydata.isChanging)
            {
                int iCount = 0, uCount = 0;
                int iPrice = 0, uPrice = 0;
                iCount = lvIbox.CheckedItems.Count;
                uCount = lvUbox.CheckedItems.Count;
                //Debug.WriteLine("iCount" + iCount);
                //Debug.WriteLine("uCount" + uCount);

                if (iCount > 5 || uCount > 5)
                {
                    tssEXchange.Text = "您一次最多选择五张卡片进行交换";
                    tssEXchange.BackColor = Color.Red;
                    return;
                }
                if (iCount >= 0 && uCount >= 0)
                {
                    //选中X张，X面值：选中X张，X面值
                    foreach (ListViewItem item in lvIbox.CheckedItems)
                    {
                        iPrice += _cardDic[((Mydata.MyItem)item.Tag).Id].Price;
                    }
                    foreach (ListViewItem item in lvUbox.CheckedItems)
                    {
                        uPrice += _cardDic[((Mydata.MyItem)item.Tag).Id].Price;
                    }
                    tssEXchange.Text = string.Format("选中{0}张，{1}面值：选中{2}张，{3}面值", 
                        lvIbox.CheckedItems.Count, iPrice, lvUbox.CheckedItems.Count, uPrice);
                    tssEXchange.BackColor = Color.Red;//Color.FromKnownColor(KnownColor.Control);
                    tsslOkExchange.Visible = false;
                    if (iCount == uCount && iPrice == uPrice && iPrice > 0 && uPrice > 0) 
                    {
                        tsslOkExchange.Visible = true;
                        tssEXchange.BackColor = Color.Green;
                    }
                    //Debug.WriteLine("ItemChecked:" + lvUbox.CheckedItems.Count);
                    Debug.WriteLine(tssEXchange.Height);
                }

            }

        }

        #endregion
        #region 交换卡片
        private void tsslOkExchange_Click(object sender, EventArgs e)
        {
            Card_Exchange();
        }
        private void Card_Exchange()
        {
            string cardid = null, cardsit = null, isCof = null;//卡片ID,卡片位置,是否保险箱
            string iChoseCards = null, uChoseCards = null;//我的卡片信息,卡友卡片信息
            int temp = 0;//数量计数
            foreach (ListViewItem item in lvIbox.CheckedItems)
            {
                cardid = ((Mydata.MyItem)item.Tag).Id.ToString(); //取出卡片ID
                cardsit = ((Mydata.MyItem)item.Tag).Sit.ToString();//取出卡片位置
                isCof = item.Group.Header.IndexOf("卡箱") > 0 ? "0" : "1";//是否保险箱 
                temp++; //数量累计
                string tempChoseCards = cardid + "_" + cardsit + "_" + isCof; 
                if (temp > 1)
                {
                    iChoseCards = iChoseCards + "|" + tempChoseCards;
                }
                else
                {
                    iChoseCards = tempChoseCards;
                }
                //少于等于1则直接输出,大于则在中间加上|
            }

            temp = 0;//重置计数
            foreach (ListViewItem item in lvUbox.CheckedItems)
            {
                cardid = ((Mydata.MyItem)item.Tag).Id.ToString();
                cardsit = ((Mydata.MyItem)item.Tag).Sit.ToString();
                string tempChoseCards = cardid + "_" + cardsit + "_0";
                temp++;
                if (temp > 1)
                {
                    uChoseCards = uChoseCards + "|" + tempChoseCards;
                }
                else
                {
                    uChoseCards = tempChoseCards;
                }
            }
            string postData; //创建提交变量
            postData = string.Format("uin={0}&frnd={1}&cmd=1&isFriend=1&src={2}&dst={3}", Mydata.Iuin, Mydata.Uuin, iChoseCards, uChoseCards);
            //frnd=对方QQ&cmd=1&isFriend=1&src=对方卡片信息&uin=我的QQ&dst=我的卡片信息
            string EXchangeTemp = Mydata.Card_Excg(postData);
            //提交并取得返回值
            if (EXchangeTemp.IndexOf("code=\"0\"") != -1)
            {
                //成功修改背景字体
                tssState.Text = "交换成功 ~~~";
                tssState.ForeColor = Color.Black;
                tssState.BackColor = Color.GreenYellow;
                I_boxInfo();//刷新我的卡箱
                U_boxInfo();//刷新卡友卡箱
                tssEXchange.Visible = false;
                tsslOkExchange.Visible = false;
                Console.Beep(800, 200);//成功提醒
            }
            else
            {
                string switchStr = Myhelp.getMid(EXchangeTemp, "code=\"", "\" mess");
                //取出失败ID,进行switch匹配
                switch (switchStr)
                {
                    case "0":
                        break;
                    case "-33061":
                        tssState.Text = "交换失败,此卡已被换走!";
                        break;
                    case "-33058":
                        tssState.Text = "交换失败,有卡片被锁!";
                        break;
                    default:
                        tssState.Text = "交换失败,未知错误!";
                        break;
                }
                tssState.ForeColor = Color.Black;
                tssState.BackColor = Color.Red;
                //I_boxInfo();//刷新我的卡箱
                U_boxInfo();//刷新卡友卡箱
                tssEXchange.Visible = false;
                tsslOkExchange.Visible = false;
                Console.Beep(400, 200);//失败提醒
            }
        }
        #endregion

        private void tssmAbout_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
        }

        private void tsmOpenHelp_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Ifover/MC-EX/blob/master/README.md");
        }
    }
}
