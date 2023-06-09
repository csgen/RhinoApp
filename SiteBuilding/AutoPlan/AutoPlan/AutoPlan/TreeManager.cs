﻿using Rhino;
using Rhino.Display;
using Rhino.Geometry;
using Rhino.Geometry.Collections;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class TreeManager
    {
        public double DOC_TOLERANCE = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
        public double OuterWidth { get; set; }//外围景观宽度
        public double GlobalTreeRadius { get; set; }//树木基础半径
        private double treeDensity;
        public double TreeDensity //种植密度，0-1
        {
            get => treeDensity;
            set
            {
                treeDensity = 0.5;
                if (value >= 0.3 && value <= 0.8) treeDensity = value;
                if (value < 0.3) treeDensity = 0.3;
                if (value > 0.8) treeDensity = 0.8;
            }
        }
        public double Rendomness { get; set; }//大小变化程度，0-1
        public int RandomSeed { get; set; }//随机种子
        public List<Tree> Trees { get; set; }
        public PlaneObjectManager PlaneObjectM { get; set; }
        private List<Brep> Plots { get; set; }
        public double GreenArea { get; set; }
        public double ConcentrationGreenArea { get; set; }
        //public double GreenAreaRatio { get; set; }
        public TreeManager(PlaneObjectManager planeObjectM)
        {
            PlaneObjectM = planeObjectM;
            Trees = new List<Tree>();
            TreeDensity = MyLib.MyLib.TreeDensity;//用treedensity来控制网格密度，以此控制疏密
            PlantingOnPlot();
            
        }
        public void GetPlot()//得到空地
        {
            Brep[] pathBreps = PlaneObjectM.PathObject.PathBreps;
            List<Building> buildings = PlaneObjectM.Buildings;
            List<Brep> cuttingBreps = new List<Brep>();
            foreach(Brep brep1 in pathBreps)
            {
                Brep brep = (Brep)brep1.Duplicate();
                if(brep.Transform(Transform.Translation(Vector3d.ZAxis * -10)))
                {
                    Brep cuttingBrep = brep.Faces[0].CreateExtrusion(new Line(Point3d.Origin, Vector3d.ZAxis, 20).ToNurbsCurve(), true);
                    cuttingBreps.Add(cuttingBrep);
                }
            }
            foreach(Building building in buildings)
            {
                Curve curve = building.BuildingCurve;
                Brep buildingSrf = Brep.CreatePlanarBreps(new Curve[] {curve}, DOC_TOLERANCE)[0];
                if (buildingSrf.Transform(Transform.Translation(Vector3d.ZAxis * -10)))
                {
                    Brep cuttingBrep = buildingSrf.Faces[0].CreateExtrusion(new Line(Point3d.Origin, Vector3d.ZAxis, 20).ToNurbsCurve(), true);
                    cuttingBreps.Add(cuttingBrep);
                }
            }
            Brep originalPlot = Brep.CreatePlanarBreps(new Curve[] { PlaneObjectM.OuterPath.MidCurve }, DOC_TOLERANCE)[0];//先以场地线为边缘建立PlanarSrf
            List<Brep> plotList = TrimSolid(originalPlot, cuttingBreps.ToArray(),false);//用形成的道路extrusion去做布尔运算，得到空地
            GreenArea = 0;
            foreach(Brep brep in plotList)
            {
                GreenArea += AreaMassProperties.Compute(brep).Area;
            }
            this.Plots = plotList;
        }
        public void PlantingAlongCurve()
        {

        }
        public void PlantingOnPlot()//空地处种植
        {
            double baseRadius = 30;//暂定一个基础半径
            double treeScale = MyLib.MyLib.TreeScale;
            
            GetPlot();
            List<Curve> meshFaceEdges = new List<Curve>();
            foreach(Brep plot in Plots)
            {
                meshFaceEdges.AddRange(GetGoodFaceEdge(plot,3));
            }
            foreach(Curve faceEdge in meshFaceEdges)
            {
                Circle circle = InCircle(faceEdge);
                if (circle.Radius > 3*(1-TreeDensity) && circle.Radius < baseRadius)//规定树木半径
                {
                    Tree tree = new Tree(circle.Radius * treeScale, circle.Center);
                    Trees.Add(tree);
                }
                
            }
        }
        public static List<Brep> TrimSolid(Brep brepA, Brep[] brepArray, bool inner)//BrepB来布尔切割brepA,类似GH中的TrimSolid电池,inner为true，保留B内部的，为false，保留外部
        {
            double rhinoTol = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
            var trims = brepA.Split(brepArray, rhinoTol);
            List<Brep> bList = new List<Brep>();
            for (int i = 0; i < trims.Length; i++)
            {
                bool outside = true;//此物件是否在切割物之外
                BoundingBox bbObj = trims[i].GetBoundingBox(true);
                Point3d p1, p2, p3, p4;
                if (bbObj.IsDegenerate(rhinoTol) == 0)
                {
                    p1 = bbObj.Corner(false, false, false);
                    p2 = bbObj.Corner(false, false, true);
                    p3 = bbObj.Corner(true, true, false);
                    p4 = bbObj.Corner(true, true, true);
                }
                else
                {
                    p1 = bbObj.Corner(false, false, false) - Vector3d.ZAxis;
                    p2 = bbObj.Corner(false, false, false) + Vector3d.ZAxis;
                    p3 = bbObj.Corner(true, true, true) - Vector3d.ZAxis;
                    p4 = bbObj.Corner(true, true, true) + Vector3d.ZAxis;
                }
                Brep diagonalBrep = Brep.CreateFromCornerPoints(p1, p2, p4, p3, rhinoTol);
                Curve[] xCrvs;
                Point3d[] xPts;
                Rhino.Geometry.Intersect.Intersection.BrepBrep(trims[i], diagonalBrep, rhinoTol, out xCrvs, out xPts);
                foreach (Curve crv in xCrvs)
                {
                    Point3d testPoint = crv.PointAt((crv.Domain.T0 + crv.Domain.T1) / 2);
                    foreach (Brep brep in brepArray)
                    {
                        if (IsPointInsideBrep(testPoint,brep))
                            outside = false;
                    }
                }
                if (inner)
                {
                    if(!outside) bList.Add(trims[i]);
                }
                if (!inner)
                {
                    if (outside) bList.Add(trims[i]);
                }
                
            }
            return bList;
        }
        public Circle InCircle(Curve triangle)//仅适用于三角形求内切圆
        {
            List<Point3d> points = P2P_Path.GetDiscontinuityPoints(triangle);//闭合三角形得到四个角点
            points.RemoveAt(0);//由于闭合曲线，首尾重复，去除第一个点，留下三个角点
            Point3d p1 = points[0];
            Point3d p2 = points[1];
            Point3d p3 = points[2];
            double C = triangle.GetLength();//周长
            double S = AreaMassProperties.Compute(triangle).Area;
            double R = 2 * S / C;

            Vector3d v12 = p2 - p1;
            Vector3d v13 = p3 - p1;
            v12.Unitize(); v13.Unitize();
            Vector3d v1 = v12 + v13;
            Line l1 = new Line(p1, v1, C);

            Vector3d v21 = p1 - p2;
            Vector3d v23 = p3 - p2;
            v21.Unitize(); v23.Unitize();
            Vector3d v2 = v21 + v23;
            Line l2 = new Line(p2, v2, C);

            Vector3d v31 = p1 - p3;
            Vector3d v32 = p2 - p3;
            v31.Unitize(); v32.Unitize();
            Vector3d v3 = v31 + v32;
            Line l3 = new Line(p3, v3, C);

            var events = Intersection.CurveCurve(l1.ToNurbsCurve(), l2.ToNurbsCurve(), DOC_TOLERANCE, DOC_TOLERANCE);
            Point3d centerPt = events[0].PointA;
            return new Circle(centerPt, R);
        }
        public List<Curve> MeshTriangle(Brep brep)//取得mesh的faceedge
        {
            var meshSetting = new MeshingParameters(0.5);
            Mesh[] brepMesh = Mesh.CreateFromBrep(brep, meshSetting);
            List<Curve> faceEdges = new List<Curve>();
            foreach(Mesh mesh in brepMesh)
            {
                mesh.Faces.ConvertQuadsToTriangles();
                MeshFaceList faceList = mesh.Faces;
                for(int i = 0; i < faceList.Count; i++)
                {
                    Point3f a, b, c, d;
                    faceList.GetFaceVertices(i, out a, out b, out c, out d);
                    Point3d[] array = { c, b, a, d };
                    Polyline triangle = new Polyline(array);
                    faceEdges.Add(triangle.ToNurbsCurve());
                }
            }
            return faceEdges;
        }
        public List<Curve> GetGoodFaceEdge(Brep brep, double filterValue)//筛选符合大小的faceEdge
        {
            List<Curve> faceEdges = MeshTriangle(brep);
            List<Curve> goodFaceEdges = new List<Curve>();
            foreach (Curve faceEdge in faceEdges)
            {
                List<Point3d> points = P2P_Path.GetDiscontinuityPoints(faceEdge);
                points.RemoveAt(0);
                Point3d p1 = points[0];
                Point3d p2 = points[1];
                Point3d p3 = points[2];
                double l1 = (p1 - p2).Length;
                double l2 = (p1 - p3).Length;
                double l3 = (p2 - p3).Length;
                double minLength = Math.Min(l1, l2);
                minLength = Math.Min(minLength, l3);
                if (minLength > filterValue)//筛掉太小的三角形
                {
                    goodFaceEdges.Add(faceEdge);
                }
            }
            return goodFaceEdges;
        }
        public static bool IsPointInsideBrep(Point3d point, Brep brep)
        {
            double C = 0;
            foreach(Curve edge in brep.Edges)
            {
                C += edge.GetLength();
            }
            Line testLine = new Line(point, Vector3d.ZAxis, C * 10);
            Curve[] overlapCrv;
            Point3d[] xPoints;
            Intersection.CurveBrep(testLine.ToNurbsCurve(), brep, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, out overlapCrv, out xPoints);
            if (xPoints.Length % 2 != 0)
            {
                return true;
            }
            else
                return false;
        }
        public void AddTreeToDoc(RhinoDoc doc)
        {
            var addedGeoIds = new List<Guid>();
            //var doc = Rhino.RhinoDoc.ActiveDoc;
            if (Trees != null)
            {
                if (AutoPlanPlugin.Instance.Dictionary.ContainsKey("TreeIDs"))
                {
                    Guid[] oldIDs = AutoPlanPlugin.Instance.Dictionary["TreeIDs"] as Guid[];
                    foreach (Guid id in oldIDs)
                    {
                        doc.Objects.Delete(id, true);
                    }
                }
                var layer = new Rhino.DocObjects.Layer() { Name = "Trees" };
                if (!doc.Layers.Any(x => x.Name == layer.Name))
                {
                    doc.Layers.Add(layer);
                }
                layer = doc.Layers.First(x => x.Name == "Trees");
                layer.Color = Color.Green;
                Guid[] treeIDs = new Guid[Trees.Count];
                for (int i = 0; i < Trees.Count; i++)
                {
                    var attribute = new Rhino.DocObjects.ObjectAttributes
                    {
                        ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromLayer,
                        LayerIndex = layer.Index
                    };
                    Guid id = doc.Objects.Add(Trees[i].TreeMesh, attribute);
                    treeIDs[i] = id;
                }
                AutoPlanPlugin.Instance.Dictionary.Set("TreeIDs", treeIDs);
                MyLib.MyLib.GreenArea = GreenArea;
                MyLib.MyLib.ConcentrationGreenArea = GreenArea * 0.4;
            }
            
        }
    }
}
