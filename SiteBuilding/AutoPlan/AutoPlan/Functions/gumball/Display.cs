using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using Rhino;
using Rhino.Geometry;
using Rhino.UI;

namespace siteUI.Functions.gumball
{

    internal partial class MyGumballs : MouseCallback
    {

        #region Dispaly
        public void Show()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.conduits[i].Enabled = true;
            }
            this.Enabled = true;
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        public void Hide()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.conduits[i].Enabled = false;
            }
            this.Enabled = false;
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        public void Dispose()
        {
            for (int i = 0; i < this.Count; i++)
            {
                this.conduits[i].Enabled = false;
                this.conduits[i].Dispose();
                this.gumballs[i].Dispose();
            }
            this.Enabled = false;
            Rhino.RhinoDoc.ActiveDoc.Views.Redraw();
        }

        #endregion

    }
}
