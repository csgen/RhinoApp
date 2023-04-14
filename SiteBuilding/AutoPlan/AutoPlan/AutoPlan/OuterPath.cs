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
        public OuterPath(RhinoDoc doc,double width = 6)
        {
            this.Doc = doc;
            this.DataSet = new ArchivableDictionary();
            FilletRadi = 8;
            Width = width;
            if (MyLib.MyLib.OuterPathWidth > 6)
            {
                Width = MyLib.MyLib.OuterPathWidth;
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
        public static OuterPath BuiltFromDict(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            Guid id = dictionary.GetGuid("ID");
            double filletRadi = dictionary.GetDouble("FilletRadi");
            double width = dictionary.GetDouble("Width");
            OuterPath p = new OuterPath(doc, width);
            p.ID = id;
            p.Width = MyLib.MyLib.OuterPathWidth;
            p.FilletRadi = filletRadi;
            return p;
        }
    }
    
}
