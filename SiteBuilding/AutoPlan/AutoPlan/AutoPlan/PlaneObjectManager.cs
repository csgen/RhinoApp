using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;

namespace AutoPlan.AutoPlan
{
    internal class PlaneObjectManager
    {
        public ArchivableDictionary BuildingDict { get; set; }
        public ArchivableDictionary OuterPathDict { get; set; }
        public ArchivableDictionary MainPathDict { get; set; }
        public ArchivableDictionary P2P_PathDict { get; set; }
        private List<Building> buildings;
        public List<Building> Buildings 
        {
            get => buildings;
            set
            {
                if (value != null)
                {
                    buildings = value;
                    foreach (Building b in buildings)
                    {
                        string tag = "Building" + b.ID.ToString();
                        BuildingDict.Set(tag, b.DataSet);
                    }
                }
            }
        }
        private OuterPath outerPath;
        public OuterPath OuterPath 
        {
            get => outerPath;
            set
            {
                outerPath = value;
                if (outerPath.ID != Guid.Empty)
                {
                    string tag = "OuterPath" + outerPath.ID.ToString();
                    OuterPathDict.Set(tag, outerPath.DataSet);
                }
            }
        }
        private List<MainPath> mainPath;
        public List<MainPath> MainPath 
        {
            get => mainPath;
            set
            {
                if (value != null)
                {
                    mainPath = value;
                    foreach (MainPath p in mainPath)
                    {
                        string tag = "MainPath" + p.ID.ToString();
                        MainPathDict.Set(tag, p.DataSet);
                    }
                }
            }
        }
        private List<P2P_Path> p2p_Path;
        public List<P2P_Path> P2P_Path 
        {
            get => p2p_Path;
            set
            {
                if (value != null)
                {
                    p2p_Path = value;
                    foreach(P2P_Path p in p2p_Path)
                    {
                        string tag = "P2P_Path" + p.ID.ToString();
                        P2P_PathDict.Set(tag, p.DataSet);//以ID作为存入字典时对应的key
                    }
                }
            }
        }
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
            BuildingDict = new ArchivableDictionary();
            OuterPathDict = new ArchivableDictionary();
            MainPathDict = new ArchivableDictionary();
            P2P_PathDict = new ArchivableDictionary();
            Buildings = new List<Building>();
            OuterPath = new OuterPath(RhinoDoc.ActiveDoc);
            MainPath = new List<MainPath>();
            P2P_Path = new List<P2P_Path>();
            Paths = new List<Path>();
            RefBuildings = new List<ObjRef>();
            //Paths.AddRange(MainPath);
            //Paths.AddRange(P2P_Path);
            //Paths.Add(OuterPath);
            //TreeM = new TreeManager(this);
        }
        public void GetData(ArchivableDictionary dictionary, RhinoDoc doc)//从字典中获取数据重建planeObjectManager
        {
            if (null == dictionary)
                return;
            RhinoApp.WriteLine(dictionary.Name);

            if (dictionary.ContainsKey("BuildingsData"))
            {
                var bDict = dictionary.GetDictionary("BuildingsData");
                Buildings = GetBuildingData(bDict, doc);
            }

            if (dictionary.ContainsKey("OuterPathData"))
            {
                var oDict = dictionary.GetDictionary("OuterPathData");
                OuterPath = GetOuterPathData(oDict, doc);
            }
            
            if (dictionary.ContainsKey("MainPathData"))
            {
                var mDict = dictionary.GetDictionary("MainPathData");
                MainPath = GetMainPathData(mDict, doc);
            }

            if (dictionary.ContainsKey("P2P_PathData"))
            {
                var pDict = dictionary.GetDictionary("P2P_PathData");
                P2P_Path = GetP2P_PathData(pDict, doc, this);
            }
            
            Paths.AddRange(MainPath);
            Paths.AddRange(P2P_Path);
            Paths.Add(OuterPath);

            //以下待删除
            ////
        }
        public void SetData(ArchivableDictionary dictionary)
        {
            if (null == dictionary)
                return;

            SetBuildingData(dictionary);
            SetOuterPathData(dictionary);
            SetMainPathData(dictionary);
            SetP2P_PathData(dictionary);

            //以下待删除
            ////
        }
        public static List<Building> GetBuildingData(ArchivableDictionary dictionary, RhinoDoc doc)//针对"BuildingsData"对应的Dict
        {
            List<Building> buildings = new List<Building>();
            if (null == dictionary) return buildings;
            foreach(string a in dictionary.Keys)
            {
                if (a.StartsWith("Building"))
                {
                    var d = dictionary.GetDictionary(a);
                    Building building = Building.BuiltFromDict(d, doc);
                    buildings.Add(building);
                }
            }
            return buildings;
        }
        public static OuterPath GetOuterPathData(ArchivableDictionary dictionary,RhinoDoc doc)
        {
            OuterPath outerPath = new OuterPath(doc);
            if (null == dictionary) return outerPath;
            RhinoApp.WriteLine(dictionary.Name);

            foreach (string a in dictionary.Keys)
            {
                if (a.StartsWith("OuterPath"))
                {
                    var d = dictionary.GetDictionary(a);
                    outerPath = OuterPath.BuiltFromDict(d, doc);
                }
            }
            return outerPath;
        }
        public static List<MainPath> GetMainPathData(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            List<MainPath> mainPaths = new List<MainPath>();
            if (null == dictionary) return mainPaths;
            foreach (string a in dictionary.Keys)
            {
                if (a.StartsWith("MainPath"))
                {
                    var d = dictionary.GetDictionary(a);
                    MainPath p = AutoPlan.MainPath.BuiltFromDict(d, doc);
                    mainPaths.Add(p);
                }
            }
            return mainPaths;
        }
        public static List<P2P_Path> GetP2P_PathData(ArchivableDictionary dictionary, RhinoDoc doc,PlaneObjectManager planeObjectM)
        {
            List<P2P_Path> p2p_Paths = new List<P2P_Path>();
            if (null == dictionary) return p2p_Paths;
            foreach (string a in dictionary.Keys)
            {
                if (a.StartsWith("P2P_Path"))
                {
                    var d = dictionary.GetDictionary(a);
                    P2P_Path p = AutoPlan.P2P_Path.BuiltFromDict(d, doc, planeObjectM);
                    p2p_Paths.Add(p);
                }
            }
            return p2p_Paths;
        }
        public void SetBuildingData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;
            dictionary.Set("BuildingsData", BuildingDict);

        }
        public void SetOuterPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;
            dictionary.Set("OuterPathData", OuterPathDict);
        }
        public void SetMainPathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;
            dictionary.Set("MainPathData", MainPathDict);
        }
        public void SetP2P_PathData(ArchivableDictionary dictionary)
        {
            if (null == dictionary) return;
            dictionary.Set("P2P_PathData", P2P_PathDict);
        }
    }
}
