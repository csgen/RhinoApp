using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class Building
    {
        public Curve BuildingCurve { get; set; }
        public double AvoidDistance { get; set; }
        public Guid ID { get; set; }
        public ArchivableDictionary ClassData { get; set; }
        public Building(Curve buildingCurve, double avoidDistance = 3)
        {
            BuildingCurve = buildingCurve;
            AvoidDistance = avoidDistance;
            SetData();
        }
        public void SetData()
        {
            ClassData = new ArchivableDictionary();
            ClassData.Set("AvoidDistance", AvoidDistance);
            ClassData.Set("GUID", ID);
        }
    }
}
