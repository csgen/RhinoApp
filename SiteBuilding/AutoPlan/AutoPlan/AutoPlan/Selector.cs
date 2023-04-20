using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static void SelectOuterPath(OuterPath outerPath, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            Curve midCurve = getPath.Object(0).Curve();
            outerPath.MidCurve = midCurve;

        }
        public static void SelectOuterPathCurve(PlaneObjectManager planeObjectM, OuterPath outerPath, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            Guid id = getPath.Object(0).ObjectId;
            var dict = getPath.Object(0).Object().UserDictionary;
            dict.Set("AutoPlan", "OuterPath");
            Curve midCurve = getPath.Object(0).Curve();
            outerPath.ID = id;
            outerPath.MidCurve = midCurve;

        }
        public static void SelectMainPathCurve(PlaneObjectManager planeObjectM, List<MainPath> paths, GetObject getPath, RhinoDoc doc, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            for (int i = 0; i < getPath.ObjectCount; i++)
            {
                Curve midCurve = getPath.Object(i).Curve();
                MainPath path = new MainPath(doc);
                path.MidCurve = midCurve;
                path.ID = getPath.Object(i).ObjectId;
                paths.Add(path);

            }
        }
        public static void SelectP2P_PathCurve(RhinoDoc doc, PlaneObjectManager planeObjectM, List<P2P_Path> paths, GetObject getPath, string prompt = "请选择")
        {
            getPath.EnablePreSelect(false, true);
            getPath.SetCommandPrompt(prompt);
            getPath.GeometryFilter = ObjectType.Curve;
            getPath.GetMultiple(1, 0);
            for (int i = 0; i < getPath.ObjectCount; i++)
            {
                Curve midCurve = getPath.Object(i).Curve();
                Guid id = getPath.Object(i).ObjectId;
                P2P_Path path = new P2P_Path(doc, id, planeObjectM);
                path.UserEdit = true;
                //path.ID = getPath.Object(i).ObjectId;
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
        public void SelectBuidling(List<Building> buildings, GetObject getBuildings, out List<ObjRef> refBuildings, string prompt = "请选择")
        {
            getBuildings.EnablePreSelect(false, true);
            getBuildings.SetCommandPrompt(prompt);
            getBuildings.GeometryFilter = ObjectType.Curve;
            getBuildings.GetMultiple(1, 0);
            refBuildings = new List<ObjRef>();
            for (int i = 0; i < getBuildings.ObjectCount; i++)
            {
                Curve buildingCurve = getBuildings.Object(i).Curve();
                Guid id = getBuildings.Object(i).ObjectId;
                //Rectangle3d buildingCrv = Rectangle3d.CreateFromPolyline(polyline);
                Building building = new Building(id,doc);
                building.ID = id;
                buildings.Add(building);
                
                getBuildings.Object(i).Curve().UserDictionary.AddContentsFrom(building.ClassData);//添加数据
                var dict = getBuildings.Object(i).Object().UserDictionary;
                dict.Set("AutoPlan", "BuildingClass");
                refBuildings.Add(getBuildings.Object(i));
                //Guid id = getBuildings.Object(i).ObjectId;
                //var a = new ObjRef(doc, id);
                //a.Object().UserDictionary.AddContentsFrom(building.ClassData);
            }
        }
    }
}
