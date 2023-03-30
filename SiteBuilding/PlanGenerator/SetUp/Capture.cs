using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace PlanGenerator.Toolkit
{
    internal static class Capture
    {
        public static Bitmap CaptureScreen_seg()
        {

            System.Drawing.Size size = new System.Drawing.Size(400, 200);
            var id = Rhino.Display.DisplayModeDescription.AddDisplayMode("myMode");
            var dm = Rhino.Display.DisplayModeDescription.FindByName("myMode");//Rendered
            dm.DisplayAttributes.CastShadows = false; //取消阴影
            dm.DisplayAttributes.ShadingEnabled = true;

            dm.DisplayAttributes.ViewSpecificAttributes.UseDocumentGrid = false;
            dm.DisplayAttributes.ViewSpecificAttributes.DrawGrid = false;
            dm.DisplayAttributes.ViewSpecificAttributes.DrawGridAxes = false;
            dm.DisplayAttributes.ViewSpecificAttributes.DrawWorldAxes = false;

            dm.DisplayAttributes.ShowSurfaceEdges = false;
            dm.DisplayAttributes.ShowIsoCurves = false;
            dm.DisplayAttributes.ShowCurves = false;

            dm.DisplayAttributes.SetFill(System.Drawing.Color.FromArgb(4, 250, 7)); //背景填充绿色

            Dictionary<string, System.Drawing.Color> color = new Dictionary<string, System.Drawing.Color>(){
            {"building",System.Drawing.Color.FromArgb(255, 180, 120, 120)},
            {"road",System.Drawing.Color.FromArgb(140, 140, 140)},
            {"tree",System.Drawing.Color.FromArgb(4, 200, 3)},
            {"floor",System.Drawing.Color.FromArgb(80, 50, 50)}
            };

            Dictionary<System.Guid, Tuple<System.Drawing.Color, Rhino.DocObjects.ObjectColorSource>> temp = new Dictionary<System.Guid, Tuple<System.Drawing.Color, Rhino.DocObjects.ObjectColorSource>>();
            for (int i = 0; i < color.Keys.Count; i++)
            {
                var layer_name = color.Keys.ElementAt(i);
                System.Drawing.Color layer_color = color[layer_name];
                var obj = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(layer_name);
                if (obj != null)
                {
                    for (int j = 0; j < obj.Length; j++)
                    {
                        var tuple = new Tuple<System.Drawing.Color, Rhino.DocObjects.ObjectColorSource>(obj[j].Attributes.ObjectColor, obj[j].Attributes.ColorSource);
                        temp[obj[j].Id] = tuple;
                        obj[j].Attributes.ObjectColor = layer_color;
                        obj[j].Attributes.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                        obj[j].CommitChanges();
                    }
                }
            }

            var v = Rhino.RhinoDoc.ActiveDoc.Views.ActiveView;
            //dm.SupportsShading = false;

            var bitmap = v.CaptureToBitmap(size, dm);
            //var image = ConvertBitmap(bitmap);
            //bitmap.Dispose();

            for (int i = 0; i < color.Keys.Count; i++)
            {
                var layer_name = color.Keys.ElementAt(i);
                var layer_color = color[layer_name];
                var obj = Rhino.RhinoDoc.ActiveDoc.Objects.FindByLayer(layer_name);
                if (obj != null)
                {
                    for (int j = 0; j < obj.Length; j++)
                    {
                        var tuple = temp[obj[j].Id];
                        obj[j].Attributes.ObjectColor = tuple.Item1;
                        obj[j].Attributes.ColorSource = tuple.Item2;
                        obj[j].CommitChanges();
                    }
                }
            }
            //Rhino.Display.DisplayModeDescription.DeleteDisplayMode(id);

            return bitmap;

        }
    }
}
