using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal partial class Building
    {
        public Curve BuildingCurve { get; set; }
        public double avoidDistance;
        public TextEntity Text_Annotation { get; set; }
        public Guid id;
        public RhinoDoc doc { get; set; }
        public double AvoidDistance 
        {
            get { return avoidDistance; }
            set
            {
                avoidDistance = value;
                UpdateData();
            }
        }
        public Guid ID
        {
            get { return id; }
            set
            {
                id = value;
                BuildingCurve = new ObjRef(doc, id).Curve();
                Profile = (PolylineCurve)BuildingCurve;
                UpdateData();
            }
        }
        public ArchivableDictionary ClassData { get; private set; }
        public Building(Curve buildingCurve, RhinoDoc doc, double avoidDistance = 3)
        {
            this.doc = doc;
            Profile = buildingCurve.ToPolyline(0.001,100,0.001,1000000);
            Initialization();
            TextAnnotation();
            BuildingCurve = buildingCurve;
            AvoidDistance = avoidDistance;
            UpdateData();
        }
        public void UpdateData()
        {
            ClassData = new ArchivableDictionary();
            ClassData.Set("AvoidDistance", avoidDistance);
            ClassData.Set("GUID", id);
        }
        public void TextAnnotation()
        {
            Text_Annotation = new TextEntity
            {
                Plane = TextPlane(),
                RichText = "30F",
                TextHeight = 3,
                Justification = TextJustification.Center
            };
            Text_Annotation.SetBold(true);
        }
        private Plane TextPlane()
        {
            Point3d[] points = new Point3d[Corners.Count - 1];
            Corners.CopyTo(0,points,0,Corners.Count-1);
            List<Point3d> pts = points.ToList();
            var s = pts.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            int index = pts.FindIndex(a => a == s[0]);
            Point3d p0 = pts[index];
            Point3d p1 = Point3d.Unset;
            Point3d p2 = Point3d.Unset;
            if (index == 0)
            {
                p1 = pts[1];
                p2 = pts.Last();
            }
            else if(index == pts.Count - 1)
            {
                p1 = pts[0];
                p2 = pts[index - 1];
            }
            else
            {
                p1 = pts[index + 1];
                p2 = pts[index - 1];
            }
            
            Vector3d v1 = p1 - p0;
            Vector3d v2 = p2 - p0;
            v1.Unitize();
            v2.Unitize();
            Plane plane = Plane.WorldXY;
            plane.Origin = p0 + v1 * 5 + v2 * 5;
            plane.OriginZ = 1;
            return plane;
        }
    }
}
