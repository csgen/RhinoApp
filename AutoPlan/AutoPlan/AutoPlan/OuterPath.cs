using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class OuterPath : Path
    {
        public OuterPath()
        {
            FilletRadi = 8;
            Width = 8;
        }
    }
}
