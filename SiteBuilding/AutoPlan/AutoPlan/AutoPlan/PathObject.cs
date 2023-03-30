using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class PathObject
    {
        public double DOC_TOLERANCE = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        public List<Path> Paths { get; set; }
        public OuterPath OuterPath { get; set; }
        public List<MainPath> MainPaths { get; set; }
        public List<P2P_Path> P2P_Paths { get; set; }
        //List<Surface> PathSurface { get; set; }
        RhinoDoc RhinoDoc { get; set; }
        Curve[] OuterPathEdge { get; set; }
        Curve[] MainPathEdge { get; set; }
        Curve[] P2P_PathEdge { get; set; }
        public Brep[] PathBreps { get; set; }
        public PlaneObjectManager planeObjectM { get; set; }

        public PathObject(PlaneObjectManager planeObjectM, RhinoDoc rhinoDoc)
        {
            this.planeObjectM = planeObjectM;
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
            #region 暂时弃用
            ///先合并OuterPath和MainPath,会形成环路，是的Curve.CreateBooleanUnion失效
            //---
            //Curve[] MainAndOuterPathEdge = new Curve[MainPathEdge.Length + OuterPathEdge.Length];
            //Array.Copy(MainPathEdge, MainAndOuterPathEdge, MainPathEdge.Length);
            //Array.Copy(OuterPathEdge, 0, MainAndOuterPathEdge, MainPathEdge.Length, OuterPathEdge.Length);
            //Curve[] union1 = Curve.CreateBooleanUnion(MainAndOuterPathEdge, 0.001);
            //for(int i = 0; i < union1.Length; i++)
            //{
            //    union1[i] = Curve.CreateFilletCornersCurve(union1[i], 4, 0.001, 0.001);
            //}
            //Curve[] union2 = new Curve[union1.Length + P2P_PathEdge.Length];
            //Array.Copy(union1, union2, union1.Length);
            //Array.Copy(P2P_PathEdge, 0, union2, union1.Length, P2P_PathEdge.Length);
            //Curve[] union3 = Curve.CreateBooleanUnion(union2, 0.001);
            //for(int i = 0; i < union3.Length; i++)
            //{
            //    union3[i] = Curve.CreateFilletCornersCurve(union3[i], 2, 0.001, 0.001);
            //}
            //return Brep.CreatePlanarBreps(union3, 0.001);
            //---
            #endregion

            Curve[] P2PAndMainEdge = new Curve[MainPathEdge.Length + P2P_PathEdge.Length];
            Array.Copy(MainPathEdge, P2PAndMainEdge, MainPathEdge.Length);
            Array.Copy(P2P_PathEdge, 0, P2PAndMainEdge, MainPathEdge.Length, P2P_PathEdge.Length);//合并P2P和mainPath

            Curve[] edgeUnion = Curve.CreateBooleanUnion(P2PAndMainEdge, 0.001);//BooleanUnion

            for (int i = 0; i < edgeUnion.Length; i++)//倒角曲线
            {
                edgeUnion[i] = Curve.CreateFilletCornersCurve(edgeUnion[i], 2, 0.001, 0.001);//倒角大小还需测试
            }
            Curve[] AllEdge = new Curve[edgeUnion.Length + OuterPathEdge.Length];
            Array.Copy(edgeUnion, AllEdge, edgeUnion.Length);
            Array.Copy(OuterPathEdge, 0, AllEdge, edgeUnion.Length, OuterPathEdge.Length);
            //edgeUnion.CopyTo(AllEdge, 0);
            //AllEdge[edgeUnion.Length] = OuterPathEdge;//合并内部Path和OuterPath
            Curve[] edgeUnion2 = Curve.CreateBooleanUnion(AllEdge, 0.001);//BooleanUnion
            for (int i = 0; i < edgeUnion2.Length; i++)//倒角
            {
                edgeUnion2[i] = Curve.CreateFilletCornersCurve(edgeUnion2[i], 4, 0.001, 0.001);
            }
            Brep[] rawBrep = Brep.CreatePlanarBreps(edgeUnion2, 0.01);
            List<Brep> cuttingBreps = new List<Brep>();
            foreach (Building building in planeObjectM.Buildings)
            {
                Curve curve = building.BuildingCurve;
                Brep buildingSrf = Brep.CreatePlanarBreps(new Curve[] { curve }, DOC_TOLERANCE)[0];
                if (buildingSrf.Transform(Transform.Translation(Vector3d.ZAxis * -10)))
                {
                    Brep cuttingBrep = buildingSrf.Faces[0].CreateExtrusion(new Line(Point3d.Origin, Vector3d.ZAxis, 20).ToNurbsCurve(), true);
                    cuttingBreps.Add(cuttingBrep);
                }
            }
            List<Brep> newBreps = new List<Brep>();
            foreach (Brep brep in rawBrep)
            {
                List<Brep> newBrep = TreeManager.TrimSolid(brep, cuttingBreps.ToArray());
                foreach (Brep nbrep in newBrep)
                    newBreps.Add(nbrep);
            }
            return newBreps.ToArray();
            //return Brep.CreatePlanarBreps(edgeUnion2, 0.01);

        }
        private Curve[] OuterPathBrep(double outerFilletRadi = 8, List<Point3d> entry=null)//默认外围道路只有一条，并有一个默认入口，入口也可以单独设置(之后增加)
        {
            List<Curve> curves = AddEntry(OuterPath.MidCurve, entry);//在入口处打断
            Curve[] brepEdgeArray = new Curve[curves.Count];
            for(int i = 0; i < curves.Count; i++)
            {
                Curve[] loftCurves = new Curve[2];
                Curve baseCurve = Curve.CreateFilletCornersCurve(curves[i], outerFilletRadi, DOC_TOLERANCE, RhinoDoc.ModelAngleToleranceRadians);
                if (baseCurve == null)
                    baseCurve = curves[i];

                loftCurves[0] = baseCurve.Offset(Plane.WorldXY, OuterPath.Width / 2, DOC_TOLERANCE, CurveOffsetCornerStyle.Sharp)[0];
                loftCurves[1] = baseCurve.Offset(Plane.WorldXY, -OuterPath.Width / 2, DOC_TOLERANCE, CurveOffsetCornerStyle.Sharp)[0];
                Brep pathSrf = Brep.CreateFromLoft(loftCurves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];
                Curve brepEdge = Curve.JoinCurves(pathSrf.Edges, 0.01)[0];
                brepEdgeArray[i] = Curve.CreateFilletCornersCurve(brepEdge, outerFilletRadi / 2, 0.001, 0.001);
            }
            return brepEdgeArray;
            //Brep pathBrep = CreatePathSurface(OuterPath);
            //Curve brepEdge = Curve.JoinCurves(pathBrep.Edges, 0.01)[0];
            //return Curve.CreateFilletCornersCurve(brepEdge, OuterFilletRadi / 2, 0.001, 0.001);
        }
        private List<Curve> AddEntry(Curve OuterCurve, List<Point3d> entryPoint)//在入口位置将封闭外围线打断
        {
            if (entryPoint == null)
            {
                entryPoint = new List<Point3d>();
                Curve entryPath = MainPaths[0].MidCurve;
                var events = Intersection.CurveCurve(OuterCurve, entryPath,0.001,0.001);
                if (events != null)
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        entryPoint.Add(events[i].PointA);
                    }
                }
                if (events == null)
                {
                    Point3d point = OuterCurve.PointAt((OuterCurve.Domain.T0 + OuterCurve.Domain.T1) / 2);//中点
                    entryPoint.Add(point);
                }
            }
            
            List<Curve> trimmedCrv = new List<Curve>();
            foreach(Point3d point in entryPoint)
            {
                Circle entryCircle = new Circle(point, 2);
                trimmedCrv.Add(entryCircle.ToNurbsCurve());
            }
            return TrimCurveWithCurves(OuterCurve, trimmedCrv);
        }
        public List<Curve> TrimCurveWithCurves(Curve oCurve, List<Curve> trimmingCurves)
        {
            //double docTolerance = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            List<double> tList = new List<double>();
            List<Curve> pieces = new List<Curve>();
            foreach(Curve trimmingCurve in trimmingCurves)
            {
                var events = Intersection.CurveCurve(oCurve, trimmingCurve, DOC_TOLERANCE, DOC_TOLERANCE);
                if (events != null)
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        var ccx = events[i];
                        tList.Add(ccx.ParameterA);
                    }
                }
            }
            
            if (!tList.Any())
            {
                pieces.Add(oCurve);
                return pieces;
            }
            pieces = oCurve.Split(tList).ToList();
            List<Curve> keepList = new List<Curve>();
            foreach(Curve piece in pieces)
            {
                bool keep = true;//是否保留此曲线
                Point3d midPoint = piece.PointAt((piece.Domain.T0 + piece.Domain.T1) / 2);
                foreach(Curve trimmingCurve in trimmingCurves)
                {
                    if (trimmingCurve.Contains(midPoint, Plane.WorldXY, DOC_TOLERANCE) != PointContainment.Outside)
                        keep = false;
                }
                if (keep)
                {
                    keepList.Add(piece);
                }
            }
            return keepList;
        }
        private Curve[] MainPathBrep(double GMainFilletRadi = 6)//G代表General：全局/默认
        {
            List<Path> mainPaths = new List<Path>();
            foreach (MainPath path in MainPaths)
            {
                Curve c = path.MidCurve;
                var events = Intersection.CurveCurve(c, OuterPath.MidCurve, 0.001, 0.001);
                if (events != null)
                {
                    for (int i = 0; i < events.Count; i++)
                    {
                        var e = events[i];
                        Point3d p1 = c.PointAtStart;
                        Point3d p2 = c.PointAtEnd;
                        double d1 = p1.DistanceTo(e.PointA);
                        double d2 = p2.DistanceTo(e.PointA);
                        if (d1 > d2)
                        {
                            Curve c1 = c.Extend(CurveEnd.Start, OuterPath.Width, CurveExtensionStyle.Line);
                            c = c1;
                        }
                        if (d1 < d2)
                        {
                            Curve c1 = c.Extend(CurveEnd.End, OuterPath.Width, CurveExtensionStyle.Line);
                            c = c1;
                        }
                    }
                }
                path.MidCurve = c;
                
                mainPaths.Add(path);
            }
            return PathEdge_SingleClass(mainPaths, GMainFilletRadi);
        }
        private Curve[] P2P_PathBrep(double GP2PFilletRadi = 2)
        {
            List<Path> p2p_Paths = new List<Path>();
            foreach (P2P_Path path in P2P_Paths)
            {
                Curve c = path.MidCurve;
                Point3d p = path.StartPoint;
                Point3d p1 = c.PointAtStart;
                Point3d p2 = c.PointAtEnd;
                double d1 = p1.DistanceTo(p);
                double d2 = p2.DistanceTo(p);
                P2P_Path path1 = new P2P_Path(path.MidCurve,planeObjectM);
                path1.Width = path.Width;
                if (d1 < d2)
                {
                    Curve c1 = c.Extend(CurveEnd.Start, 15, CurveExtensionStyle.Line);
                    path1.MidCurve = c1;
                    
                }
                if (d1 > d2)
                {
                    Curve c1 = c.Extend(CurveEnd.End, 15, CurveExtensionStyle.Line);
                    path1.MidCurve = c1;
                }
                p2p_Paths.Add(path1);
            }
            return PathEdge_SingleClass(p2p_Paths, GP2PFilletRadi);
        }
        private Brep CreatePathSurface(Path path)//倒角后，往两边偏移并loft出曲面
        {
            Curve[] loftCurves = new Curve[2];
            Curve baseCurve = Curve.CreateFilletCornersCurve(path.MidCurve, path.FilletRadi, RhinoDoc.ModelAbsoluteTolerance, RhinoDoc.ModelAngleToleranceRadians);
            if (baseCurve == null)
                baseCurve = path.MidCurve;

            loftCurves[0] = baseCurve.Offset(Plane.WorldXY, path.Width / 2, RhinoDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];
            loftCurves[1] = baseCurve.Offset(Plane.WorldXY, -path.Width / 2, RhinoDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];


            Brep pathSrf = Brep.CreateFromLoft(loftCurves, Point3d.Unset, Point3d.Unset, LoftType.Normal, false)[0];
            return pathSrf;
        }
        private Curve[] PathEdge_SingleClass(List<Path> paths, double generalRadius)//为单一种类的道路建立BooleanUnion,返回封闭边缘线
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
            for (int i = 0; i < filletedEdge.Length; i++)
            {
                filletedEdge[i] = Curve.CreateFilletCornersCurve(brepUnionEdge[i], generalRadius, 0.001, 0.001);
            }
            return filletedEdge;
            //return Brep.CreatePlanarBreps(filletedEdge, 0.01);
        }
        public bool ReplayPathObjectHistory(ReplayHistoryData replay)
        {
            ObjRef objRef = null;
            return true;
        }
        //public bool WritePathObjectHistory(HistoryRecord history, ObjRef objRef)
        //{
        //    if(!history.SetObjRef())
        //    return true;
        //}
    }
}
