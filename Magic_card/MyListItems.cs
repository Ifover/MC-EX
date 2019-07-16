using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Magic_card
{
    class MyListItems
    {
        public string Name
        { get; set; }
        public int Id  
        { get; set; }
        public override string ToString()
        {
            return Name;
        }
        public MyListItems(string name, int id)
        { Name = name; Id = id;  }
    }
}
