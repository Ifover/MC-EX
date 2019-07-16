frmLogin 登录窗口
Size:700, 415

frmEx	 卡牌交换窗口
Mydata	 各种变量
Myhelp	 各种函数

CardTemplet  卡片信息
ThemeTemplet 套卡信息

        ThreadStart myThreaddelegate = new ThreadStart(Get_CardFriends);
        Thread tre = new Thread(myThreaddelegate);

	0=未下架,可以购买 
	1=下架,可以变卡或变卡 
	2=抽卡随机获得,无法购买? 
	5=通过活动,不能变卡和购买 
	9=闪卡

1--用时:00:00:15.8173412
2--用时:00:00:08.6240537
4--用时:00:00:07.9675640

700,408
440,408  
260