using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class LandscapeBlock
    {
        public Curve LandscapeObject { get; set; }

        public LandscapeBlock(Curve landscapeObject)
        {
            LandscapeObject = landscapeObject;
        }
    }
}
