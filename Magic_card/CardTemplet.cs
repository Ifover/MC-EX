using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magic_card
{
    class CardTemplet
    {
        #region 字段
        string _name;
        int _id, _themeID, _price;
        //List<CardTemplet> _formCards;
        #endregion
        #region 属性
        public int ID
        {
            get { return _id; }
        }

        public int ThemeID
        {
            get { return _themeID; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Price
        {
            get { return _price; }
        }
        #endregion
        public CardTemplet(int id, int themeID, string name, int price)
        {
            _id = id;
            _themeID = themeID;
            _name = name;
            _price = price;
        }

    }
}
