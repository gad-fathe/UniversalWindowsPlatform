using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.ExpandableListView
{
    class ImageState
    {
        private bool _expanded;

        public bool Expanded
        {
            get { return _expanded; }
            set { _expanded = value; }
        }

    }
}
