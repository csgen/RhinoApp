﻿using AutoPlan.AutoPlan.AutoCommands;
using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using siteUI.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace AutoPlan.AutoPlan
{
    public class AutoPlanCommand : Command
    {
        public AutoPlanCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static AutoPlanCommand Instance { get; private set; }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "AutoPlan";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            //foreach(Curve curve in localcurve)
            //{
            //    foreach (Brep pipe in Brep.CreatePipe(curve, 5, true, PipeCapMode.None, true, 0.01, 0.01))
            //    {
            //        doc.Objects.AddBrep(pipe);
            //    }
            //}
            Commands.DrawP2P_Path(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class AutoDrawCommand : Command
    {
        public AutoDrawCommand()
        {
            Instance = this;
        }
        public static AutoDrawCommand Instance { get; private set; }
        public override string EnglishName => "AutoDraw";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GlobalGenerate(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class AddDataCommand : Command
    {
        public AddDataCommand()
        {
            Instance = this;
        }
        public static AddDataCommand Instance { get; private set; }
        public override string EnglishName => "AddData";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            //int a = 2, b = 3;
            using(GetObject getObject = new GetObject())
            {
                getObject.SetCommandPrompt("select");
                getObject.GeometryFilter = ObjectType.Curve;
                getObject.Get();
                Dictionary<string,int> myDict = new Dictionary<string,int>();
                TestData td = new TestData();
                td.a=2; td.b=3;
                ArchivableDictionary ad = new ArchivableDictionary();
                
                //getObject.Object(0).Curve().UserDictionary.Append(new KeyValuePair<string, object>("a",1));
                getObject.Object(0).Curve().UserData.Add(td);
            }
            return Result.Success;
        }
    }
    public class GetDataCommand : Command
    {
        public GetDataCommand()
        {
            Instance = this;
        }
        public static GetDataCommand Instance { get; private set; }
        public override string EnglishName => "GetData";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            UserDataList ud;
            using (GetObject getObject = new GetObject())
            {
                getObject.SetCommandPrompt("select");
                getObject.GeometryFilter = ObjectType.Curve;
                getObject.Get();
                ud = getObject.Object(0).Curve().UserData;
                
            }
            
            //foreach(string key in ud.Keys)
            RhinoApp.WriteLine(ud.ToString());
            
            return Result.Success;
        }
    }
    public class TestData : UserData
    {
        public int a { get; set; }
        public int b { get; set; }
        public TestData() { }
        //public override string ToString()
        //{
        //    return string.Format("a={0},b={1}", a, b);
        //}
    }
    public class siteCommand : Command
    {
        public siteCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static siteCommand Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "DragLine";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---

            SelectLine.SelectLines(doc);

            doc.Views.Redraw();
            return Result.Success;

        }
    }
    
}