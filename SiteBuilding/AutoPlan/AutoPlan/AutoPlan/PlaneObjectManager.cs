﻿using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
        public void GetData(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            if (null == dictionary)
                return;
            RhinoApp.WriteLine(dictionary.Name);

            Buildings = GetBuildingData(dictionary, doc);
            foreach(Building building in Buildings)
            {
                RefBuildings.Add(new ObjRef(doc,building.ID));
            }
            OuterPath = GetOuterPathData(dictionary, doc);
            MainPath = GetMainPathData(dictionary, doc);
            P2P_Path = GetP2P_PathData(dictionary, doc, this);
            
        }
        public void SetData(ArchivableDictionary dictionary)
        {
            if (null == dictionary)
                return;
            Curve[] buildingCurves = new Curve[Buildings.Count];
            double[] buildingAvoidDist = new double[Buildings.Count];
            Guid[] buildingIDs = new Guid[Buildings.Count];
            for(int i = 0; i < Buildings.Count; i++)
            {
                buildingCurves[i] = Buildings[i].BuildingCurve;
                buildingAvoidDist[i] = Buildings[i].avoidDistance;
                buildingIDs[i] = Buildings[i].ID;
            }
            dictionary.Set("BuildingCurves", buildingCurves);
            dictionary.Set("BuildingAvoidDist", buildingAvoidDist);
            dictionary.Set("BuildingIDs", buildingIDs);
            

            dictionary.Set("OuterPathCurve", OuterPath.MidCurve);
            dictionary.Set("OuterPathWidth", OuterPath.Width);
            dictionary.Set("OuterPathFilletRadi", OuterPath.FilletRadi);
            dictionary.Set("OuterPathID", OuterPath.ID);

            Curve[] mainPathCurves = new Curve[MainPath.Count];
            double[] mainPathWidths = new double[MainPath.Count];
            double[] mainPathFilletRadi = new double[MainPath.Count];
            Guid[] mainPathGuids = new Guid[MainPath.Count];
            for (int i = 0; i < MainPath.Count; i++)
            {
                mainPathCurves[i] = MainPath[i].MidCurve;
                mainPathWidths[i] = MainPath[i].Width;
                mainPathFilletRadi[i] = MainPath[i].FilletRadi;
                mainPathGuids[i] = MainPath[i].ID;
            }
            dictionary.Set("MainPathCurves", mainPathCurves);
            dictionary.Set("MainPathWidths", mainPathWidths);
            dictionary.Set("MainPathFilletRadi", mainPathFilletRadi);
            dictionary.Set("MainPathIDs", mainPathGuids);

            Curve[] p2p_pathCurves = new Curve[P2P_Path.Count];
            double[] p2p_pathWidths = new double[P2P_Path.Count];
            double[] p2p_pathFilletRadi = new double[P2P_Path.Count];
            Guid[] p2p_pathGuids = new Guid[P2P_Path.Count];
            for (int i = 0; i < P2P_Path.Count; i++)
            {
                p2p_pathCurves[i] = P2P_Path[i].MidCurve;
                p2p_pathWidths[i] = P2P_Path[i].Width;
                p2p_pathFilletRadi[i] = P2P_Path[i].FilletRadi;
                p2p_pathGuids[i] = P2P_Path[i].ID;
            }
            dictionary.Set("p2p_pathCurves", p2p_pathCurves);
            dictionary.Set("p2p_pathWidths", p2p_pathWidths);
            dictionary.Set("p2p_pathFilletRadi", p2p_pathFilletRadi);
            dictionary.Set("p2p_pathIDs", p2p_pathGuids);
        }
        public static List<Building> GetBuildingData(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            List<Building> buildings = new List<Building>();
            if (null == dictionary) return buildings;
            if (dictionary.ContainsKey("BuildingCurves"))
            {
                double[] buildingAvoidDist = dictionary["BuildingAvoidDist"] as double[];
                Guid[] buildingIDs = dictionary["BuildingIDs"] as Guid[];
                if (buildingIDs != null && buildingIDs.Length != 0)
                {
                    for(int i = 0; i < buildingIDs.Length; i++)
                    {
                        if (doc.Objects.Find(buildingIDs[i]) != null)
                        {
                            Curve curve = new ObjRef(doc, buildingIDs[i]).Curve();
                            Building building = new Building(curve, buildingAvoidDist[i]);
                            building.ID = buildingIDs[i];
                            buildings.Add(building);
                        }
                    }
                }
            }
            return buildings;
        }
        public static OuterPath GetOuterPathData(ArchivableDictionary dictionary,RhinoDoc doc)
        {
            OuterPath outerPath = new OuterPath();
            if (null == dictionary) return outerPath;
            RhinoApp.WriteLine(dictionary.Name);

            dictionary.TryGetDouble("OuterPathWidth", out double OuterPathWidth);
            dictionary.TryGetDouble("OuterPathFilletRadi", out double OuterPathFilletRadi);
            dictionary.TryGetGuid("OuterPathID", out Guid OuterPathGuid);
            if (doc.Objects.Find(OuterPathGuid) != null)
            {
                outerPath.MidCurve = new ObjRef(doc, OuterPathGuid).Curve();
                outerPath.Width = OuterPathWidth;
                outerPath.FilletRadi = OuterPathFilletRadi;
                outerPath.ID = OuterPathGuid;
            }
            return outerPath;
        }
        public static List<MainPath> GetMainPathData(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            List<MainPath> mainPaths = new List<MainPath>();
            if (null == dictionary) return mainPaths;
            if (dictionary.ContainsKey("MainPathCurves"))
            {
                Curve[] mainPathCurves = dictionary["MainPathCurves"] as Curve[];
                double[] mainPathWidths = dictionary["MainPathWidths"] as double[];
                double[] mainPathFilletRadi = dictionary["MainPathFilletRadi"] as double[];
                Guid[] mainPathGuids = dictionary["MainPathIDs"] as Guid[];
                if (mainPathGuids != null && mainPathGuids.Length != 0)
                {
                    for (int i = 0; i < mainPathGuids.Length; i++)
                    {
                        MainPath mainPath = new MainPath();
                        if (doc.Objects.Find(mainPathGuids[i]) != null)
                        {
                            mainPath.MidCurve = new ObjRef(doc, mainPathGuids[i]).Curve();
                            mainPath.ID = mainPathGuids[i];
                            mainPath.Width = mainPathWidths[i];
                            mainPath.FilletRadi = mainPathFilletRadi[i];
                            mainPaths.Add(mainPath);
                        }
                        else
                        {
                            mainPathGuids[i] = Guid.Empty;
                            mainPathWidths[i] = 0;
                            mainPathFilletRadi[i] = 0;
                        }
                    }
                }
            }
            return mainPaths;
        }
        public static List<P2P_Path> GetP2P_PathData(ArchivableDictionary dictionary, RhinoDoc doc,PlaneObjectManager planeObjectM)
        {
            List<P2P_Path> p2p_Paths = new List<P2P_Path>();
            if (null == dictionary) return p2p_Paths;
            if (dictionary.ContainsKey("p2p_pathCurves") && dictionary.ContainsKey("p2p_pathWidths") && dictionary.ContainsKey("p2p_pathFilletRadi"))
            {
                Curve[] p2p_pathCurves = dictionary["p2p_pathCurves"] as Curve[];
                double[] p2p_pathWidths = dictionary["p2p_pathWidths"] as double[];
                double[] p2p_pathFilletRadi = dictionary["p2p_pathFilletRadi"] as double[];
                Guid[] guids = dictionary["p2p_pathIDs"] as Guid[];
                if (guids != null && guids.Length > 0)
                {
                    for (int i = 0; i < guids.Length; i++)
                    {
                        if (doc.Objects.Find(guids[i]) != null)
                        {
                            P2P_Path path = new P2P_Path(new ObjRef(doc, guids[i]).Curve(), planeObjectM);
                            path.Width = p2p_pathWidths[i];
                            path.FilletRadi = p2p_pathFilletRadi[i];
                            path.ID = guids[i];
                            p2p_Paths.Add(path);
                        }
                        else
                        {
                            guids[i] = Guid.Empty;
                            p2p_pathWidths[i] = 0;
                            p2p_pathFilletRadi[i] = 0;
                        }
                    }
                }
            }
            return p2p_Paths;
        }
        public void SetBuildingData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] buildingCurves = new Curve[Buildings.Count];
            double[] buildingAvoidDist = new double[Buildings.Count];
            ObjRef[] refBuildings = new ObjRef[RefBuildings.Count];
            Guid[] guids = new Guid[Buildings.Count];
            for(int i = 0; i < Buildings.Count; i++)
            {
                buildingCurves[i] = Buildings[i].BuildingCurve;
                buildingAvoidDist[i] = Buildings[i].AvoidDistance;
                guids[i] = Buildings[i].ID;
            }
            for(int i = 0; i < RefBuildings.Count; i++)
            {
                refBuildings[i] = RefBuildings[i];
            }
            
            dictionary.Set("BuildingCurves", buildingCurves);
            dictionary.Set("BuildingAvoidDist", buildingAvoidDist);
            dictionary.Set("RefBuildings", refBuildings);
            dictionary.Set("BuildingIDs", guids);
        }
        public void SetOuterPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            dictionary.Set("OuterPathCurve", OuterPath.MidCurve);
            dictionary.Set("OuterPathWidth", OuterPath.Width);
            dictionary.Set("OuterPathFilletRadi", OuterPath.FilletRadi);
            dictionary.Set("OuterPathID", OuterPath.ID);
        }
        public void SetMainPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] mainPathCurves = new Curve[MainPath.Count];
            double[] mainPathWidths = new double[MainPath.Count];
            double[] mainPathFilletRadi = new double[MainPath.Count];
            Guid[] guids = new Guid[MainPath.Count];
            for (int i = 0; i < MainPath.Count; i++)
            {
                mainPathCurves[i] = MainPath[i].MidCurve;
                mainPathWidths[i] = MainPath[i].Width;
                mainPathFilletRadi[i] = MainPath[i].FilletRadi;
                guids[i] = MainPath[i].ID;
            }
            dictionary.Set("MainPathCurves", mainPathCurves);
            dictionary.Set("MainPathWidths", mainPathWidths);
            dictionary.Set("MainPathFilletRadi", mainPathFilletRadi);
            dictionary.Set("MainPathIDs", guids);
        }
        public void SetP2P_PathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;

            Curve[] p2p_pathCurves = new Curve[P2P_Path.Count];
            double[] p2p_pathWidths = new double[P2P_Path.Count];
            double[] p2p_pathFilletRadi = new double[P2P_Path.Count];
            Guid[] guids = new Guid[P2P_Path.Count];
            for (int i = 0; i < P2P_Path.Count; i++)
            {
                p2p_pathCurves[i] = P2P_Path[i].MidCurve;
                p2p_pathWidths[i] = P2P_Path[i].Width;
                p2p_pathFilletRadi[i] = P2P_Path[i].FilletRadi;
                guids[i] = P2P_Path[i].ID;
            }
            dictionary.Set("p2p_pathCurves", p2p_pathCurves);
            dictionary.Set("p2p_pathWidths", p2p_pathWidths);
            dictionary.Set("p2p_pathFilletRadi", p2p_pathFilletRadi);
            dictionary.Set("p2p_pathIDs", guids);
        }
    }
}
