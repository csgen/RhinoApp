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
        public double avoidDistance;
        public Guid id;
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
                UpdateData();
            }
        }
        public ArchivableDictionary ClassData { get; private set; }
        public Building(Curve buildingCurve, double avoidDistance = 3)
        {
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
