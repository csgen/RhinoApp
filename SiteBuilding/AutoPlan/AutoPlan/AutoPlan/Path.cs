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
    internal class Path
    {
        public RhinoDoc Doc { private get; set; }
        public ArchivableDictionary DataSet { get; set; }
        private Curve midCurve;
        public Curve MidCurve 
        {
            get => midCurve;
            set
            {
                midCurve = value;
                DataSet.Set("MidCurve", midCurve);
            }
        }
        private double width;
        public double Width 
        {
            get => width;
            set
            {
                width = value;
                DataSet.Set("Width", width);
            }
        }
        private double filletRadi;
        public double FilletRadi 
        {
            get => filletRadi;
            set
            {
                filletRadi = value;
                DataSet.Set("FilletRadi", filletRadi);
            }
        }
        private Guid id;
        public Guid ID 
        {
            get => id;
            set
            {
                id = value;
                DataSet.Set("ID", id);
                var c = Doc.Objects.FindId(id);
                if (c!=null)
                {
                    MidCurve = new ObjRef(Doc, id).Curve();
                }
            }
        }

    }
}
