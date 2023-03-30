using Rhino;
using Rhino.Collections;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoPlan.AutoPlan
{
    internal class OuterPath : Path
    {
        public RhinoDoc doc;
        public OuterPath(RhinoDoc doc,double width = 6)
        {
            FilletRadi = 8;
            Width = width;
            if (MyLib.MyLib.OuterPathWidth > 6)
            {
                Width = MyLib.MyLib.OuterPathWidth;
            }
            this.doc = doc;
        }
        private Guid id;
        public Guid ID 
        {
            get => id;
            set
            {
                id = value;
                MidCurve = new ObjRef(doc,id).Curve();
            }
        }
        private double area;
        public double Area
        {
            get
            {
                area = AreaMassProperties.Compute(MidCurve).Area;
                return area;
            }
            private set
            {
                area = value;
            }
        }
    }
    
}
