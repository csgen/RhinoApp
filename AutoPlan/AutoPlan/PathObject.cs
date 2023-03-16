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
        List<OuterPath> OuterPaths { get; set; }
        List<MainPath> MainPaths { get; set; }
        List<P2P_Path> P2P_Paths { get; set; }
        List<Surface> PathSurface { get; set; }
        RhinoDoc RhinoDoc { get; set; }
        private Brep[] MainPathSurface(double GMainFilletRadi)
        {
            Curve[] brepEdgeArray = new Curve[MainPaths.Count];
            for(int i = 0; i < MainPaths.Count; i++)
            {
                Brep pathSrf = CreatePathSurface(MainPaths[i]);
                brepEdgeArray[i] = Curve.JoinCurves(pathSrf.Edges, 0.01)[0];
            }
            Curve[] brepUnionEdge = Curve.CreateBooleanUnion(brepEdgeArray, 0.01);//布尔运算主要道路面并取得边缘线
            //主路倒角（全局统一半径）
            foreach (Curve edge in brepUnionEdge) 
            {
                Curve.CreateFilletCornersCurve(edge, GMainFilletRadi, 0.001, 0.001);
            }
            return Brep.CreatePlanarBreps(brepUnionEdge, 0.01);
        }
        private Brep CreatePathSurface(Path path)
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
    }
}
