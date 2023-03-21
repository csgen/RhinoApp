using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteBuilding
{
    internal class Building
    {
        public Plane basePlane;
        public double Height { get; set; }
        public PolylineCurve Profile { get; set; }
        public PolylineCurve Shadow { get; set; }

        public Building(PolylineCurve profile)
        {
            this.Profile = profile;
        }
        public Building(Plane pl, double width, double length)
        {
            this.basePlane = pl;
            this.Profile = new Rectangle3d(pl, 
                new Interval(-0.5 * width, 0.5 * width), 
                new Interval(-0.5 * length, 0.5 * length))
                .ToPolyline()
                .ToPolylineCurve();
        }

        //public PolylineCurve GetShadow()
        //{

        //}

        public void CheckDistance(Building building)
        {
            
        }

    }
}
