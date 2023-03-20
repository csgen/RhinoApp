using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan.AutoCommands
{
    internal class Commands
    {
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
            using (GetObject getBuilding = new GetObject())
            {
                Selector.SelectBuidling(buildings, getBuilding, "选楼");
            }
            planeObjectM.Buildings = buildings;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;
            planeObjectM.P2P_Path = p2p_Paths;
            PathObject pathObject = new PathObject(planeObjectM, doc);
            foreach (Brep brep in pathObject.PathBreps)
                doc.Objects.AddBrep(brep);
        }
        public static Result DrawP2P_Path(RhinoDoc doc)
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
            using (GetObject getBuilding = new GetObject())
            {
                Selector.SelectBuidling(buildings, getBuilding, "选楼");
            }


            planeObjectM.Buildings = buildings;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;

            Point3d pt0;
            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第一个点");
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No start point was selected.");
                    return GPT.CommandResult();
                }
                pt0 = GPT.Point();
            }
            Point3d pt1;
            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第二个点");
                GPT.SetBasePoint(pt0, true);
                GPT.DynamicDraw +=
                    (sender, e) => e.Display.DrawCurve(new P2P_Path(new List<Point3d> { pt0, e.CurrentPoint }, planeObjectM).MidCurve, System.Drawing.Color.DarkRed);
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No end point was selected.");
                    return GPT.CommandResult();
                }
                pt1 = GPT.Point();
            }
            P2P_Path p1 = new P2P_Path(new List<Point3d> { pt0, pt1 }, planeObjectM);
            planeObjectM.P2P_Path.Add(p1);
            PathObject pathObject = new PathObject(planeObjectM, doc);
            doc.Objects.AddCurve(p1.MidCurve);
            foreach (Brep brep in pathObject.PathBreps)
                doc.Objects.AddBrep(brep);
            return Result.Success;
        }
    }
}
