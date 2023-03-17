using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Eto.Forms;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using Rhino.PlugIns;
using siteUI.Functions.gumball;

namespace siteUI.Functions
{
    internal class SelectLine
    {
        public static void SelectLines(RhinoDoc doc)
        {
            RhinoApp.WriteLine("The {0} command will add a building right now.", "DrawRect");

            MyGumballs myGumball;

            using (GetObject obj = new GetObject())
            {
                
                obj.GeometryFilter = ObjectType.Curve;
                obj.SubObjectSelect = true;
                obj.SetCommandPrompt("select buildings");
                obj.EnableClearObjectsOnEntry(false); 
                obj.EnablePreSelect(true, true);
                obj.EnableUnselectObjectsOnExit(true);

                //Select single Line at a time
                try
                {
                    obj.GetMultiple(1, 0);
                    var id = obj.Object(0).ObjectId;
                    obj.Object(0).Object().Select(true);

                    #region trigger gumball
                    myGumball = new MyGumballs(id);
                    myGumball.Show();
                    #endregion
                }
                catch   
                {
                    MessageBox.Show("error");
                }


            }

        }
    }
}
