using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class TreeManager
    {
        public double OuterWidth { get; set; }//外围景观宽度
        public double GlobalTreeRadius { get; set; }//树木基础半径
        public double TreeDensity { get; set; }//种植密度，0-1
        public double Rendomness { get; set; }//大小变化程度，0-1
        public int RandomSeed { get; set; }//随机种子
        public List<Tree> Trees { get; set; }
        public PlaneObjectManager PlaneObjectManager { get; set; }
        public TreeManager()
        {

        }
        public void GetPlot()
        {
            Brep[] pathBreps = PlaneObjectManager.PathObject.PathBreps;
            List<Building> buildings = PlaneObjectManager.Buildings;
            
        }
        public void PlantingAlongCurve()
        {

        }
        public void PlantingOnPlot()
        {

        }
    }
}
