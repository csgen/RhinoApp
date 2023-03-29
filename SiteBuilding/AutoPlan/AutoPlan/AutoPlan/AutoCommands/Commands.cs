﻿using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPlan.AutoPlan.AutoCommands
{
    internal class Commands
    {
        public PlaneObjectManager planeObjectM = new PlaneObjectManager();
        public static void GetBuildings(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            List<Building> buildings = new List<Building>();
            List<ObjRef> refBuildings;
            using (GetObject getBuilding = new GetObject())
            {
                Selector s = new Selector(doc);
                s.SelectBuidling(buildings, getBuilding, out refBuildings, "选楼");
            }
            planeObjectM.Buildings = buildings;
            planeObjectM.RefBuildings = refBuildings;
            planeObjectM.SetBuildingData(AutoPlanPlugin.Instance.Dictionary);
        }
        public static void GetOuterPath(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            OuterPath outerPath = new OuterPath();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectOuterPathCurve(planeObjectM, outerPath, getPath, "选外围道路");
            }
            planeObjectM.OuterPath = outerPath;
            planeObjectM.SetOuterPathData(AutoPlanPlugin.Instance.Dictionary);
        }
        public static void GetMainPath(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            List<MainPath> mainPaths = new List<MainPath>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectMainPathCurve(planeObjectM, mainPaths, getPath, "选主路");
            }
            planeObjectM.MainPath = mainPaths;
            planeObjectM.SetMainPathData(AutoPlanPlugin.Instance.Dictionary);
        }
        public static void GetP2P_Path(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            List<P2P_Path> p2p_Paths = new List<P2P_Path>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectP2P_PathCurve(planeObjectM, p2p_Paths, getPath, "选小路");
            }
            planeObjectM.P2P_Path = p2p_Paths;
            planeObjectM.SetP2P_PathData(AutoPlanPlugin.Instance.Dictionary);
        }
        public static void GeneratePathObject(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            
            planeObjectM.GetData(AutoPlanPlugin.Instance.Dictionary,doc);
            PathObject pathObject = new PathObject(planeObjectM, doc);
            planeObjectM.PathObject = pathObject;
            if (pathObject != null)
            {
                if (AutoPlanPlugin.Instance.Dictionary.ContainsKey("PathObjectGUID"))
                {
                    Guid[] oldGuids = AutoPlanPlugin.Instance.Dictionary["PathObjectGUID"] as Guid[];
                    foreach (Guid id in oldGuids)
                    {
                        doc.Objects.Delete(id, true);
                    }
                }
                //TreeManager treeM = new TreeManager(planeObjectM);
                Guid[] pathObjectIDs = new Guid[pathObject.PathBreps.Length];
                //List<Guid> pathObjectIDs = new List<Guid>();
                for(int i = 0; i < pathObject.PathBreps.Length; i++)
                {
                    pathObjectIDs[i] = doc.Objects.AddBrep(pathObject.PathBreps[i]);
                }
                planeObjectM.SetData(AutoPlanPlugin.Instance.Dictionary);
                AutoPlanPlugin.Instance.Dictionary.Set("PathObjectGUID", pathObjectIDs);
            }
            
            //treeM.AddTreeToDoc();
            //planeObjectM.PathObject = pathObject;
        }
        public static void GenerateLandscape(RhinoDoc doc)
        {
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器

            planeObjectM.GetData(AutoPlanPlugin.Instance.Dictionary, doc);
            PathObject pathObject = new PathObject(planeObjectM, doc);
            planeObjectM.PathObject = pathObject;
            if (pathObject != null)
            {
                List<Brep> pathObjs = new List<Brep>();
                if (AutoPlanPlugin.Instance.Dictionary.ContainsKey("PathObjectGUID"))
                {
                    Guid[] guids = AutoPlanPlugin.Instance.Dictionary["PathObjectGUID"] as Guid[];
                    foreach(Guid id in guids)
                    {
                        if (doc.Objects.Find(id) != null)
                            pathObjs.Add(new ObjRef(doc,id).Brep());
                    }
                }
                if (pathObjs.Count != 0)
                {
                    TreeManager treeM = new TreeManager(planeObjectM);
                    treeM.AddTreeToDoc(doc);
                }
                else
                {
                    MessageBox.Show("请先绘制道路物件");
                }
                
            }
        }
        public static void GlobalGenerate(RhinoDoc doc)
        {
            RhinoApp.WriteLine("Start");

            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器

            OuterPath outerPath = new OuterPath();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectOuterPathCurve(planeObjectM, outerPath, getPath, "选外围道路");
            }

            List<MainPath> mainPaths = new List<MainPath>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectMainPathCurve(planeObjectM, mainPaths, getPath, "选主路");
            }

            List<P2P_Path> p2p_Paths = new List<P2P_Path>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectP2P_PathCurve(planeObjectM, p2p_Paths, getPath, "选小路");
            }

            List<Building> buildings = new List<Building>();
            List<ObjRef> refBuildings;
            using (GetObject getBuilding = new GetObject())
            {
                Selector s = new Selector(doc);
                s.SelectBuidling(buildings, getBuilding, out refBuildings, "选楼");
            }
            planeObjectM.Buildings = buildings;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;
            planeObjectM.P2P_Path = p2p_Paths;
            PathObject pathObject = new PathObject(planeObjectM, doc);
            planeObjectM.PathObject = pathObject;
            TreeManager treeM = new TreeManager(planeObjectM);
            List<Guid> pathObjectIDs = new List<Guid>();
            foreach (Brep brep in pathObject.PathBreps)
                pathObjectIDs.Add(doc.Objects.AddBrep(brep));
            treeM.AddTreeToDoc(doc);
            RhinoApp.WriteLine(treeM.Trees.Count().ToString());

            planeObjectM.SetData(AutoPlanPlugin.Instance.Dictionary);
            AutoPlanPlugin.Instance.Dictionary.Set("GUID", pathObjectIDs);
        }
        public static void AddPath(RhinoDoc doc)
        {
            RhinoApp.WriteLine("Start");
            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            planeObjectM.GetData(AutoPlanPlugin.Instance.Dictionary,doc);
            List<MainPath> mainPaths = new List<MainPath>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectMainPathCurve(planeObjectM, mainPaths, getPath, "选主路");
            }
            foreach(MainPath path in mainPaths)
            {
                planeObjectM.MainPath.Add(path);
            }
            List<P2P_Path> p2p_Paths = new List<P2P_Path>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectP2P_PathCurve(planeObjectM, p2p_Paths, getPath, "选小路");
            }
            foreach(P2P_Path path in p2p_Paths)
            {
                planeObjectM.P2P_Path.Add(path);
            }
            PathObject pathObject = new PathObject(planeObjectM, doc);

            List<Guid> pathObjectIDs = AutoPlanPlugin.Instance.Dictionary["PathObjectGUID"] as List<Guid>;
            foreach (Guid id in pathObjectIDs)
            {
                doc.Objects.Delete(id, true);
            }
            List<Guid> pathObjNewIDs = new List<Guid>();
            foreach (Brep brep in pathObject.PathBreps)
            pathObjNewIDs.Add(doc.Objects.AddBrep(brep));
            
            planeObjectM.SetData(AutoPlanPlugin.Instance.Dictionary);
            AutoPlanPlugin.Instance.Dictionary.Set("PathObjectGUID", pathObjNewIDs);
        }
        public static Result DrawP2P_Path(RhinoDoc doc,Command command,out PlaneObjectManager planeObjectManager, out P2P_Path path, out List<ObjRef> refBuildings, out Point3d pt0, out Point3d pt1)
        {
            RhinoApp.WriteLine("Start");

            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器

            OuterPath outerPath = new OuterPath();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectOuterPathCurve(planeObjectM, outerPath, getPath, "选外围道路");
            }

            List<MainPath> mainPaths = new List<MainPath>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectMainPathCurve(planeObjectM, mainPaths, getPath, "选主路");
            }

            List<Building> buildings = new List<Building>();
            //List<ObjRef> refBuildings;
            using (GetObject getBuilding = new GetObject())
            {
                Selector s = new Selector(doc);
                s.SelectBuidling(buildings, getBuilding, out refBuildings, "选楼");
            }


            planeObjectM.Buildings = buildings;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;


            //P2P_Path p1 = new P2P_Path(new List<Point3d> { pt0, pt1 }, planeObjectM);
            //P2P_Path path;
            List<Point3d> points = DrawP2PPolyline(planeObjectM, out path);
            pt0 = points[0];
            pt1 = points[1];
            planeObjectM.P2P_Path.Add(path);
            PathObject pathObject = new PathObject(planeObjectM, doc);

            planeObjectManager = planeObjectM;

            //Guid id = doc.Objects.AddCurve(p1.MidCurve);
            
            //foreach (Brep brep in pathObject.PathBreps)
            //    doc.Objects.AddBrep(brep);
            return Result.Success;
        }
        public static List<Point3d> DrawP2PPolyline(PlaneObjectManager planeObjectM, out P2P_Path path)
        {
            Point3d pt0 = Point3d.Unset;
            Point3d pt1 = Point3d.Unset;
            List<Point3d> points = new List<Point3d>();

            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第一个点");
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No start point was selected.");
                    path = null;
                    return points;
                    //return GPT.CommandResult();
                }
                pt0 = GPT.Point();
            }
            
            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第二个点");
                GPT.SetBasePoint(pt0, true);
                GPT.DynamicDraw +=
                    (sender, e) => e.Display.DrawCurve(new P2P_Path(new List<Point3d> { pt0, e.CurrentPoint }, planeObjectM).MidCurve, System.Drawing.Color.DarkRed);
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No end point was selected.");
                    path = null;
                    return points;
                    //return GPT.CommandResult();
                }
                pt1 = GPT.Point();
            }
            points.Add(pt0);
            points.Add(pt1);
            path = new P2P_Path(points, planeObjectM);
            return points;
        }
        public static Result AutoDrawP2P_Path(RhinoDoc doc, Command command, out PlaneObjectManager planeObjectManager, out P2P_Path path, out List<ObjRef> refBuildings, out Point3d pt0, out Point3d pt1)
        {
            planeObjectManager = new PlaneObjectManager();
            //PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            planeObjectManager.GetData(AutoPlanPlugin.Instance.Dictionary, doc);
            refBuildings = planeObjectManager.RefBuildings;
            List<Point3d> points = DrawP2PPolyline(planeObjectManager, out path);
            pt0 = points[0];
            pt1 = points[1];
            planeObjectManager.P2P_Path.Add(path);
            
            //PathObject pathObject = new PathObject(planeObjectM, doc);

            //planeObjectManager = planeObjectM;
            return Result.Success;
        }
    }
}
