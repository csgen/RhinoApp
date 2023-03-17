using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class MainPath:Path
    {
        public MainPath()
        {
            this.FilletRadi = 6;
            this.Width = 8;
            
        }
    }
}
