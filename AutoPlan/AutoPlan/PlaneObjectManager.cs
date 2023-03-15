using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class PlaneObjectManager
    {
        public Building Building { get; set; }
        public OuterPath OuterPath { get; set; }
        public MainPath MainPath { get; set; }
        public P2P_Path P2P_Path { get; set; }
        public PlantingTrees PlantingTrees { get; set; }
        public double OuterPathWidth { get; set; }
        public double MainPathWidth { get; set; }
        public double P2P_PathWidth { get; set; }
        public double OuterPlantingWidth { get; set; }
        
    }
}
