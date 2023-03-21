using AutoPlan.AutoPlan.PathFinder;
using Eto.Forms;
using Rhino.Collections;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class P2P_Path : Path
    {
        //public double Width { get; set; }
        //public Curve MidCurve { get; set; }
        //public double FilletRadi { get; set; }
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }
        public Building BaseBuilding { get; set; }
        public Path BasePath { get; set; }
        public ArchivableDictionary ClassData { get; set; }
        public P2P_Path()
        {
            FilletRadi = 2;
            Width = 3;
        }
        public P2P_Path(List<Point3d> points, PlaneObjectManager planeObjectM)
        {
            FilletRadi = 2;
            Width = 3;
            StartPoint = PairPoint(points, planeObjectM.Buildings)[0];
            EndPoint = PairPoint(points, planeObjectM.Buildings)[1];
            List<Path> pathList = new List<Path>();
            pathList.AddRange(planeObjectM.MainPath);
            pathList.AddRange(planeObjectM.P2P_Path);
            pathList.Add(planeObjectM.OuterPath);
            MidCurve = CreatePath(planeObjectM.Buildings, pathList);
            SetData();
        }
        public void SetData()
        {
            ClassData = new ArchivableDictionary();
            ClassData.Set("BaseBuilding", BaseBuilding.BuildingCurve.ToNurbsCurve());
            ClassData.Set("BasePath", BasePath.MidCurve);
            ClassData.Set("Width", Width);
            //ArchivableDictionary dictionary = new ArchivableDictionary();
            //dictionary.Set("BaseBuilding", );
            //UserData userData = new UserData();
        }
        public Curve CreatePath(List<Building> buildings, List<Path> paths)//画出完整路径，包括起点针对楼栋和道路各自的偏移
        {
            Point3d secondPoint = Get2ndPoint(buildings);

            Point3d secondLastPoint = Get2ndLastPoint(paths);

            List<Curve> obstacles = new List<Curve>();
            foreach (Building building in buildings)
            {
                Curve obstacle = PaddingBox(building.BuildingCurve, building.AvoidDistance);
                obstacles.Add(obstacle);
            }
            PathSolver pathS = new PathSolver(secondPoint, secondLastPoint, obstacles, 40, true);
            //Polyline pathCreate = pathS.PathRhinoSolver();
            //List<Point3d> ptList = GetDiscontinuityPoints(pathCreate.ToNurbsCurve());
            List<Point3d> ptList = pathS.PathRhinoSolver();
            ptList.Insert(0, StartPoint);
            ptList.Add(EndPoint);
            List<Point3d> simplifiedPts = GetDiscontinuityPoints(new Polyline(ptList).ToNurbsCurve());
            Polyline path = new Polyline(simplifiedPts);
            return path.ToNurbsCurve();

        }
        public Curve PaddingBox(Rectangle3d inputCrv, double padding)
        {
            Plane basePlane = inputCrv.Plane;
            Interval domainX = new Interval(inputCrv.X.T0 - padding, inputCrv.X.T1 + padding);
            Interval domainY = new Interval(inputCrv.Y.T0 - padding, inputCrv.Y.T1 + padding);
            Rectangle3d paddingBox = new Rectangle3d(basePlane, domainX, domainY);
            return paddingBox.ToNurbsCurve();
        }
        public List<Point3d> PairPoint(List<Point3d> points, List<Building> buildings)
        {
            List<Point3d> pairedPt = new List<Point3d>();
            if (points.Count == 2)
            {
                Point3d startPt = points[0];
                Point3d endPt = points[1];
                //List<Curve> buildingPlans = new List<Curve>();
                for (int i = 0; i < buildings.Count; i++)
                {
                    Curve baseCurve = buildings[i].BuildingCurve.ToNurbsCurve();
                    double t1, t2;
                    bool b1 = baseCurve.ClosestPoint(startPt, out t1, 1);
                    bool b2 = baseCurve.ClosestPoint(endPt, out t2, 1);
                    if (!b1 && b2)
                    {
                        startPt = points[1];
                        endPt = points[0];

                    }
                    if (b1 && b2)
                    {
                        if (startPt.DistanceTo(baseCurve.PointAt(t1)) > endPt.DistanceTo(baseCurve.PointAt(t2)))
                        {
                            startPt = points[1];
                            endPt = points[0];

                        }
                    }
                }
                pairedPt.Add(startPt);
                pairedPt.Add(endPt);
            }
            return pairedPt;
        }
        private void GetBaseBuilding(List<Building> buildings)
        {
            Building basebuilding = buildings[0];
            foreach (Building building in buildings)
            {
                Curve basePlan = building.BuildingCurve.ToNurbsCurve();
                bool findbase = basePlan.ClosestPoint(StartPoint, out double t, 1);
                if (findbase)
                {
                    basebuilding = building;
                    break;
                }
            }
            BaseBuilding = basebuilding;
            //return basebuilding;
        }
        private void GetBasePath(List<Path> paths)//找到点的附着路径
        {
            Path basePath = paths[0];
            foreach (Path path in paths)
            {
                bool findbase = path.MidCurve.ClosestPoint(EndPoint, out double t, 1);
                if (findbase)
                {
                    basePath = path;
                    break;
                }
            }
            BasePath = basePath;
            //return basePath;
        }
        private Point3d Get2ndPoint(List<Building> buildings)
        {
            GetBaseBuilding(buildings);
            Curve paddingCrv = PaddingBox(BaseBuilding.BuildingCurve, BaseBuilding.AvoidDistance + 0.5);
            bool findScondPt = paddingCrv.ClosestPoint(StartPoint, out double secondPoint_t);
            Point3d secondPoint = paddingCrv.PointAt(secondPoint_t);//找到路径上处于paddingbox上的点,即路径上的第二个点
            return secondPoint;
        }
        private Point3d Get2ndLastPoint(List<Path> paths)
        {
            GetBasePath(paths);
            //Path basePath = GetBasePath(paths);
            double pathAvoid = BasePath.Width / 2 + 2;
            Curve offsetCrv1 = BasePath.MidCurve.Offset(Plane.WorldXY, pathAvoid, 0.001, CurveOffsetCornerStyle.Sharp)[0];
            Curve offsetCrv2 = BasePath.MidCurve.Offset(Plane.WorldXY, -pathAvoid, 0.001, CurveOffsetCornerStyle.Sharp)[0];
            offsetCrv1.ClosestPoint(EndPoint, out double last2ndPoint_t1);
            offsetCrv2.ClosestPoint(EndPoint, out double last2ndPoint_t2);
            Point3d last2ndPoint1 = offsetCrv1.PointAt(last2ndPoint_t1);
            Point3d last2ndPoint2 = offsetCrv2.PointAt(last2ndPoint_t2);
            Point3d last2ndPoint = last2ndPoint1;
            if (StartPoint.DistanceTo(last2ndPoint1) > StartPoint.DistanceTo(last2ndPoint2))
            {
                last2ndPoint = last2ndPoint2;
            }
            return last2ndPoint;
        }
        public List<Point3d> GetDiscontinuityPoints(Curve curve)
        {
            double t0 = curve.Domain.Min;
            double t1 = curve.Domain.Max;
            double t;
            List<Point3d> discontinuityList = new List<Point3d>();
            do
            {
                discontinuityList.Add(curve.PointAt(t0));
                if (!curve.GetNextDiscontinuity(Continuity.C1_locus_continuous, t0, t1, out t)) { break; }

                Interval trim = new Interval(t0, t);
                if (trim.Length < 1e-10)
                {
                    t0 = t;
                    continue;
                }
                t0 = t;
            }
            while (true);
            return discontinuityList;
        }
    }
}
