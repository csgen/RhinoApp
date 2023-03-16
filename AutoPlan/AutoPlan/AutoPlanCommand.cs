using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;

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
            List<Point3d> inputPoints = new List<Point3d>();
            
            using (GetPoint GPT = new GetPoint())
            {
                while (true)
                {
                    GPT.SetCommandPrompt("选点");
                    //GPT.GeometryFilter = ObjectType.Point;
                    //GPT.GetMultiple(1, 0);
                    //GPT.DynamicDraw+=(sender,e)=>e.Display.DrawPoint()
                    GPT.AcceptNothing(true);
                    GPT.Get();
                    if (GPT.CommandResult() != Result.Success)
                        break;
                    inputPoints.Add(GPT.Point());
                }
            }
            List<Building> buildings = new List<Building>();
            using(GetObject getCurve = new GetObject())
            {
                getCurve.SetCommandPrompt("选楼");
                getCurve.GeometryFilter = ObjectType.Curve;
                getCurve.GetMultiple(1, 0);
                for(int i = 0; i < getCurve.ObjectCount; i++)
                {
                    getCurve.Object(i).Curve().TryGetPolyline(out Polyline polyline);
                    Rectangle3d buildingCrv = Rectangle3d.CreateFromPolyline(polyline);
                    buildings.Add(new Building(buildingCrv, 3));
                }
            }
            List<Path> paths = new List<Path>();
            using (GetObject getCurve = new GetObject())
            {
                getCurve.SetCommandPrompt("选主路");
                getCurve.GeometryFilter = ObjectType.Curve;
                getCurve.GetMultiple(1, 0);
                for (int i = 0; i < getCurve.ObjectCount; i++)
                {
                    Curve midCurve = getCurve.Object(i).Curve();
                    Path mainPath = new Path();
                    mainPath.MidCurve = midCurve;
                    mainPath.Width = 3;
                    paths.Add(mainPath);
                    
                }
            }
            PlaneObjectManager planeObjectM = new PlaneObjectManager();
            planeObjectM.Buildings = buildings;
            planeObjectM.Paths = paths;
            P2P_Path p1 = new P2P_Path(inputPoints, planeObjectM);

            doc.Objects.AddCurve(p1.MidCurve);
            RhinoApp.WriteLine(inputPoints.Count.ToString());
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
    }
}
