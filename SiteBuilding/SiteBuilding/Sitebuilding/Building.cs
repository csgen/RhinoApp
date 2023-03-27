using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBuilding.Sitebuilding
{
    internal class Building
    {
        public Plane basePlane;
        public double Height { get; set; }
        public PolylineCurve Profile { get; set; }
        public List<Point3d> Corners { get; set; }
        public PolylineCurve Shadow { get; set; }

        public Building(PolylineCurve profile)
        {
            Profile = profile;
            Initialization();


        }
        public Building(Plane pl, double width, double length)
        {
            basePlane = pl;
            Profile = new Rectangle3d(pl,
                new Interval(-0.5 * width, 0.5 * width),
                new Interval(-0.5 * length, 0.5 * length))
                .ToPolyline()
                .ToPolylineCurve();
            Initialization();

        }

        void Initialization()
        {
            GetCorners();
            GetShadow();
        }

        public void GetShadow()
        {
            var v1 = SunLight.light_start;
            var v2 = SunLight.light_end;

            var corner1 = Corners
                .OrderBy(x => -1 * Toolkit.Intersept(x, v1))
                .First();
            var corner1_height = corner1 + new Vector3d(0, 0, 10);
            var p1 = Toolkit.GetPoint(corner1_height, v1);

            var corner2 = Corners
                .OrderBy(x => -1 * Toolkit.Intersept(x, v2))
                .First();
            var corner2_height = corner1 + new Vector3d(0, 0, 10);
            var p2 = Toolkit.GetPoint(corner2_height, v2);

             
            var result = Toolkit.TestIntersection(corner1_height, p1, corner2_height, p2);

            if (!result)
            {
                Shadow = new PolylineCurve(new List<Point3d>() { corner1, p1, p2, corner2 });
            }
            else
            {
                var intersectPt = Toolkit.GetIntersectPoint(p1, corner1, p2, corner2);
                Shadow = new PolylineCurve(new List<Point3d>() { corner1, intersectPt, corner2 });
            }
        }

        void GetCorners()
        {
            Corners = new List<Point3d>();
            for (int i = 0; i < Profile.PointCount; i++)
            {
                Corners.Add(Profile.PointAt(i));
            }
        }

        public void CheckDistance(Building building)
        {
            var p1 = Profile;
            var p2 = building.Profile;
            var line = Toolkit.CheckDistance(p1, p2);
        }

        public double GetArea()
        {
            var area = AreaMassProperties.Compute(this.Profile, 0.001).Area;
            return area;
        }

    }
}
