using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magic_card
{
    class ThemeTemplet
    {
        #region 私有字段
        int _id, _diff, _time, _type, _offtime;
        string _name, _cards;
        #endregion
        #region 属性
        public int ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Diff
        {
            get { return _diff; }
        }
        public int Time
        {
            get { return _time; }
        }
        public string Cards
        {
            get { return _cards; }
        }
        public int Type
        {
            get { return _type; }
        }
        public int Offtime
        {
            get { return _offtime; }
        }
        #endregion
        public ThemeTemplet(int id, string name, int diff, int time, string cards, int type, int offtime)
        {
            _id = id;
            _name = name;
            _diff = diff;
            _time = time;
            _cards = cards;
            _type = type;
            _offtime = offtime;
        }
    }
}
