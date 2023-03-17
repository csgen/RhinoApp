
#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;
using Rhino.UI;
using Rhino.UI.Gumball;

namespace siteUI.Functions.gumball
{

    internal partial class MyGumballs : MouseCallback
    {
        private int index = -1;
        #region MouseCallBack

        protected override void OnMouseDown(MouseCallbackEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.MouseButton != MouseButton.Left) return;

            var pick = new Rhino.Input.Custom.PickContext();
            pick.PickStyle = Rhino.Input.Custom.PickStyle.PointPick;
            pick.View = e.View; //is Necessary？
            var xform = e.View.ActiveViewport.GetPickTransform(e.ViewportPoint);
            pick.SetPickTransform(xform);

            this.index = -1;

            for (int i = 0; i < this.Count; i++)
            {
                var result = this.conduits[i].PickGumball(pick, null);
                if (result)
                {
                    index = i;
                    e.Cancel = true;
                    break;
                }
            }

            if (this.index == -1) this.Dispose();

#if TEST
            RhinoApp.WriteLine(index.ToString());
#endif
        }

        protected override void OnMouseMove(MouseCallbackEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.index == -1) return;
            if (this.conduits[index].PickResult.Mode == Rhino.UI.Gumball.GumballMode.None) return;

            Line worldLine;
            // ViewPortPoint is the mouse position?
            e.View.ActiveViewport.GetFrustumLine(e.ViewportPoint.X, e.ViewportPoint.Y, out worldLine);
            Plane plane = e.View.ActiveViewport.GetConstructionPlane().Plane;
            double t;
            Rhino.Geometry.Intersect.Intersection.LinePlane(worldLine, plane, out t);
            Point3d dragPoint = worldLine.PointAt(t);

            //update the display of gumball
            this.conduits[index].UpdateGumball(dragPoint, worldLine);
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
            e.Cancel = true;

        }

        protected override void OnMouseUp(MouseCallbackEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.index == -1) return;

            UpdateLine();
            UpdateGumball();
            e.Cancel = true;

        }
        #endregion

        private void UpdateGumball()
        {
            //update the conduit with new conduits[index].Gumball

            this.conduits[index].SetBaseGumball(this.conduits[index].Gumball, this.gumballAppearanceSettings[index]);
            this.conduits[index].Enabled = true;

            //update the neighbor conduit
            GumballObject temp_gumball;
            if (index > 0)
            {
                temp_gumball = new Rhino.UI.Gumball.GumballObject();
                temp_gumball.SetFromPlane(this.planes[index - 1]);
                this.gumballs[index - 1] = temp_gumball;
                this.conduits[index - 1].SetBaseGumball(this.gumballs[index - 1], this.gumballAppearanceSettings[index - 1]);
                this.conduits[index - 1].Enabled = true;
            }

            if (index < this.Count - 1)
            {
                temp_gumball = new Rhino.UI.Gumball.GumballObject();
                temp_gumball.SetFromPlane(this.planes[index + 1]);
                this.gumballs[index + 1] = temp_gumball;
                this.conduits[index + 1].SetBaseGumball(this.gumballs[index + 1], this.gumballAppearanceSettings[index + 1]);
                this.conduits[index + 1].Enabled = true;
            }

            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        private void UpdateLine()
        {
            Point3d newPt;
            Transform trans = this.conduits[index].GumballTransform;

            newPt = this.curve.Point(index);
            newPt.Transform(trans);
            this.curve.SetPoint(index, newPt);

            newPt = this.curve.Point(index + 1);
            newPt.Transform(trans);
            this.curve.SetPoint(index + 1, newPt);

            this.obj.CommitChanges();

        }
    }
}
