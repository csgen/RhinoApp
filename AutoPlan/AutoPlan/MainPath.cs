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
        
        public double FilletRadi { get; set; }

        public MainPath(double width, Curve midCurve, double filletRadi)
        {
            base.Width = width;
            base.MidCurve = midCurve;
            FilletRadi = filletRadi;
        }
    }
}
