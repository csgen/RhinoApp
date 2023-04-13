using AutoPlan.AutoPlan.PathFinder;
using Eto.Forms;
using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPlan.AutoPlan
{
    internal class P2P_Path : Path
    {
        //public double Width { get; set; }
        //public Curve MidCurve { get; set; }
        //public double FilletRadi { get; set; }
        public RhinoDoc doc { private get; set; }
        public Guid id;
        public Guid ID
        {
            get => id;
            set
            {
                id = value;
                MidCurve = new ObjRef(doc,id).Curve();//指定ID时关联midcurve
            }
        }
        public Point3d StartPoint { get; set; }
        public Point3d EndPoint { get; set; }
        public List<BaseBuilding> BaseBuildings { get; set; }
        //public Building BaseBuilding { get; set; }
        public double BaseBuildingtValue { get; set; }
        public Guid BaseBuildingID { get; set; }
        public Path BasePath { get; set; }
        public ArchivableDictionary ClassData { get; set; }
        public class BaseBuilding
        {
            public Building Building { get; set; }
            public double tValue { get; set; }
        }
        public P2P_Path(Curve curve, PlaneObjectManager planeObjectM)
        {
            MidCurve = curve;
            FilletRadi = 1;
            Width = 4;
            Point3d p1 = MidCurve.PointAtStart;
            Point3d p2 = MidCurve.PointAtEnd;
            List<Point3d> points = new List<Point3d> { p1, p2 };
            StartPoint = PairPoint(points, planeObjectM.Buildings)[0];
            EndPoint = PairPoint(points, planeObjectM.Buildings)[1];
        }
        public P2P_Path(Guid id, PlaneObjectManager planeObjectM)//用于选择已有P2P,将P2P_Path所有几何信息与guid挂钩
        {
            ID = id;
            FilletRadi = 1;
            Width = 4;//宽度太小创建Object时有问题，可能是boolean/fillet其中之一的bug，目前经验值最小是4
            //MidCurve = curve;
            Point3d p1 = MidCurve.PointAtStart;
            Point3d p2 = MidCurve.PointAtEnd;
            List<Point3d> points = new List<Point3d> { p1, p2 };
            StartPoint = PairPoint(points, planeObjectM.Buildings)[0];
            EndPoint = PairPoint(points, planeObjectM.Buildings)[1];
            BaseBuildings = new List<BaseBuilding>();
        }
        public P2P_Path(List<Point3d> points, PlaneObjectManager planeObjectM)//用于新绘制建立P2P
        {
            FilletRadi = 1;
            Width = 4;
            
            StartPoint = PairPoint(points, planeObjectM.Buildings)[0];
            EndPoint = PairPoint(points, planeObjectM.Buildings)[1];
            List<Path> pathList = new List<Path>();
            //pathList.AddRange(planeObjectM.MainPath);
            //pathList.AddRange(planeObjectM.P2P_Path);
            //pathList.Add(planeObjectM.OuterPath);
            MidCurve = CreatePath(planeObjectM);
            //SetData();
        }
        //public P2P_Path(PlaneObjectManager planeObjectM)//用于从basebuilding重建P2P
        //{
        //    int n = this.BaseBuildings.Count;
        //    if(n)
        //}
        //public void SetData()
        //{
        //    ClassData = new ArchivableDictionary();
        //    ClassData.Set("BaseBuilding", BaseBuilding.BuildingCurve);
        //    ClassData.Set("BasePath", BasePath.MidCurve);
        //    ClassData.Set("Width", Width);
        //    //ArchivableDictionary dictionary = new ArchivableDictionary();
        //    //dictionary.Set("BaseBuilding", );
        //    //UserData userData = new UserData();
        //}
        public Curve CreatePath(PlaneObjectManager planeObjectM)//画出完整路径，包括起点针对楼栋和道路各自的偏移(之后可以加上倒角)
        {
            //Point3d secondPoint = Get2ndPoint(buildings);

            //Point3d secondLastPoint = Get2ndLastPoint(paths);
            List<Point3d> points = GetSolverPoints(planeObjectM);
            
            List<Curve> obstacles = new List<Curve>();
            foreach (Building building in planeObjectM.Buildings)
            {
                Curve obstacle = PaddingBox(building.BuildingCurve, building.AvoidDistance);
                obstacles.Add(obstacle);
            }
            PathSolver pathS;
            if (points[1] != Point3d.Unset)
            {
                pathS = new PathSolver(points[0], points[1], obstacles, 40, true);
                List<Point3d> ptList = pathS.PathRhinoSolver();
                ptList.Insert(0, StartPoint);
                ptList.Add(EndPoint);
                Polyline c = new Polyline(ptList);
                if (c == null)
                {
                    System.Windows.MessageBox.Show("error");
                }
                List<Point3d> simplifiedPts = GetDiscontinuityPoints(new Polyline(ptList).ToNurbsCurve());
                Polyline path = new Polyline(simplifiedPts);
                return path.ToNurbsCurve();
            }
            else
            {
                pathS = new PathSolver(points[0], EndPoint, obstacles, 40, true);
                List<Point3d> ptList = pathS.PathRhinoSolver();
                ptList.Insert(0, StartPoint);
                if (ptList.Count == 1)
                {
                    ptList.Add(EndPoint);
                }
                Curve c = new Polyline(ptList).ToNurbsCurve();
                if (c == null)
                {
                    System.Windows.MessageBox.Show("error");
                }
                List<Point3d> simplifiedPts = GetDiscontinuityPoints(c);
                Polyline path = new Polyline(simplifiedPts);
                return path.ToNurbsCurve();
            }
            //PathSolver pathS = new PathSolver(secondPoint, secondLastPoint, obstacles, 40, true);
            //Polyline pathCreate = pathS.PathRhinoSolver();
            //List<Point3d> ptList = GetDiscontinuityPoints(pathCreate.ToNurbsCurve());
        }
        private Curve PaddingBox(Curve inputCrv, double padding)
        {
            Curve paddingCurve = inputCrv.Offset(Plane.WorldXY, padding, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];
            
            return paddingCurve;
        }
        private static List<Point3d> PairPoint(List<Point3d> points, List<Building> buildings)//分出起始点（连接building的点）和终点
        {
            List<Point3d> pairedPt = new List<Point3d>();
            if (points.Count == 2)
            {
                Point3d startPt = points[0];
                Point3d endPt = points[1];
                //List<Curve> buildingPlans = new List<Curve>();
                for (int i = 0; i < buildings.Count; i++)
                {
                    Curve baseCurve = buildings[i].BuildingCurve;
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
            BaseBuildings = new List<BaseBuilding>();
            //Building basebuilding = buildings[0];
            double t = 0;
            foreach (Building building in buildings)
            {
                Curve basePlan = building.BuildingCurve;
                bool findbase = basePlan.ClosestPoint(StartPoint, out t, 1);
                if (findbase)
                {
                    BaseBuilding bb = new BaseBuilding();
                    bb.Building = building;
                    bb.tValue = t;
                    //basebuilding = building;
                    BaseBuildings.Add(bb);
                    break;
                }
            }
            foreach (Building building in buildings)
            {
                Curve basePlan = building.BuildingCurve;
                bool findbase = basePlan.ClosestPoint(EndPoint, out t, 1);
                if (findbase)
                {
                    BaseBuilding bb = new BaseBuilding();
                    bb.Building = building;
                    bb.tValue = t;
                    //basebuilding = building;
                    BaseBuildings.Add(bb);
                    break;
                }
            }
            //BaseBuilding = basebuilding;
            //BaseBuildingID = basebuilding.ID;
            BaseBuildingtValue = t;
            //return basebuilding;
        }
        private void GetBasePath(PlaneObjectManager planeObjectM)//找到点的附着路径(如果有的话)
        {
            Path basePath = null;
            foreach (Path path in planeObjectM.Paths)
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
        private Point3d GetBuildingOffsetPoint(Building baseBuilding, Point3d point, double distance)
        {
            //GetBaseBuilding(buildings);
            Curve paddingCrv = PaddingBox(baseBuilding.BuildingCurve, baseBuilding.AvoidDistance + distance);
            bool findScondPt = paddingCrv.ClosestPoint(point, out double secondPoint_t);
            Point3d secondPoint = paddingCrv.PointAt(secondPoint_t);//找到路径上处于paddingbox上的点,即路径上的第二个点
            return secondPoint;
        }
        private Point3d GetPathOffsetPoint()
        {
            //GetBasePath(paths);
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
        private List<Point3d> GetSolverPoints(PlaneObjectManager planeObjectM)//第一个点附着于building，第二个点有三种可能（1.附于building，2.附于Path，3.为空）
        {
            Point3d point1 = Point3d.Unset;
            Point3d point2 = Point3d.Unset;
            GetBaseBuilding(planeObjectM.Buildings);
            if (BaseBuildings.Count == 1)
            {
                Building baseBuilding = BaseBuildings[0].Building;
                point1 = GetBuildingOffsetPoint(baseBuilding,StartPoint,2);
                GetBasePath(planeObjectM);
                if (BasePath != null)
                {
                    point2 = GetPathOffsetPoint();
                }
            }
            if (BaseBuildings.Count == 2)
            {
                Building baseBuilding1 = BaseBuildings[0].Building;
                Building baseBuilding2 = BaseBuildings[1].Building;
                point1 = GetBuildingOffsetPoint(baseBuilding1,StartPoint,10);
                point2 = GetBuildingOffsetPoint(baseBuilding2,EndPoint,10);
            }
            List<Point3d> points = new List<Point3d> { point1, point2 };
            return points;
        }
        public static List<Point3d> GetDiscontinuityPoints(Curve curve)
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
        public static bool ReplayHistory(int historyVersion, ReplayHistoryData replay,Rhino.Commands.Command command,PlaneObjectManager planeObjectM)
        {
            P2P_Path p2p_path;
            //Brep[] p2p_Paths;
            List<ObjRef> refBuilidngs = new List<ObjRef>();
            List<Point3d> points = new List<Point3d>();
            Guid baseBuildingID = Guid.Empty;
            double baseBuildingtValue = 0;
            if (!ReadHistory(historyVersion, replay, ref refBuilidngs, ref points, ref baseBuildingtValue, ref baseBuildingID))
                return false;

            if (refBuilidngs.Count != 0) { planeObjectM.Buildings = new List<Building>(); }

            Curve baseBuildingCurve = refBuilidngs[0].Curve();
            for(int i = 0; i < refBuilidngs.Count; i++)
            {
                ArchivableDictionary userDict = refBuilidngs[i].Curve().UserDictionary;
                var x = userDict["AvoidDistance"];
                var y = userDict["GUID"];
                double avoidDist = (double)x;
                Guid id = (Guid)y;
                Building building = new Building(id, RhinoDoc.ActiveDoc, avoidDist);
                building.ID = id;
                planeObjectM.Buildings.Add(building);//更新Buildings
                if(id == baseBuildingID)
                {
                    baseBuildingCurve = refBuilidngs[i].Curve();
                }
            }
            Point3d startPt = baseBuildingCurve.PointAt(baseBuildingtValue);
            List<Point3d> pathPoints = new List<Point3d>() { startPt, points[3] };
            p2p_path = new P2P_Path(pathPoints, planeObjectM);
            replay.Results[0].UpdateToCurve(p2p_path.MidCurve, null);
            return true;
        }
        private static bool ReadHistory
            (
            int historyVersion, ReplayHistoryData replay, 
            ref List<ObjRef> refBuildings, 
            ref List<Point3d> points,
            ref double baseBuildingtValue, 
            ref Guid baseBuildingID
            )
        {
            points = new List<Point3d>();
            if (historyVersion != replay.HistoryVersion)
                return false;
            int n;
            replay.TryGetInt(0, out n);//n为building数量
            for(int i = 0; i < n; i++)
            {
                ObjRef objref = replay.GetRhinoObjRef(i+1);//id是否从0开始？调试后确认
                if (null == objref)
                    return false;
                refBuildings.Add(objref);
            }
            Point3d pt0, pt1, startPoint, endPoint;
            if (!replay.TryGetPoint3d(n + 1, out pt0))
                return false;
            if (!replay.TryGetPoint3d(n + 2, out pt1))
                return false;
            if (!replay.TryGetPoint3d(n + 3, out startPoint))
                return false;
            if (!replay.TryGetPoint3d(n + 4, out endPoint))
                return false;
            if (!replay.TryGetDouble(n + 5, out baseBuildingtValue))
                return false;
            if (!replay.TryGetGuid(n + 6, out baseBuildingID))
                return false;

            points.Add(pt0);
            points.Add(pt1);
            points.Add(startPoint);
            points.Add(endPoint);
            return true;
        }
        public static bool WriteHistory(HistoryRecord history, List<ObjRef> refBuildings, List<Point3d> points, P2P_Path path)
        {
            int n = refBuildings.Count;
            if(!history.SetInt(0,n))
                return false;
            for(int i = 0; i < n; i++)
            {
                if (!history.SetObjRef(i+1, refBuildings[i]))
                    return false;
            }
            if (!history.SetPoint3d(n + 1, points[0]))
                return false;
            if (!history.SetPoint3d(n + 2, points[1]))
                return false;
            if (!history.SetPoint3d(n + 3, path.StartPoint))
                return false;
            if (!history.SetPoint3d(n + 4, path.EndPoint))
                return false;
            if (!history.SetDouble(n + 5, path.BaseBuildingtValue))
                return false;
            if (!history.SetGuid(n + 6, path.BaseBuildingID))
                return false;
            //if (!history.SetCurve(2, BaseBuilding.BuildingCurve.ToNurbsCurve()))
            //    return false;
                
            //if (!history.SetCurve(3 + planeObjectM.Buildings.Count, BasePath.MidCurve))
            //    return false;
            return true;
        }
    }
}
