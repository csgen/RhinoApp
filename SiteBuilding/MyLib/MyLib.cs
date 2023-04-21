using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Forms;
using Rhino.Collections;
//using Eto.Forms;

namespace MyLib
{
    public class MyLib
    {
        public static RhinoDoc doc { get; set; }
        public static double area { get; set; }
        private static double landArea;
        public static double LandArea//用地面积
        {
            get
            {
                var c = new ObjRef(doc, LandID).Curve();
                if (c!=null)
                {
                    landArea = AreaMassProperties.Compute(c).Area;
                    return landArea;
                }
                return landArea;
                
            }
            set
            {
                landArea = value;
            }
        }
        public static Guid LandID { get; set; }
        public static double testRadius { get; set; }
        //public static List<ObjRef>? refList { get; set; }
        //public static List<Guid>? guidList { get; set; }
        private static double outerPathWidth { get; set; }
        public static double OuterPathWidth 
        {
            get => outerPathWidth;
            set
            {
                outerPathWidth = 6;
                if(value>outerPathWidth) outerPathWidth = value;
            }
        }
        private static double mainPathWidth { get; set; }
        public static double MainPathWidth 
        {
            get => mainPathWidth;
            set
            {
                mainPathWidth = 4;
                if (value > mainPathWidth)
                {
                    mainPathWidth = value;
                }
            }
        }
        private static double p2p_pathWidth;
        public static double P2P_PathWidth 
        { 
            get => p2p_pathWidth;
            set
            {
                p2p_pathWidth = 4;
                if(value> p2p_pathWidth)
                {
                    p2p_pathWidth = value;
                }
            }
        }
        public static double GreenArea { get; set; }//绿化面积
        public static double ConcentrationGreenArea { get; set; }
        public static double GreenAreaRatio { get; set; }
        public static double TreeScale { get; set; }//控制树木大小
        public static double TreeDensity { get; set; }//控制树木疏密
        public static void WIP_Message()
        {
            MessageBox.Show("努力开发中");
        }
    }
}