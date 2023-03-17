using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.UI;

namespace siteUI.Functions.gumball
{

    internal partial class MyGumballs : MouseCallback
    {
        public RhinoObject obj;
        public PolylineCurve curve => ((PolylineCurve)obj.Geometry);
        public List<Plane> planes => GetPlanes();
        private int Count => this.curve.PointCount - 1;

        public Rhino.UI.Gumball.GumballDisplayConduit[] conduits;
        private Rhino.UI.Gumball.GumballObject[] gumballs;
        private Rhino.UI.Gumball.GumballAppearanceSettings[] gumballAppearanceSettings;
        //private List<int> gumballParameters = new List<int>() { 1, 1, 2, 1, 1, 50, 5, 2, 15, 35 };

        public MyGumballs(Guid id)
        {
            this.obj = Rhino.RhinoDoc.ActiveDoc.Objects.FindId(id);

            SetGumball();
            SetAppearance();
            SetConduit();
        }

        private List<Plane> GetPlanes()
        {

            var planes = new List<Plane>();

            for (int i = 0; i < this.curve.PointCount - 1; i++)
            {
                //get mid point of each segment line
                var midPt = (this.curve.Point(i) + this.curve.Point(i + 1)) * 0.5;
                var xAxis = this.curve.Point(i + 1) - this.curve.Point(i);
                xAxis.Unitize();
                var yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
                yAxis.Unitize();
                planes.Add(new Plane(midPt, xAxis, yAxis));
            }

            return planes;
        }

        #region gumball
        public void SetGumball()
        {
            var gumballs = new Rhino.UI.Gumball.GumballObject[Count];
            //initialize gumball object here
            for (int i = 0; i < Count; i++)
            {
                gumballs[i] = new Rhino.UI.Gumball.GumballObject();
                gumballs[i].SetFromPlane(this.planes[i]);
            }
            this.gumballs = gumballs;
        }
        #endregion

        #region appearance
        public void SetAppearance()
        {
            var appearances = new Rhino.UI.Gumball.GumballAppearanceSettings[Count];

            var appearance = new Rhino.UI.Gumball.GumballAppearanceSettings();

            //set customized gumballAppearance setting here
            appearance.RotateXEnabled = false;
            appearance.RotateYEnabled = false;
            appearance.RotateZEnabled = false;

            appearance.ScaleXEnabled = false;
            appearance.ScaleYEnabled = false;
            appearance.ScaleZEnabled = false;

            //appearance.TranslateZEnabled = false;
            //appearance.TranslateXEnabled = false;
            appearance.TranslateYZEnabled = false;
            appearance.TranslateZXEnabled = false;

            //

            for (int i = 0; i < Count; i++)
            {
                appearances[i] = appearance;
            }
            this.gumballAppearanceSettings = appearances;
        }
        #endregion

        #region conduit
        public void SetConduit()
        {
            var conduits = new Rhino.UI.Gumball.GumballDisplayConduit[Count];
            //initialize conduit object here
            for (int i = 0; i < Count; i++)
            {
                conduits[i] = new Rhino.UI.Gumball.GumballDisplayConduit();
                conduits[i].SetBaseGumball(gumballs[i], gumballAppearanceSettings[i]);
            }
            this.conduits = conduits;
        }
        #endregion

        private void GetPlanesFromCurve()
        {
            double t0 = this.curve.Domain.T0;
            double t1 = this.curve.Domain.T1;
            List<double> paras = new List<double>() { t0 };
            //var pt = curve.PointAt((t0 + t1) * 0.5);

            while (true)
            {
                double t;
                bool result = this.curve.GetNextDiscontinuity(Continuity.C1_continuous, t0, t1, out t);
                if (!result)
                {
                    break;
                }
                else
                {
                    t0 = t;
                }
                paras.Add(t);
            }
            paras.Add(t1);

            var parameters = new List<double>();
            for (int i = 0; i < paras.Count - 1; i++)
            {
                //get mid point of each segment line
                parameters.Add((paras[i] + paras[i + 1]) * 0.5);
            }

            //this.parameters = paras;
            //this.planes = parameters.Select(x => this.CreatePlane(this.curve, x)).ToList();
            //this.Count = this.planes.Count;
        }
        public Plane CreatePlane(Curve crv, double t)
        {
            var point = crv.PointAt(t);
            var xAxis = crv.TangentAt(t);
            xAxis.Unitize();
            var yAxis = Vector3d.CrossProduct(Vector3d.ZAxis, xAxis);
            yAxis.Unitize();

            return new Plane(point, xAxis, yAxis);
        }

    }
}
