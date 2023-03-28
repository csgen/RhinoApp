using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class PlaneObjectManager
    {
        public List<Building> Buildings { get; set; }
        public OuterPath OuterPath { get; set; }
        public List<MainPath> MainPath { get; set; }
        public List<P2P_Path> P2P_Path { get; set; }
        public List<Path> Paths { get; set; }
        public PlantingTrees PlantingTrees { get; set; }
        public double OuterPathWidth { get; set; }
        public double MainPathWidth { get; set; }
        public double P2P_PathWidth { get; set; }
        public double OuterPlantingWidth { get; set; }
        public double GMainFilletRadi { get; set; }//主路MainPath全局倒角半径
        public PathObject PathObject { get; set; }
        public TreeManager TreeM { get; set; }
        public List<ObjRef> RefBuildings { get; set; }

        public PlaneObjectManager()
        {
            Buildings = new List<Building>();
            OuterPath = new OuterPath();
            MainPath = new List<MainPath>();
            P2P_Path = new List<P2P_Path>();
            Paths = new List<Path>();
            RefBuildings = new List<ObjRef>();
            Paths.AddRange(MainPath);
            Paths.AddRange(P2P_Path);
            Paths.Add(OuterPath);
            //TreeM = new TreeManager(this);
        }
        public void GetData(ArchivableDictionary dictionary)
        {
            if (null == dictionary)
                return;
            RhinoApp.WriteLine(dictionary.Name);

            Curve[] buildingCurve = dictionary["BuildingCurves"] as Curve[];
            double[] buildingAvoidDist = dictionary["BuildingAvoidDist"] as double[];
            ObjRef[] refBuildings = dictionary["RefBuildings"] as ObjRef[];
            for(int i = 0; i < buildingCurve.Length; i++)
            {
                Building building = new Building(buildingCurve[i], buildingAvoidDist[i]);
                Buildings.Add(building);
            }
            for(int i = 0; i < refBuildings.Length; i++)
            {
                RefBuildings.Add(refBuildings[i]);
            }

            Curve OuterPathCurve = dictionary["OuterPathCurve"] as Curve;
            dictionary.TryGetDouble("OuterPathWidth", out double OuterPathWidth);
            dictionary.TryGetDouble("OuterPathFilletRadi", out double OuterPathFilletRadi);
            OuterPath.MidCurve = OuterPathCurve;
            OuterPath.Width = OuterPathWidth;
            OuterPath.FilletRadi = OuterPathFilletRadi;

            Curve[] mainPathCurves = dictionary["MainPathCurves"] as Curve[];
            double[] mainPathWidths = dictionary["MainPathWidths"] as double[];
            double[] mainPathFilletRadi = dictionary["MainPathFilletRadi"] as double[];
            for(int i = 0; i < mainPathCurves.Length; i++)
            {
                MainPath mainPath = new MainPath();
                mainPath.MidCurve = mainPathCurves[i];
                mainPath.Width = mainPathWidths[i];
                mainPath.FilletRadi = mainPathFilletRadi[i];
                MainPath.Add(mainPath);
            }

            if (dictionary.ContainsKey("p2p_pathCurves") && dictionary.ContainsKey("p2p_pathWidths") && dictionary.ContainsKey("p2p_pathFilletRadi"))
            {
                Curve[] p2p_pathCurves = dictionary["p2p_pathCurves"] as Curve[];
                double[] p2p_pathWidths = dictionary["p2p_pathWidths"] as double[];
                double[] p2p_pathFilletRadi = dictionary["p2p_pathFilletRadi"] as double[];
                for (int i = 0; i < p2p_pathCurves.Length; i++)
                {
                    P2P_Path path = new P2P_Path(p2p_pathCurves[i], this);
                    //path.MidCurve = p2p_pathCurves[i];
                    path.Width = p2p_pathWidths[i];
                    path.FilletRadi = p2p_pathFilletRadi[i];
                    P2P_Path.Add(path);
                }
            }
            
            
            
        }
        public void SetData(ArchivableDictionary dictionary)
        {
            if (null == dictionary)
                return;

            dictionary.Set("OuterPathCurve", OuterPath.MidCurve);
            dictionary.Set("OuterPathWidth", OuterPath.Width);
            dictionary.Set("OuterPathFilletRadi", OuterPath.FilletRadi);

            Curve[] mainPathCurves = new Curve[MainPath.Count];
            double[] mainPathWidths = new double[MainPath.Count];
            double[] mainPathFilletRadi = new double[MainPath.Count];
            for (int i = 0; i < MainPath.Count; i++)
            {
                mainPathCurves[i] = MainPath[i].MidCurve;
                mainPathWidths[i] = MainPath[i].Width;
                mainPathFilletRadi[i] = MainPath[i].FilletRadi;
            }
            dictionary.Set("MainPathCurves", mainPathCurves);
            dictionary.Set("MainPathWidths", mainPathWidths);
            dictionary.Set("MainPathFilletRadi", mainPathFilletRadi);

            Curve[] p2p_pathCurves = new Curve[P2P_Path.Count];
            double[] p2p_pathWidths = new double[P2P_Path.Count];
            double[] p2p_pathFilletRadi = new double[P2P_Path.Count];
            for (int i = 0; i < P2P_Path.Count; i++)
            {
                p2p_pathCurves[i] = P2P_Path[i].MidCurve;
                p2p_pathWidths[i] = P2P_Path[i].Width;
                p2p_pathFilletRadi[i] = P2P_Path[i].FilletRadi;
            }
            dictionary.Set("p2p_pathCurves", p2p_pathCurves);
            dictionary.Set("p2p_pathWidths", p2p_pathWidths);
            dictionary.Set("p2p_pathFilletRadi", p2p_pathFilletRadi);
        }
        public void GetOuterPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;
            RhinoApp.WriteLine(dictionary.Name);

            Curve OuterPathCurve = dictionary["OuterPathCurve"] as Curve;
            dictionary.TryGetDouble("OuterPathWidth", out double OuterPathWidth);
            dictionary.TryGetDouble("OuterPathFilletRadi", out double OuterPathFilletRadi);
        }
        public void SetBuildingData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] buildingCurves = new Curve[Buildings.Count];
            double[] buildingAvoidDist = new double[Buildings.Count];
            ObjRef[] refBuildings = new ObjRef[RefBuildings.Count];
            for(int i = 0; i < Buildings.Count; i++)
            {
                buildingCurves[i] = Buildings[i].BuildingCurve;
                buildingAvoidDist[i] = Buildings[i].AvoidDistance;
            }
            for(int i = 0; i < RefBuildings.Count; i++)
            {
                refBuildings[i] = RefBuildings[i];
            }
            
            dictionary.Set("BuildingCurves", buildingCurves);
            dictionary.Set("BuildingAvoidDist", buildingAvoidDist);
            dictionary.Set("RefBuildings", refBuildings);
        }
        public void SetOuterPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            dictionary.Set("OuterPathCurve", OuterPath.MidCurve);
            dictionary.Set("OuterPathWidth", OuterPath.Width);
            dictionary.Set("OuterPathFilletRadi", OuterPath.FilletRadi);
        }
        public void SetMainPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] mainPathCurves = new Curve[MainPath.Count];
            double[] mainPathWidths = new double[MainPath.Count];
            double[] mainPathFilletRadi = new double[MainPath.Count];
            for (int i = 0; i < MainPath.Count; i++)
            {
                mainPathCurves[i] = MainPath[i].MidCurve;
                mainPathWidths[i] = MainPath[i].Width;
                mainPathFilletRadi[i] = MainPath[i].FilletRadi;
            }
            dictionary.Set("MainPathCurves", mainPathCurves);
            dictionary.Set("MainPathWidths", mainPathWidths);
            dictionary.Set("MainPathFilletRadi", mainPathFilletRadi);
        }
        public void SetP2P_PathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] p2p_pathCurves = new Curve[P2P_Path.Count];
            double[] p2p_pathWidths = new double[P2P_Path.Count];
            double[] p2p_pathFilletRadi = new double[P2P_Path.Count];
            for (int i = 0; i < P2P_Path.Count; i++)
            {
                p2p_pathCurves[i] = P2P_Path[i].MidCurve;
                p2p_pathWidths[i] = P2P_Path[i].Width;
                p2p_pathFilletRadi[i] = P2P_Path[i].FilletRadi;
            }
            dictionary.Set("p2p_pathCurves", p2p_pathCurves);
            dictionary.Set("p2p_pathWidths", p2p_pathWidths);
            dictionary.Set("p2p_pathFilletRadi", p2p_pathFilletRadi);
        }
    }
}
