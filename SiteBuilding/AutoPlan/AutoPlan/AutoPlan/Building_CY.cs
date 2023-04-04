using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoPlan.BuildingShadow;
using Rhino;
using Rhino.DocObjects;
using Rhino.DocObjects.Tables;

namespace AutoPlan.AutoPlan
{
    internal partial class Building
    {
        public Plane basePlane;
        public double Height { get; set; }
        public PolylineCurve Profile { get; set; }
        public List<Point3d> Corners { get; set; }
        public PolylineCurve Shadow { get; set; }
        public BuildingShadow buildingShadow { get; set; }

        //public Building(PolylineCurve profile)
        //{
        //    Profile = profile;
        //    Initialization();
        //}
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
            this.buildingShadow = new BuildingShadow();
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
            var corner1_height = corner1 + new Vector3d(0, 0, 1000);
            var p1 = Toolkit.GetPoint(corner1_height, v1);

            var corner2 = Corners
                .OrderBy(x => -1 * Toolkit.Intersept(x, v2))
                .First();
            var corner2_height = corner1 + new Vector3d(0, 0, 1000);
            var p2 = Toolkit.GetPoint(corner2_height, v2);


            var result = Toolkit.TestIntersection(corner1_height, p1, corner2_height, p2);

            if (!result)
            {
                Shadow = new PolylineCurve(new List<Point3d>() { corner1, p1, p2, corner2, corner1 });
                this.buildingShadow.Boundary = new PolylineCurve(new List<Point3d>() { corner1, p1, p2, corner2, corner1 });
            }
            else
            {
                var intersectPt = Toolkit.GetIntersectPoint(p1, corner1, p2, corner2);
                Shadow = new PolylineCurve(new List<Point3d>() { corner1, intersectPt, corner2, corner1 });
                this.buildingShadow.Boundary = new PolylineCurve(new List<Point3d>() { corner1, intersectPt, corner2, corner1 });
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

        public LinearDimension GetLinearAnnotation(Building building)
        {
            var p1 = Profile;
            var p2 = building.Profile;
            var line = Toolkit.CheckDistance(p1, p2);
            LinearDimension dimension = null;
            if (line.Length <= 13)
            {
                dimension = AddLinearDimension(line);
            }
            return dimension;
        }

        public LinearDimension AddLinearDimension(Line line)
        {
            Point3d ptS = line.From;
            Point3d ptE = line.To;
            Curve lineOffset = line.ToNurbsCurve().Offset(Plane.WorldXY, 10, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance, CurveOffsetCornerStyle.Sharp)[0];
            Point3d ptAnnotation = (lineOffset.PointAtEnd + lineOffset.PointAtStart) / 2;
            Vector3d vx = ptS - ptE;
            Vector3d vy = lineOffset.PointAtStart - ptS;
            vx.Unitize();
            vy.Unitize();
            Plane plane = Plane.WorldXY;
            plane.Origin = ptS;
            plane.XAxis = vx;
            plane.YAxis = vy;
            plane.ZAxis = Vector3d.ZAxis;
            double u, v;
            plane.ClosestParameter(ptS, out u, out v);
            Point2d ext1 = new Point2d(u, v);

            plane.ClosestParameter(ptE, out u, out v);
            Point2d ext2 = new Point2d(u, v);

            plane.ClosestParameter(ptAnnotation, out u, out v);
            Point2d linePt = new Point2d(u, v);

            LinearDimension dimension = new LinearDimension(plane, ext1, ext2, linePt);
            dimension.SetBold(true);
            dimension.TextHeight = 100;
            dimension.DimensionStyle.ArrowType1 = DimensionStyle.ArrowType.Tick;
            dimension.DimensionStyle.ArrowType2 = DimensionStyle.ArrowType.Tick;
            
            return dimension;
        }

        public double GetArea()
        {
            var area = AreaMassProperties.Compute(Profile, 0.001).Area;
            return area;
        }

        public class BuildingShadow
        {
            public PolylineCurve boundary;
            public PolylineCurve Boundary 
            { 
                get => boundary;
                set
                {
                    boundary = value;
                    var t = RhinoDoc.ActiveDoc.HatchPatterns;
                    //int index = t.Find("Hatch1",true);
                    this.Hatch = Hatch.Create(new Curve[] { boundary }, 1, Math.PI / 4, 25, RhinoDoc.ActiveDoc.ModelAbsoluteTolerance)[0];
                }
            }
            public Hatch Hatch { get; private set; }
            public void AddShadow()
            {

            }
        }
    }
}
