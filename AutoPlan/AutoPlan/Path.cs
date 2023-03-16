using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class Path
    {
        public Curve MidCurve { get; set; }
        public double Width { get; set; }
        public double FilletRadi { get; set; }
        
    }
}
