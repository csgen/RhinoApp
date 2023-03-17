using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class OuterPath:Path
    {
        public OuterPath()
        {
            this.FilletRadi = 8;
        }
    }
}
