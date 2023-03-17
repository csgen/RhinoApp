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
        //List<Surface> PathSurface { get; set; }
        RhinoDoc RhinoDoc { get; set; }
        Curve OuterPathEdge { get; set; }
        Curve[] MainPathEdge { get; set; }
        Curve[] P2P_PathEdge { get; set; }
        public Brep[] PathBreps { get; set; }

        public PathObject(PlaneObjectManager planeObjectM, RhinoDoc rhinoDoc)
        {
            OuterPath = planeObjectM.OuterPath;
            MainPaths = planeObjectM.MainPath;
            P2P_Paths = planeObjectM.P2P_Path;
            RhinoDoc = rhinoDoc;
            OuterPathEdge = OuterPathBrep();
            MainPathEdge = MainPathBrep();
            P2P_PathEdge = P2P_PathBrep();
            PathBreps = PathBrep();
        }

        private Brep[] PathBrep()
        {
            Curve[] P2PAndMainEdge = new Curve[MainPathEdge.Length + P2P_PathEdge.Length];
            Array.Copy(MainPathEdge, P2PAndMainEdge, MainPathEdge.Length);
            Array.Copy(P2P_PathEdge, 0, P2PAndMainEdge, MainPathEdge.Length, P2P_PathEdge.Length);//合并P2P和mainPath

            Curve[] edgeUnion = Curve.CreateBooleanUnion(P2PAndMainEdge, 0.01);//BooleanUnion
            
            for(int i = 0; i < edgeUnion.Length; i++)//倒角曲线
            {
                edgeUnion[i] = Curve.CreateFilletCornersCurve(edgeUnion[i], 2, 0.001, 0.001);
            }
            Curve[] AllEdge = new Curve[edgeUnion.Length + 1];
            edgeUnion.CopyTo(AllEdge, 0);
            AllEdge[edgeUnion.Length] = OuterPathEdge;//合并内部Path和OuterPath
            Curve[] edgeUnion2 = Curve.CreateBooleanUnion(AllEdge, 0.01);//BooleanUnion
            for(int i = 0; i < edgeUnion2.Length; i++)//倒角
            {
                edgeUnion2[i] = Curve.CreateFilletCornersCurve(edgeUnion2[i], 4, 0.001, 0.001);
            }
            return Brep.CreatePlanarBreps(edgeUnion2,0.01);
            
        }
        private Curve OuterPathBrep(double OuterFilletRadi=8)//默认外围道路只有一条，并有一个默认入口，入口也可以单独设置
        {
            Brep pathBrep = CreatePathSurface(OuterPath);
            Curve brepEdge = Curve.JoinCurves(pathBrep.Edges, 0.01)[0];
            return Curve.CreateFilletCornersCurve(brepEdge,OuterFilletRadi/2,0.001,0.001);
        }
        private Curve[] MainPathBrep(double GMainFilletRadi=6)//G代表General：全局/默认
        {
            List<Path> mainPaths = new List<Path>();
            foreach(MainPath path in MainPaths) 
                mainPaths.Add(path);
            return PathEdge_SingleClass(mainPaths, GMainFilletRadi);
        }
        private Curve[] P2P_PathBrep(double GP2PFilletRadi=2)
        {
            List<Path> p2p_Paths = new List<Path>();
            foreach (P2P_Path path in p2p_Paths)
                p2p_Paths.Add(path);
            return PathEdge_SingleClass(p2p_Paths, GP2PFilletRadi);
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
        private Curve[] PathEdge_SingleClass(List<Path> paths,double generalRadius)//为单一种类的道路建立BooleanUnion,返回封闭边缘线
        {
            Curve[] brepEdgeArray = new Curve[paths.Count];
            for (int i = 0; i < paths.Count; i++)
            {
                Brep pathBrep = CreatePathSurface(paths[i]);
                brepEdgeArray[i] = Curve.JoinCurves(pathBrep.Edges, 0.01)[0];//把边缘形成封闭线
            }
            Curve[] brepUnionEdge = Curve.CreateBooleanUnion(brepEdgeArray, 0.01);//布尔运算主要道路面并取得边缘线
            //主路倒角（全局统一半径）
            Curve[] filletedEdge = new Curve[brepUnionEdge.Length];
            for(int i = 0; i < filletedEdge.Length; i++)
            {
                filletedEdge[i] = Curve.CreateFilletCornersCurve(brepUnionEdge[i], generalRadius, 0.001, 0.001);
            }
            return filletedEdge;
            //return Brep.CreatePlanarBreps(filletedEdge, 0.01);
        }
    }
}
