using Rhino.Geometry.Intersect;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan.PathFinder
{
    internal class GridCreation
    {
        public Point3d startPoint { get; set; }
        public Point3d endPoint { get; set; }
        public int resolution { get; set; }
        public Rectangle3d[,] Grids { get; set; }//格子
        public int[,] EnvValue { get; set; }//阵列中的0和1，代表是否能通过
        public Point3d[,] Points { get; set; }//点阵
        public int[] StartNodeId { get; set; }
        public int[] EndNodeId { get; set; }
        public Point3d startLocation { get; set; }
        public Point3d endLocation { get; set; }
        public int worldLength;
        public int worldWidth;
        List<Curve> Obstacles { get; set; }
        public GridCreation(Point3d startPoint, Point3d endPoint, int resolution, List<Curve> Obstacles)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.resolution = resolution;
            this.Obstacles = Obstacles;
            startLocation = startPoint;
            endLocation = endPoint;
            Curve seBox = new Rectangle3d(Plane.WorldXY, startPoint, endPoint).ToNurbsCurve();
            bool meetObstacle = false;

            foreach (Curve obstacle in Obstacles)
            {
                if (Curve.PlanarCurveCollision(seBox, obstacle, Plane.WorldXY, 0.001))
                    meetObstacle = true;
            }

            Box newbox;
            if (meetObstacle == false)
            {
                newbox = new Box(new BoundingBox(startPoint, endPoint));
            }
            else
            {
                //Rectangle3d rect = new Rectangle3d(Plane.WorldXY, this.pt1, this.pt2);
                Box box = new Box(GetObstacleBox(Obstacles));
                //Box box2 = new Box(Plane.WorldXY, new Point3d[] { this.startPoint, this.endPoint });
                box.Union(this.startPoint);
                box.Union(this.endPoint);
                Plane boxScalePlane = Plane.WorldXY;
                boxScalePlane.Origin = box.Center;
                Point3d ctPt = box.Center;
                Interval nbX = new Interval(ctPt.X - (ctPt.X - box.X.T0) * 2, ctPt.X + (box.X.T1 - ctPt.X) * 2);
                Interval nbY = new Interval(ctPt.Y - (ctPt.Y - box.Y.T0) * 2, ctPt.Y + (box.Y.T1 - ctPt.Y) * 2);
                //box = new Box(new Plane(ctPt, Vector3d.ZAxis), nbX, nbY, new Interval(0, 0));
                newbox = BoxCenterScale(box, 2, 2, 1);
            }



            //box.Transform(Transform.Scale(tPlane,2));
            Interval domainX = newbox.X;
            Interval domainY = newbox.Y;
            double length = domainX.Length;
            double width = domainY.Length;
            double distance = this.startPoint.DistanceTo(this.endPoint);
            double DPI = Math.Sqrt(length * length + width * width) / this.resolution;
            if (DPI > distance / 4)
                DPI = distance / 4;
            //DPI = 5;
            worldLength = Math.Max((int)Math.Ceiling((length - length % DPI) / DPI), 4);
            worldWidth = Math.Max((int)Math.Ceiling((width - width % DPI) / DPI), 4);
            List<Interval> segX = DivideDomain(domainX, worldLength);
            List<Interval> segY = DivideDomain(domainY, worldWidth);
            worldLength += 6;
            worldWidth += 6;
            Grids = new Rectangle3d[worldLength, worldWidth];
            Points = new Point3d[worldLength, worldWidth];
            EnvValue = new int[worldLength, worldWidth];
            List<int> startList = new List<int>();
            List<int> endList = new List<int>();
            for (int x = 0; x < worldLength; x++)
            {
                for (int y = 0; y < worldWidth; y++)
                {
                    Grids[x, y] = new Rectangle3d(Plane.WorldXY, segX[x], segY[y]);
                    Points[x, y] = Grids[x, y].Center;
                    int isStart = (int)Grids[x, y].Contains(this.startPoint);
                    startList.Add(isStart);
                    int isEnd = (int)Grids[x, y].Contains(this.endPoint);
                    endList.Add(isEnd);
                    if (isStart != 2)
                        StartNodeId = new int[] { x, y };
                    //if (isStart==1)
                    //    StartNodeId = new int[] { x, y };
                    if (isEnd != 2)
                        EndNodeId = new int[] { x, y };
                    //if (isEnd==1)
                    //    EndNodeId = new int[] { x, y };
                }
            }
            //this.startLocation = Points[StartNodeId[0], StartNodeId[1]];
            //this.endLocation = Points[EndNodeId[0], EndNodeId[1]];
            AdjustGrids();
            for (int x = 0; x < worldLength; x++)
            {
                for (int y = 0; y < worldWidth; y++)
                {
                    Points[x, y] = Grids[x, y].Center;
                    if (IsObstacle(Grids[x, y], this.Obstacles, DPI / 100))
                        EnvValue[x, y] = 0;
                    else EnvValue[x, y] = 1;
                }
            }
            //将起始和终点所在网格强制设置为可通行，以暂时解决一些bug，之后若有更完善优雅的方法再修改
            int nx = Math.Abs(StartNodeId[0] - EndNodeId[0]);
            int ny = Math.Abs(StartNodeId[1] - EndNodeId[1]);
            if (nx >= 2 && ny >= 2)
            {
                EnvValue[StartNodeId[0], StartNodeId[1]] = 1;
                EnvValue[EndNodeId[0], EndNodeId[1]] = 1;
            }
            //return box;
        }
        private void AdjustGrids()
        {
            Vector3d moveGridtoSPt = this.startLocation - this.Grids[this.StartNodeId[0], this.StartNodeId[1]].Center;
            for (int x = 0; x < worldLength; x++)
            {
                for (int y = 0; y < worldWidth; y++)
                {
                    Grids[x, y].Transform(Transform.Translation(moveGridtoSPt));
                }
            }
            Point3d endGridCenter = this.Grids[this.EndNodeId[0], this.EndNodeId[1]].Center;
            double scaleX;
            double scaleY;
            if (Math.Abs((this.startLocation.X - endGridCenter.X)) < 0.0001)
            {
                scaleX = 1;
            }
            else
            {
                scaleX = Math.Abs(this.startLocation.X - this.endLocation.X) / Math.Abs(this.startLocation.X - endGridCenter.X);
            }
            if (Math.Abs((this.startLocation.Y - endGridCenter.Y)) < 0.0001)
            {
                scaleY = 1;
            }
            else
            {
                scaleY = Math.Abs(this.startLocation.Y - this.endLocation.Y) / Math.Abs(this.startLocation.Y - endGridCenter.Y);
            }
            Plane startPtPlane = Plane.WorldXY;
            startPtPlane.Origin = startLocation;
            for (int x = 0; x < worldLength; x++)
            {
                for (int y = 0; y < worldWidth; y++)
                {
                    Grids[x, y].Transform(Transform.Scale(startPtPlane, scaleX, scaleY, 1));
                    //Grids[x, y] = RecScale(Grids[x, y], startPtPlane, scaleX, scaleY);
                }
            }
        }
        private static bool IsObstacle(Rectangle3d grid, List<Curve> obstacles, double tolerence)
        {
            bool isObstacle = false;
            if (obstacles.Count == 0) isObstacle = false;
            Curve mycurve = grid.ToNurbsCurve();

            foreach (Curve obstacle in obstacles)
            {
                CurveIntersections intersect = Intersection.CurveCurve(mycurve, obstacle, tolerence, 0);
                if (intersect.Count > 0) isObstacle = true;
                if ((int)obstacle.Contains(grid.Center) == 1) isObstacle = true;
            }
            return isObstacle;
        }
        private List<Interval> DivideDomain(Interval domain, int res)//res为分段数量，分段后向前后各延展2个subdomain
        {
            double t0 = domain.T0;
            double t1 = domain.T1;
            double l = (domain.Length / (double)res);
            List<Interval> subdomains = new List<Interval>();
            subdomains.Add(new Interval(t0 - l * 3, t0 - l * 2));
            subdomains.Add(new Interval(t0 - l * 2, t0 - l));
            subdomains.Add(new Interval(t0 - l, t0));
            for (int i = 0; i < res; i++)
            {
                subdomains.Add(new Interval(t0 + i * l, t0 + (i + 1) * l));
            }
            subdomains.Add(new Interval(t1, t1 + l));
            subdomains.Add(new Interval(t1 + l, t1 + l * 2));
            subdomains.Add(new Interval(t1 + l * 2, t1 + l * 3));
            return subdomains;
        }
        private BoundingBox GetObstacleBox(List<Curve> obstacles)
        {
            BoundingBox bBox = BoundingBox.Unset;
            foreach (Curve obstacle in obstacles)
            {
                BoundingBox bbObj = obstacle.GetBoundingBox(true);
                bBox.Union(bbObj);
            }
            return bBox;
        }
        private Box BoxCenterScale(Box box, double scaleX, double scaleY, double scaleZ)
        {
            Point3d centerPoint = box.Center;
            Interval oX = box.X;
            Interval oY = box.Y;
            Interval oZ = box.Z;
            Interval newX = new Interval(centerPoint.X - (centerPoint.X - oX.T0) * scaleX, centerPoint.X + (oX.T1 - centerPoint.X) * scaleX);
            Interval newY = new Interval(centerPoint.Y - (centerPoint.Y - oY.T0) * scaleY, centerPoint.Y + (oY.T1 - centerPoint.Y) * scaleY);
            Interval newZ = new Interval(centerPoint.Z - (centerPoint.Z - oZ.T0) * scaleZ, centerPoint.Z + (oX.T1 - centerPoint.Z) * scaleZ);
            return new Box(new Plane(centerPoint, Vector3d.ZAxis), newX, newY, newZ);
        }
        private Rectangle3d RecScale(Rectangle3d rec, Plane scalePlane, double scaleX, double scaleY)
        {
            Point3d anchorPoint = scalePlane.Origin;
            double anchorX = anchorPoint.X;
            double anchorY = anchorPoint.Y;
            double anchorZ = anchorPoint.Z;
            Interval oX = rec.X;
            Interval oY = rec.Y;
            Point3d recCenter = rec.Center;
            double iX0 = Math.Abs(oX.T0 - anchorX) * scaleX;//i means:implement for increasing or decreasing from original x
            double iX1 = Math.Abs(oX.T1 - anchorX) * scaleX;
            double iY0 = Math.Abs(oY.T1 - anchorY) * scaleY;
            double iY1 = Math.Abs(oY.T1 - anchorY) * scaleY;
            double newX0 = 0, newX1 = 0;//new bounds for x interval
            double newY0 = 0, newY1 = 0;
            if (anchorX > oX.T0)
            {
                newX0 = anchorX - iX0;
            }
            else
            {
                newX0 = anchorX + iX0;
            }
            if (anchorX > oX.T1)
            {
                newX1 = anchorX - iX1;
            }
            else
            {
                newX1 = anchorX + iX1;
            }//new x interval complete

            if (anchorY > oY.T0)
            {
                newY0 = anchorY - iY0;
            }
            else
            {
                newY0 = anchorY + iY0;
            }
            if (anchorY > oY.T1)
            {
                newY1 = anchorY - iY1;
            }
            else
            {
                newY1 = anchorY + iY1;
            }//new y interval complete
            Interval newX = new Interval(newX0, newX1);
            Interval newY = new Interval(newY0, newY1);

            double newRecCenterX = 0, newRecCenterY = 0;
            double iCenterX = Math.Abs(recCenter.X - anchorX) * scaleX;
            double iCenterY = Math.Abs(recCenter.Y - anchorY) * scaleY;
            if (anchorX > recCenter.X)
            {
                newRecCenterX = anchorX - iCenterX;
            }
            else
            {
                newRecCenterX = anchorX + iCenterX;
            }
            if (anchorY > recCenter.Y)
            {
                newRecCenterY = anchorY - iCenterY;
            }
            else
            {
                newRecCenterY = anchorY + iCenterY;
            }

            Plane newRecPlane = new Plane(new Point3d(newRecCenterX, newRecCenterY, recCenter.Z), Vector3d.ZAxis);
            return new Rectangle3d(newRecPlane, newX, newY);
        }
        //public Rectangle3d GridPoint()
        //{
        //Rectangle3d rect = new Rectangle3d(Plane.WorldXY, this.pt1, this.pt2);
        //double xD = Math.Abs(pt1.X - pt2.X);
        //double yD = Math.Abs(pt1.Y - pt2.Y);
        //double maxD = Math.Max(xD, yD);
        //double minD = Math.Min(xD, yD);
        //List<Point3d> ptGrid = new List<Point3d>();
        //double x = maxD / this.dpi;

        //for(int i = 0; i < this.dpi; i++)
        //{
        //    for(int j = 0; j < minD / (maxD / this.dpi); j++)
        //    {
        //        if (xD < yD)
        //        {

        //        }
        //    }
        //}

        //return rect;
        //}
    }
}
