using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class Selector
    {
        public RhinoDoc doc { get; set; }
        public Selector(RhinoDoc doc)
        {
            this.doc = doc;
        }
        public static void SelectOuterPathCurve(PlaneObjectManager planeObjectM, OuterPath outerPath, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            Curve midCurve = getPath.Object(0).Curve();
            outerPath.MidCurve = midCurve;

        }
        public static void SelectMainPathCurve(PlaneObjectManager planeObjectM, List<MainPath> paths, GetObject getPath, string prompt = "请选择")
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

            }
        }
        public static void SelectP2P_PathCurve(PlaneObjectManager planeObjectM, List<P2P_Path> paths, GetObject getPath, string prompt = "请选择")
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
        public static void SelectPathCurve(PlaneObjectManager planeObjectM, List<Path> paths, GetObject getPath, string prompt = "请选择")
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
            }
        }
        public void SelectBuidling(List<Building> buildings, GetObject getBuildings, string prompt = "请选择")
        {
            getBuildings.EnablePreSelect(false, true);
            getBuildings.SetCommandPrompt(prompt);
            getBuildings.GeometryFilter = ObjectType.Curve;
            getBuildings.GetMultiple(1, 0);

            for (int i = 0; i < getBuildings.ObjectCount; i++)
            {
                getBuildings.Object(i).Curve().TryGetPolyline(out Polyline polyline);
                
                Rectangle3d buildingCrv = Rectangle3d.CreateFromPolyline(polyline);
                Building building = new Building(buildingCrv, 3);
                buildings.Add(building);

                Guid id = getBuildings.Object(i).ObjectId;
                var a = new ObjRef(doc, id);
                a.Object().UserDictionary.AddContentsFrom(building.ClassData);
            }
        }
    }
}
