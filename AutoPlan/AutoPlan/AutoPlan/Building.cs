using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class Building
    {
        public Rectangle3d BuildingCurve { get; set; }
        public double AvoidDistance { get; set; }


        public Building(Rectangle3d buildingCurve, double avoidDistance)
        {
            BuildingCurve = buildingCurve;
            AvoidDistance = avoidDistance;
        }
    }
}
