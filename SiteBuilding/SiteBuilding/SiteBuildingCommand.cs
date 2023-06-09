﻿using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using PlanGenerator;
using MaterialDesignThemes.Wpf;
using Rhino.DocObjects;
using MyArgs;

namespace SiteBuilding.Sitebuilding
{
    public class WPFStart : Command
    {
        public WPFStart()
        {
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static WPFStart Instance { get; private set; }
        public override string EnglishName => "SitePlan";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var dialog = new PlanGenerator.MainWindow();
            dialog.Show();

            return Result.Success;
        }
    }

    public class BuildingShadow : Command
    {
        public BuildingShadow()
        {
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static BuildingShadow Instance { get; private set; }
        public override string EnglishName => "AddShadow";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("The {0} command will add a building right now.", "DrawRect");
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
                    //var id = obj.Object(0).ObjectId;
                    //obj.Object(0).Object().Select(true);
                    //var totalArea = 0.0;
                    for (int i = 0; i < obj.ObjectCount; i++)
                    {
                        PolylineCurve curve = obj.Object(i).Curve().ToPolyline(0.01, 10, 0.01, 10000);
                        var building = new Building(curve);
                        Rhino.RhinoDoc.ActiveDoc.Objects.Add(building.Shadow);
                        //totalArea += building.GetArea();
                    }     
                    //MyLib.MyLib.area = totalArea;

                }
                catch
                {
                    RhinoApp.WriteLine("error");
                }

                return Result.Success;
            }
        }



        public class SiteBuildingCommand : Command
        {
            public SiteBuildingCommand()
            {
                // Rhino only creates one instance of each command class defined in a
                // plug-in, so it is safe to store a refence in a static property.
                Instance = this;
            }

            ///<summary>The only instance of this command.</summary>
            public static SiteBuildingCommand Instance { get; private set; }

            private const string INTEGER_VALUE = "INT";
            private const string DOUBLE_VALUE = "DBL";

            private int m_integer_value = 1;
            private double m_double_value = 2.13;

            ///<returns>The command name as it appears on the Rhino command line.</returns>
            public override string EnglishName => "SiteBuildingCommand";

            protected override Result RunCommand(RhinoDoc doc, RunMode mode)
            {
                GetValues(SiteBuilding.Instance.Dictionary);
                SetValues(SiteBuilding.Instance.Dictionary);
                return Result.Success;
            }
            private void GetValues(ArchivableDictionary dictionary)
            {
                if (null == dictionary)
                    return;

                RhinoApp.WriteLine(dictionary.Name);

                int integer_value;
                if (dictionary.TryGetInteger(INTEGER_VALUE, out integer_value))
                {
                    m_integer_value = integer_value;
                    RhinoApp.WriteLine("  Integer = {0}", m_integer_value);
                }
                else
                    RhinoApp.WriteLine("  Integer = <none>");

                double double_value;
                if (dictionary.TryGetDouble(DOUBLE_VALUE, out double_value))
                {
                    m_double_value = double_value;
                    RhinoApp.WriteLine("  Double = {0}", m_double_value);
                }
                else
                    RhinoApp.WriteLine("  Double = <none>");
            }

            private void SetValues(ArchivableDictionary dictionary)
            {
                if (null == dictionary)
                    return;

                m_integer_value++;
                m_double_value *= 1.57;

                dictionary.Set(INTEGER_VALUE, m_integer_value);
                dictionary.Set(DOUBLE_VALUE, m_double_value);
            }
        }
    }
}
