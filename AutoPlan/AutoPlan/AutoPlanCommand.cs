using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using siteUI.Functions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AutoPlan
{
    public class AutoPlanCommand : Command
    {
        public AutoPlanCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static AutoPlanCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "AutoPlanCommand";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            #region
            //RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName);

            //Point3d pt0;
            //using (GetPoint getPointAction = new GetPoint())
            //{
            //    getPointAction.SetCommandPrompt("Please select the start point");
            //    if (getPointAction.Get() != GetResult.Point)
            //    {
            //        RhinoApp.WriteLine("No start point was selected.");
            //        return getPointAction.CommandResult();
            //    }
            //    pt0 = getPointAction.Point();
            //}

            //Point3d pt1;
            //using (GetPoint getPointAction = new GetPoint())
            //{
            //    getPointAction.SetCommandPrompt("Please select the E point");
            //    getPointAction.SetBasePoint(pt0, true);
            //    getPointAction.DynamicDraw +=
            //      (sender, e) => e.Display.DrawLine(pt0, e.CurrentPoint, System.Drawing.Color.DarkRed);
            //    if (getPointAction.Get() != GetResult.Point)
            //    {
            //        RhinoApp.WriteLine("No end point was selected.");
            //        return getPointAction.CommandResult();
            //    }
            //    pt1 = getPointAction.Point();
            //}

            //doc.Objects.AddLine(pt0, pt1);
            //doc.Views.Redraw();
            //RhinoApp.WriteLine("The {0} command added one line to the document.", EnglishName);
            #endregion

            //Point3d pt0;
            //Curve curve1;
            //List<Curve> localcurve = new List<Curve>();
            RhinoApp.WriteLine("Start");

            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器
            OuterPath outerPath = new OuterPath();
            using(GetObject getPath = new GetObject())
            {
                //SelectPathCurve(planeObjectM, outerPath, getPath, "选外围道路");
                getPath.EnablePreSelect(false, true);
                getPath.SetCommandPrompt("选外围道路");
                getPath.GeometryFilter = ObjectType.Curve;
                getPath.GetMultiple(1, 0);
                Curve midCurve = getPath.Object(0).Curve();
                outerPath.MidCurve = midCurve;
                //Path p = new Path();
                //p.MidCurve= midCurve;
                //p.Width = 8;
                //planeObjectM.Paths.Add(p);
            }
            List<MainPath> mainPaths = new List<MainPath>();
            List<Path> paths = new List<Path>();
            using (GetObject getPath = new GetObject())
            {
                SelectPathCurve(planeObjectM, paths, getPath, "选主路");
            }
            List<P2P_Path> p2p_Path = new List<P2P_Path>();
            //using(GetObject getPath = new GetObject())
            //{
            //    SelectPathCurve(p2p_Path, getPath, "选择小路");
            //}
            

            //using(GetObject getPath = new GetObject())
            //{
            //    SelectPathCurve(paths)
            //}
            //GetObject getPath = new GetObject();
            //getPath.SetCommandPrompt("选主路");
            //getPath.GeometryFilter = ObjectType.Curve;
            //getPath.GetMultiple(1, 0);
            //for (int i = 0; i < getPath.ObjectCount; i++)
            //{
            //    Curve midCurve = getPath.Object(i).Curve();
            //    Path mainPath = new Path();
            //    mainPath.MidCurve = midCurve;
            //    mainPath.Width = 3;
            //    paths.Add(mainPath);
            //}

            List<Building> buildings = new List<Building>();
            using (GetObject getBuilding = new GetObject())
            {
                SelectBuidling(buildings, getBuilding, "选楼");
            }

            
            planeObjectM.Buildings = buildings;
            //planeObjectM.Paths = paths;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;
            planeObjectM.Paths = paths;
            //foreach (MainPath path in mainPaths)
            //    planeObjectM.Paths.Add(path);
            
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
            //planeObjectM.P2P_Path.Add(p1);
            //PathObject pathObject = new PathObject(planeObjectM, doc);
            doc.Objects.AddCurve(p1.MidCurve);
            //foreach (Brep brep in pathObject.PathBreps)
            //    doc.Objects.AddBrep(brep);

            //if (paths.Count > 0)
            //{
            //    PlaneObjectManager planeObjectM = new PlaneObjectManager();
            //    planeObjectM.Buildings = buildings;
            //    planeObjectM.Paths = paths;
            //    P2P_Path p1 = new P2P_Path(inputPoints, planeObjectM);

            //    doc.Objects.AddCurve(p1.MidCurve);
            //}

            //RhinoApp.WriteLine(pathObject.PathBreps.Length.ToString());
            //foreach(Curve curve in localcurve)
            //{
            //    foreach (Brep pipe in Brep.CreatePipe(curve, 5, true, PipeCapMode.None, true, 0.01, 0.01))
            //    {
            //        doc.Objects.AddBrep(pipe);
            //    }
            //}
            
            doc.Views.Redraw();



            //RhinoApp.WriteLine("The");

            // ---
            return Result.Success;
        }
        public class siteCommand : Command
        {
            public siteCommand()
            {
                // Rhino only creates one instance of each command class defined in a
                // plug-in, so it is safe to store a refence in a static property.
                Instance = this;
            }

            ///<summary>The only instance of this command.</summary>
            public static siteCommand Instance { get; private set; }

            ///<returns>The command name as it appears on the Rhino command line.</returns>
            public override string EnglishName => "DragLine";

            protected override Result RunCommand(RhinoDoc doc, RunMode mode)
            {
                // TODO: start here modifying the behaviour of your command.
                // ---

                SelectLine.SelectLines(doc);

                doc.Views.Redraw();

                return Result.Success;


            }
        }
        void SelectPathCurve(PlaneObjectManager planeObjectM, OuterPath outerPath, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            Curve midCurve = getPath.Object(0).Curve();
            outerPath.MidCurve = midCurve;
            //Path p = new Path();
            //p.MidCurve = midCurve;
            //planeObjectM.Paths.Add(p);
            //for (int i = 0; i < getPath.ObjectCount; i++)
            //{
            //    Curve midCurve = getPath.Object(i).Curve();
            //    P2P_Path path = new P2P_Path();
            //    path.MidCurve = midCurve;

            //    paths.Add(path);
            //}
        }
        void SelectPathCurve(PlaneObjectManager planeObjectM, List<MainPath> paths, GetObject getPath,string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            for (int i = 0; i < getPath.ObjectCount; i++)
            {
                Curve midCurve = getPath.Object(i).Curve();
                MainPath path = new MainPath();
                path.MidCurve = midCurve;
                
                paths.Add(path);
                Path p = new Path();
                p.MidCurve = midCurve;
                planeObjectM.Paths.Add(p);
            }
        }
        void SelectPathCurve(PlaneObjectManager planeObjectM, List<P2P_Path> paths, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            for (int i = 0; i < getPath.ObjectCount; i++)
            {
                Curve midCurve = getPath.Object(i).Curve();
                P2P_Path path = new P2P_Path();
                path.MidCurve = midCurve;
                
                paths.Add(path);
                planeObjectM.Paths.Add(path);
            }
        }
        void SelectPathCurve(PlaneObjectManager planeObjectM, List<Path> paths, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            for (int i = 0; i < getPath.ObjectCount; i++)
            {
                Curve midCurve = getPath.Object(i).Curve();
                Path path = new Path();
                path.MidCurve = midCurve;
                path.Width = 3;
                paths.Add(path);
                //planeObjectM.Paths.Add(path);
            }
        }
        void SelectBuidling(List<Building> buildings,GetObject getBuildings, string prompt = "请选择")
        {
            getBuildings.EnablePreSelect(false, true);
            getBuildings.SetCommandPrompt(prompt);
            getBuildings.GeometryFilter = ObjectType.Curve;
            getBuildings.GetMultiple(1, 0);

            for (int i = 0; i < getBuildings.ObjectCount; i++)
            {
                getBuildings.Object(i).Curve().TryGetPolyline(out Polyline polyline);
                Rectangle3d buildingCrv = Rectangle3d.CreateFromPolyline(polyline);
                buildings.Add(new Building(buildingCrv, 3));
            }
        }
    }
}
