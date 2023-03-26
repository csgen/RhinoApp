using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBuilding.Sitebuilding
{
    internal static class Toolkit
    {
        public static Point3d GetPoint(Point3d pt, Vector3d dir)
        {
            var x_cor = -1 * (dir.X / dir.Z) * pt.Z + pt.X;
            var y_cor = -1 * (dir.Y / dir.Z) * pt.Z + pt.Y;
            return new Point3d(x_cor, y_cor, 0);
        }

        public static double Intersept(Point3d pt, Vector3d dir)
        {
            return -1 * (dir.Y / dir.X) * pt.X + pt.Y;
        }

        public static Point3d GetIntersectPoint(Point3d p1, Point3d p2, Point3d p3, Point3d p4)
        {
            var x_dem = p4.X * p2.Y - p4.X * p1.Y - p3.X * p2.Y + p3.X * p1.Y
              - p2.X * p4.Y + p2.X * p3.Y + p1.X * p4.Y - p1.X * p3.Y;
            var x_cor = p3.Y * p4.X * p2.X - p4.Y * p3.X * p2.X - p3.Y * p4.X * p1.X + p4.Y * p3.X * p1.X
              - p1.Y * p2.X * p4.X + p2.Y * p1.X * p4.X + p1.Y * p2.X * p3.X - p2.Y * p1.X * p3.X;

            var y_dem = p4.Y * p2.X - p4.Y * p1.X - p3.Y * p2.X + p3.Y * p1.X
              - p2.Y * p4.X + p2.Y * p3.X + p1.Y * p4.X - p1.Y * p3.X;
            var y_cor = -p3.Y * p4.X * p2.Y + p4.Y * p3.X * p2.Y + p3.Y * p4.X * p1.Y - p4.Y * p3.X * p1.Y
              + p1.Y * p2.X * p4.Y - p1.X * p2.Y * p4.Y - p1.Y * p2.X * p3.Y + p2.Y * p1.X * p3.Y;

            return new Point3d(x_cor / x_dem, y_cor / y_dem, 0);
        }

        public static bool TestIntersection(Point3d p_1, Point3d p_2, Point3d p_3, Point3d p_4)
        {
            var p1 = new Point3d(p_1.X, p_1.Y, 0);
            var p2 = new Point3d(p_2.X, p_2.Y, 0);
            var p3 = new Point3d(p_3.X, p_3.Y, 0);
            var p4 = new Point3d(p_4.X, p_4.Y, 0);

            if (Math.Max(p3.X, p4.X) < Math.Min(p1.X, p2.X) ||
              Math.Max(p1.X, p2.X) < Math.Min(p3.X, p4.X) ||
              Math.Max(p3.Y, p4.Y) < Math.Min(p1.Y, p2.Y) ||
              Math.Max(p1.Y, p2.Y) < Math.Min(p3.Y, p4.Y))
            {
                return false;
            }

            if (Vector3d.CrossProduct(p1 - p4, p3 - p4) * Vector3d.CrossProduct(p2 - p4, p3 - p4) > 0 ||
            Vector3d.CrossProduct(p4 - p2, p1 - p2) * Vector3d.CrossProduct(p3 - p2, p1 - p2) > 0)
            {
                return false;
            }
            return true;
        }

        public static Line CheckDistance(PolylineCurve p1, PolylineCurve p2)
        {
            var dist = double.MaxValue;
            Line line = new Line();

            for (int i = 0; i < p1.PointCount - 1; i++)
            {
                var pt = p1.Point(i);
                double t;
                var result = p2.ClosestPoint(pt, out t);
                double distance = pt.DistanceTo(p2.PointAt(t));

                if (distance < dist)
                {
                    line = new Line(pt, p2.PointAt(t));
                    dist = distance;
                }
            }
            for (int i = 0; i < p2.PointCount - 1; i++)
            {
                var pt = p2.Point(i);
                double t;
                var result = p1.ClosestPoint(pt, out t);
                double distance = pt.DistanceTo(p1.PointAt(t));

                if (distance < dist)
                {
                    line = new Line(pt, p1.PointAt(t));
                    dist = distance;
                }
            }
            return line;
        }
    }
}
