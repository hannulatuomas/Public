using System;

namespace GH_GUI
{
    public class MenuDropItem
    {
        public string content;

        public int index;

        public object data;

        public MenuDropItem(string cont, int ind)
        {
            this.content = cont;
            this.index = ind;
        }
    }
}
