﻿using AutoPlan.AutoPlan.AutoCommands;
using Rhino;
using Rhino.Collections;
using Rhino.Commands;
using Rhino.Display;
using Rhino.DocObjects;
using Rhino.DocObjects.Custom;
using Rhino.FileIO;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using siteUI.Functions;
using System;
using System.Collections.Generic;
using System.IO;
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
                ad.Set("a", 2);
                ad.Set("b", 3);
                getObject.Object(0).Curve().UserDictionary.AddContentsFrom(ad);
                //getObject.Object(0).Curve().UserDictionary.Append(new KeyValuePair<string, object>("a",1));
                //getObject.Object(0).Curve().UserData.Add(td);
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
            //UserDataList ud;
            ArchivableDictionary ad;
            using (GetObject getObject = new GetObject())
            {
                getObject.SetCommandPrompt("select");
                getObject.GeometryFilter = ObjectType.Curve;
                getObject.Get();
                //ud = getObject.Object(0).Curve().UserData;
                ad = getObject.Object(0).Curve().UserDictionary;
            }
            //var i = ud.Find(typeof(TestData)) as TestData;
            //foreach(string key in ud.Keys)

            RhinoApp.WriteLine(ad["a"].ToString());
            
            return Result.Success;
        }
    }
    public class TestData : UserData
    {
        public int a { get; set; }
        //public string name { get; set; }
        public int b { get; set; }
        //BinaryArchiveReader archive;
        
        public TestData() { }
        public override bool ShouldWrite
        {
            get
            {
                return true;
            }
        }
        protected override bool Read(BinaryArchiveReader archive)
        {
            ArchivableDictionary dict = archive.ReadDictionary();
            if(dict.ContainsKey("a")&&dict.ContainsKey("b"))
            {
                a = (int)dict["a"];
                b = (int)dict["b"];
            }
            return true;
        }
        protected override bool Write(BinaryArchiveWriter archive)
        {
            var dict = new ArchivableDictionary(1, "Test");
            dict.Set("a", a);
            dict.Set("b", b);
            archive.WriteDictionary(dict);

            return true;
        }
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
    public class DisplayBrep : Command
    {
        private const int HISTORY_VERSION = 20230322;
        public DisplayBrep()
        {
            Instance = this;
        }
        public static DisplayBrep Instance { get; private set; }
        public override string EnglishName => "DisplayTest";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            ObjRef objRef;
            Curve[] crv = new Curve[1];
            using(GetObject getObject = new GetObject())
            {
                getObject.EnablePreSelect(true, false);
                getObject.SetCommandPrompt("select curve");
                getObject.GeometryFilter = ObjectType.Curve;
                getObject.Get();
                objRef = getObject.Object(0);
                crv[0] = getObject.Object(0).Curve();
            }
            
            HistoryRecord history = new HistoryRecord(this, HISTORY_VERSION);
            WriteHistory(history, objRef);
            Brep brep = Brep.CreatePlanarBreps(crv, 0.01)[0];
            doc.Objects.AddBrep(brep, null, history, false);
            //System.Guid id = doc.Objects.AddBrep(brep[0]);
            return Result.Success; 
        }
        protected override bool ReplayHistory(ReplayHistoryData replay)
        {
            ObjRef objRef = null;
            if(!ReadHistory(replay,ref objRef))
                return false;
            Curve curve = objRef.Curve();
            if(curve == null) 
                return false;
            Brep brep = Brep.CreatePlanarBreps(curve, 0.01)[0];
            replay.Results[0].UpdateToBrep(brep, null);
            return true;
        }
        private bool ReadHistory(ReplayHistoryData replay,ref ObjRef objRef)
        {
            if (HISTORY_VERSION != replay.HistoryVersion)
                return false;
            objRef = replay.GetRhinoObjRef(0);
            return true;
        }
        private bool WriteHistory(HistoryRecord history,ObjRef objRef)
        {
            if (!history.SetObjRef(0, objRef))
                return false;
            return true;
        }
    }
    public class SampleCsHistory : Command
    {
        /// <summary>
        /// The only instance of the SampleCsHistory command.
        /// </summary>
        static SampleCsHistory g_command_instance;

        /// <summary>
        /// History recording version.
        /// This field is used to ensure the version of the replay function matches
        /// the version that originally created the geometry.
        /// </summary>
        private const int HISTORY_VERSION = 20131107;

        /// <summary>
        /// Private data member
        /// </summary>
        private int m_segment_count = 2;

        /// <summary>
        /// Public constructor
        /// </summary>
        public SampleCsHistory()
        {
            g_command_instance = this;
        }

        /// <summary>
        /// Returns the only instance of the SampleCsHistory command.
        /// </summary>
        public static SampleCsHistory Instance => g_command_instance;

        /// <summary>
        /// Returns the English name of this command.
        /// </summary>
        public override string EnglishName => "SampleCsHistory";

        /// <summary>
        /// Rhino calls this function to run the command.
        /// </summary>
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            const Rhino.DocObjects.ObjectType filter = Rhino.DocObjects.ObjectType.Curve;
            Rhino.DocObjects.ObjRef objref;
            Rhino.Commands.Result rc = Rhino.Input.RhinoGet.GetOneObject("Select curve to divide", false, filter, out objref);
            if (rc != Rhino.Commands.Result.Success || objref == null)
                return rc;

            Rhino.Geometry.Curve curve = objref.Curve();
            if (null == curve || curve.IsShort(Rhino.RhinoMath.ZeroTolerance))
                return Rhino.Commands.Result.Failure;

            rc = Rhino.Input.RhinoGet.GetInteger("Number of segments", false, ref m_segment_count, 2, 100);
            if (rc != Rhino.Commands.Result.Success)
                return rc;

            Rhino.Geometry.Point3d[] points;
            curve.DivideByCount(m_segment_count, true, out points);
            if (null == points)
                return Rhino.Commands.Result.Failure;

            // Create a history record
            Rhino.DocObjects.HistoryRecord history = new Rhino.DocObjects.HistoryRecord(this, HISTORY_VERSION);
            WriteHistory(history, objref, m_segment_count, points.Length);

            for (int i = 0; i < points.Length; i++)
                doc.Objects.AddPoint(points[i], null, history, false);

            doc.Views.Redraw();

            return Rhino.Commands.Result.Success;
        }

        /// <summary>
        /// Rhino calls the virtual ReplayHistory functions to to remake an objects when inputs have changed.  
        /// </summary>
        protected override bool ReplayHistory(Rhino.DocObjects.ReplayHistoryData replay)
        {
            Rhino.DocObjects.ObjRef objref = null;
            int segmentCount = 0;
            int pointCount = 0;

            if (!ReadHistory(replay, ref objref, ref segmentCount, ref pointCount))
                return false;

            Rhino.Geometry.Curve curve = objref.Curve();
            if (null == curve)
                return false;
            
            if (pointCount != replay.Results.Length)
                return false;

            Rhino.Geometry.Point3d[] points;
            curve.DivideByCount(segmentCount, true, out points);
            if (null == points)
                return false;

            for (int i = 0; i < points.Length; i++)
                replay.Results[i].UpdateToPoint(points[i], null);

            return true;
        }

        /// <summary>
        /// Read a history record and extract the references for the antecedent objects.
        /// </summary>
        private bool ReadHistory(Rhino.DocObjects.ReplayHistoryData replay, ref Rhino.DocObjects.ObjRef objref, ref int segmentCount, ref int pointCount)
        {
            if (HISTORY_VERSION != replay.HistoryVersion)
                return false;

            objref = replay.GetRhinoObjRef(0);
            if (null == objref)
                return false;

            if (!replay.TryGetInt(1, out segmentCount))
                return false;

            if (!replay.TryGetInt(2, out pointCount))
                return false;

            return true;
        }

        /// <summary>
        /// Write a history record referencing the antecedent objects
        /// The history should contain all the information required to reconstruct the input.
        /// This may include parameters, options, or settings.
        /// </summary>
        private bool WriteHistory(Rhino.DocObjects.HistoryRecord history, Rhino.DocObjects.ObjRef objref, int segmentCount, int pointCount)
        {
            if (!history.SetObjRef(0, objref))
                return false;

            if (!history.SetInt(1, segmentCount))
                return false;

            if (!history.SetInt(2, pointCount))
                return false;

            return true;
        }
    }
}
