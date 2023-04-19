using AutoPlan.AutoPlan.AutoCommands;
using PlanGenerator;
using Rhino;
using Rhino.ApplicationSettings;
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
    public class WPFStart : Command
    {
        public WPFStart()
        {
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static WPFStart Instance { get; private set; }
        public override string EnglishName => "StartAutoPlan";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            var dialog = new PlanGenerator.MainWindow();
            dialog.Show();
            RhinoDoc.SelectObjects += OnSelect;
            return Result.Success;
        }
        void OnSelect(object sender, RhinoObjectSelectionEventArgs e)
        {
            var objs = e.RhinoObjects; //获得选择到的objects

            RhinoObject obj0 = objs[0];//我们只选第1个
            int buildingN = 0;
            foreach(RhinoObject obj in objs)
            {
                var data = obj.UserDictionary; //可以直接从RhinoObject获得userDictionary或者data
                if (data.ContainsKey("AutoPlan"))
                {
                    if (data["AutoPlan"] as string == "OuterPath")
                    {
                        MainWindow.myArgs.LandArea = string.Format("{0:0.00}㎡", MyLib.MyLib.LandArea);
                    }
                    if (data["AutoPlan"] as string == "BuildingClass")
                    {
                        buildingN++;
                        Commands.GetBuildingShadow(RhinoDoc.ActiveDoc);
                        Commands.ShowBuildingArea(RhinoDoc.ActiveDoc);
                        Commands.ShowIllegalDimension(RhinoDoc.ActiveDoc);
                        //MainWindow.myArgs.Area = string.Format("{0:0.00}㎡", MyLib.MyLib.area);
                    }
                }
                if (MyLib.MyLib.LandArea != 0)
                    MainWindow.myArgs.AreaRatio= string.Format("{0:0.00}", MyLib.MyLib.area / MyLib.MyLib.LandArea);
            }
            //if (buildingN > 0)
            //{
            //    var dictionary = AutoPlanPlugin.Instance.Dictionary;
            //    if (dictionary.ContainsKey("P2P_PathData"))
            //    {
            //        Commands.UpdateP2P_Paths(RhinoDoc.ActiveDoc);
            //    }
            //}



            //if (obj.ObjectType == ObjectType.Curve)
            //{
            //    var crv = (Curve)obj.Geometry; //cast
            //    MainWindow.totalArea.Area = crv.GetLength().ToString();
            //}
        }
    }
    public class GetBuildingsCommand : Command
    {
        public GetBuildingsCommand()
        {
            Instance = this;
        }
        public static GetBuildingsCommand Instance { get; private set; }
        public override string EnglishName => "GetBuildingsCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GetBuildings(doc);
            Commands.ShowBuildingArea(doc);
            Commands.ShowIllegalDimension(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class GetOuterPathCommand : Command//使用中
    {
        public GetOuterPathCommand()
        {
            Instance = this;
        }
        public static GetOuterPathCommand Instance { get; private set; }
        public override string EnglishName => "GetOuterPathCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GetOuterPath(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class GetMainPathCommand : Command//使用中
    {
        public GetMainPathCommand()
        {
            Instance = this;
        }
        public static GetMainPathCommand Instance { get; private set; }
        public override string EnglishName => "GetMainPathCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GetMainPath(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class GetP2P_PathCommand : Command//使用中
    {
        public GetP2P_PathCommand()
        {
            Instance = this;
        }
        public static GetP2P_PathCommand Instance { get; private set; }
        public override string EnglishName => "GetP2P_PathCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GetP2P_Path(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class GeneratePathObjectCommand : Command//使用中
    {
        public GeneratePathObjectCommand()
        {
            Instance = this;
        }
        public static GeneratePathObjectCommand Instance { get; private set; }
        public override string EnglishName => "GeneratePathObjectCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GeneratePathObject(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class GenerateLandscapeCommand : Command
    {
        public GenerateLandscapeCommand()
        {
            Instance = this;
        }
        public static GenerateLandscapeCommand Instance { get; private set; }
        public override string EnglishName => "GenerateLandscapeCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            Commands.GenerateLandscape(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class AutoDrawCommand : Command//使用中
    {
        public const int HISTORY_VERSION = 20230324;
        public AutoDrawCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static AutoDrawCommand Instance { get; private set; }
        private PlaneObjectManager PlaneObjectM { get; set; }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "AutoDraw";
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
            HistorySettings.RecordingEnabled = true;
            PlaneObjectManager planeObjectM = new PlaneObjectManager();
            P2P_Path path;
            List<ObjRef> refBuildings;
            Point3d pt0, pt1;
            //Commands.DrawP2P_Path(doc, this, out planeObjectM, out path, out refBuildings, out pt0, out pt1);
            Commands.AutoDrawP2P_Path(doc, this, out planeObjectM, out path, out refBuildings, out pt0, out pt1);
            this.PlaneObjectM = planeObjectM;

            HistoryRecord history = new HistoryRecord(this, HISTORY_VERSION);
            P2P_Path.WriteHistory(history, refBuildings, new List<Point3d> { pt0, pt1 }, path);
            Guid id = doc.Objects.AddCurve(path.MidCurve, null, history, false);
            path.Doc = doc;
            path.ID = id;
            PlaneObjectM.P2P_Path.Add(path);
            PlaneObjectM.UpdateP2P_Path();
            PlaneObjectM.SetP2P_PathData(AutoPlanPlugin.Instance.Dictionary);

            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
        protected override bool ReplayHistory(ReplayHistoryData replayData)
        {
            return P2P_Path.ReplayHistory(HISTORY_VERSION, replayData, this, this.PlaneObjectM);
        }
    }
    [Obsolete]
    public class AutoPlanCommand : Command
    {
        public AutoPlanCommand()
        {
            Instance = this;
        }
        public static AutoPlanCommand Instance { get; private set; }
        public override string EnglishName => "AutoPlan";
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
    public class AddPathCommand : Command//使用中
    {
        public AddPathCommand()
        {
            Instance = this;
        }
        public static AddPathCommand Instance { get; private set; }
        public override string EnglishName => "AddPath";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            Commands.AddPath(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
    }
    public class ShowBuildingShadow : Command
    {
        public ShowBuildingShadow()
        {
            Instance = this;
        }
        public static ShowBuildingShadow Instance { get; private set; }
        public override string EnglishName => "ShowBuildingShadowCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            AutoPlanPlugin.Instance.Dictionary.Set("ShadowClass", "Empty");
            Commands.GetBuildingShadow(doc);
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
    public class EditPath : Command
    {
        public EditPath()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }

        ///<summary>The only instance of this command.</summary>
        public static EditPath Instance { get; private set; }

        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "EditPathCommand";

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
    [Obsolete]
    public class HistoryTestCommand : Command
    {
        public const int HISTORY_VERSION = 20230324;
        public HistoryTestCommand()
        {
            // Rhino only creates one instance of each command class defined in a
            // plug-in, so it is safe to store a refence in a static property.
            Instance = this;
        }
        ///<summary>The only instance of this command.</summary>
        public static HistoryTestCommand Instance { get; private set; }
        private PlaneObjectManager PlaneObjectM { get; set; }
        ///<returns>The command name as it appears on the Rhino command line.</returns>
        public override string EnglishName => "HistoryTestCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            RhinoApp.WriteLine("Start");

            PlaneObjectManager planeObjectM = new PlaneObjectManager();//创建新场景管理器

            OuterPath outerPath = new OuterPath(doc);
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectOuterPathCurve(planeObjectM, outerPath, getPath, "选外围道路");
            }

            List<MainPath> mainPaths = new List<MainPath>();
            using (GetObject getPath = new GetObject())
            {
                Selector.SelectMainPathCurve(planeObjectM, mainPaths, getPath, doc, "选主路");
            }

            List<Building> buildings = new List<Building>();
            List<ObjRef> refBuildings;
            using (GetObject getBuilding = new GetObject())
            {
                Selector s = new Selector(doc);
                
                s.SelectBuidling(buildings, getBuilding, out refBuildings, "选楼");
            }


            planeObjectM.Buildings = buildings;
            planeObjectM.OuterPath = outerPath;
            planeObjectM.MainPath = mainPaths;

            Point3d pt0;
            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第一个点");
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No start point was selected.");
                    return GPT.CommandResult();
                }
                pt0 = GPT.Point();
            }
            Point3d pt1;
            using (GetPoint GPT = new GetPoint())
            {
                GPT.SetCommandPrompt("第二个点");
                GPT.SetBasePoint(pt0, true);
                GPT.DynamicDraw +=
                    (sender, e) => e.Display.DrawCurve(new P2P_Path(new List<Point3d> { pt0, e.CurrentPoint }, planeObjectM).MidCurve, System.Drawing.Color.DarkRed);
                if (GPT.Get() != GetResult.Point)
                {
                    RhinoApp.WriteLine("No end point was selected.");
                    return GPT.CommandResult();
                }
                pt1 = GPT.Point();
            }
            P2P_Path p1 = new P2P_Path(new List<Point3d> { pt0, pt1 }, planeObjectM);
            planeObjectM.P2P_Path.Add(p1);
            PathObject pathObject = new PathObject(planeObjectM, doc);
            this.PlaneObjectM = planeObjectM;

            HistoryRecord history = new HistoryRecord(this, HISTORY_VERSION);
            P2P_Path.WriteHistory(history, refBuildings, new List<Point3d> { pt0, pt1 }, p1);
            Guid id = doc.Objects.AddCurve(p1.MidCurve, null, history, false);

            foreach (Brep brep in pathObject.PathBreps)
                doc.Objects.AddBrep(brep);
            return Result.Success;
        }
        protected override bool ReplayHistory(ReplayHistoryData replayData)
        {
            return P2P_Path.ReplayHistory(HISTORY_VERSION,replayData,this,this.PlaneObjectM);
        }
    }
    public class Test20230327 : Command
    {
        public Test20230327()
        {
            Instance = this;
        }
        public static Test20230327 Instance { get; private set; }
        public override string EnglishName => "TestDrawCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            List<Guid> gl = new List<Guid>();
            AutoPlanPlugin.Instance.Dictionary.Set("testId", gl);
            if (AutoPlanPlugin.Instance.Dictionary.ContainsKey("testId"))
            {
                List<Guid> idList = AutoPlanPlugin.Instance.Dictionary["testId"] as List<Guid>;
                if (idList.Count == 0)
                {
                    TestDraw(doc);
                }
            }
            //UpdateDraw(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
        public static void TestDraw(RhinoDoc doc)
        {
            Circle c1 = new Circle(Point3d.Origin, MyLib.MyLib.testRadius);
            //Circle c2 = new Circle(new Point3d(5, 5, 0), MyLib.MyLib.testRadius);
            List<Circle> cL = new List<Circle> { c1 };
            List<ObjRef> refL = new List<ObjRef>();
            List<Guid> guidL = new List<Guid>();
            foreach (Circle c in cL)
            {
                Guid id = doc.Objects.AddCircle(c);
                //refL.Add(new ObjRef(doc, id));
                guidL.Add(id);
            }
            //MyLib.MyLib.refList = refL;
            AutoPlanPlugin.Instance.Dictionary.Set("testId", guidL);
            //MyLib.MyLib.guidList = guidL;
            return;
            //MyArgs.MyArgs.ref
        }
        public static void UpdateDraw(RhinoDoc doc)
        {
            List<Guid> idList = AutoPlanPlugin.Instance.Dictionary["testId"] as List<Guid>;
            List<Guid> nullList = new List<Guid>();
            AutoPlanPlugin.Instance.Dictionary.Set("testId", nullList);
            foreach(Guid id in idList)
            {
                doc.Objects.Delete(id, true);
                TestDraw(doc);
            }
            
        }
    }
    public class Test20230328 : Command
    {
        public Test20230328()
        {
            Instance = this;
        }
        public static Test20230328 Instance { get; private set; }
        public override string EnglishName => "TestUpdateCommand";
        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            // TODO: start here modifying the behaviour of your command.
            // ---
            //TestDraw(doc);
            if (AutoPlanPlugin.Instance.Dictionary.ContainsKey("testId"))
                Test20230327.UpdateDraw(doc);
            //Test20230327.UpdateDraw(doc);
            doc.Views.Redraw();
            // ---
            return Result.Success;
        }
        
    }

}
