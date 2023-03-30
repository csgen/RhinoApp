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
    }
}
