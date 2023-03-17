using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class PathObject
    {
        List<Path> Paths { get; set; }
        OuterPath OuterPath { get; set; }
        List<MainPath> MainPaths { get; set; }
        List<P2P_Path> P2P_Paths { get; set; }
        List<Surface> PathSurface { get; set; }
        RhinoDoc RhinoDoc { get; set; }
        public PathObject(PlaneObjectManager planeObjectM, RhinoDoc rhinoDoc)
        {
            OuterPath = planeObjectM.OuterPath;
            MainPaths = planeObjectM.MainPath;
            P2P_Paths = planeObjectM.P2P_Path;
            RhinoDoc = rhinoDoc;
        }
        private Brep OuterPathBrep(double OuterFilletRadi=8)
        {
            Brep pathBrep = CreatePathSurface(OuterPath);
            return pathBrep;
        }
        private Brep[] MainPathBrep(double GMainFilletRadi=6)//G代表General：全局/默认
        {
            List<Path> mainPaths = new List<Path>();
            foreach(MainPath path in MainPaths) 
                mainPaths.Add(path);
            return PathBrep_SingleClass(mainPaths, GMainFilletRadi);
        }
        private Brep[] P2P_PathBrep(double GP2PFilletRadi=2)
        {
            List<Path> p2p_Paths = new List<Path>();
            foreach (P2P_Path path in p2p_Paths)
                p2p_Paths.Add(path);
            return PathBrep_SingleClass(p2p_Paths, GP2PFilletRadi);
        }
        private Brep CreatePathSurface(Path path)//倒角后，往两边偏移并loft出曲面
        {
            Curve[] loftCurves = new Curve[2];
            Curve baseCurve = Curve.CreateFilletCornersCurve(path.MidCurve, path.FilletRadi, RhinoDoc.ModelAbsoluteTolerance, RhinoDoc.ModelAngleToleranceRadians);
            if (baseCurve != null)
            {
                loftCurves[0] = baseCurve.Offset(Plane.WorldXY, path.Width / 2, RhinoDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];
                loftCurves[1] = baseCurve.Offset(Plane.WorldXY, -path.Width / 2, RhinoDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];
            }
            Brep pathSrf = Brep.CreateFromLoft(loftCurves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];
            return pathSrf;
        }
        private Brep[] PathBrep_SingleClass(List<Path> paths,double generalRadius)//为单一种类的道路建立BooleanUnion
        {
            Curve[] brepEdgeArray = new Curve[paths.Count];
            for (int i = 0; i < paths.Count; i++)
            {
                Brep pathBrep = CreatePathSurface(paths[i]);
                brepEdgeArray[i] = Curve.JoinCurves(pathBrep.Edges, 0.01)[0];//把边缘形成封闭线
            }
            Curve[] brepUnionEdge = Curve.CreateBooleanUnion(brepEdgeArray, 0.01);//布尔运算主要道路面并取得边缘线
            //主路倒角（全局统一半径）
            foreach (Curve edge in brepUnionEdge)
            {
                Curve.CreateFilletCornersCurve(edge, generalRadius, 0.001, 0.001);
            }
            return Brep.CreatePlanarBreps(brepUnionEdge, 0.01);
        }
    }
}
