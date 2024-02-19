using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognexVisionProForm
{
    internal class MyListBoxItem
    {
        public MyListBoxItem(string Text)
        {
            Camdesc = Text;
            itemData = false;
        }
        public MyListBoxItem(string Text, bool ItemData)
        {
            Camdesc = Text;
            itemData = ItemData;
        }
        public bool ItemData
        {
            get => itemData;
            set => itemData = value;
        }
        public override string ToString()
        {
            return Camdesc;
        }

        private string Camdesc;
        private bool itemData;
    }
}
