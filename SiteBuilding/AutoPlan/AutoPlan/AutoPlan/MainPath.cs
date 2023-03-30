using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class MainPath : Path
    {
        public MainPath(double width = 4)
        {
            FilletRadi = 4;
            Width = 4;
            if (MyLib.MyLib.MainPathWidth > width)
            {
                Width = MyLib.MyLib.MainPathWidth;
            }
        }
        public Guid ID { get; set; }
    }
}
