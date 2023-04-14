using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class MainPath : Path
    {
        public MainPath(RhinoDoc doc, double width = 4)
        {
            this.Doc = doc;
            this.DataSet = new ArchivableDictionary();
            FilletRadi = 4;
            Width = 4;
            if (MyLib.MyLib.MainPathWidth > width)
            {
                Width = MyLib.MyLib.MainPathWidth;
            }
        }
        public static MainPath BuiltFromDict(ArchivableDictionary dictionary, RhinoDoc doc)
        {
            double width = dictionary.GetDouble("Width");
            double filletRadi = dictionary.GetDouble("FilletRadi");
            Guid id = dictionary.GetGuid("ID");
            MainPath p = new MainPath(doc, width);
            p.ID = id;
            p.FilletRadi = filletRadi;
            p.Width = MyLib.MyLib.MainPathWidth;
            return p;
        }
    }
}
