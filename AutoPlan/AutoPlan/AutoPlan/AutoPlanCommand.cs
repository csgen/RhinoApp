using AutoPlan.AutoPlan.AutoCommands;
using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using siteUI.Functions;
using System;
using System.Collections.Generic;
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
        public override string EnglishName => "AutoPlanCommand";
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
