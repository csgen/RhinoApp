﻿using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class PlaneObjectManager
    {
        public List<Building> Buildings { get; set; }
        public OuterPath OuterPath { get; set; }
        public List<MainPath> MainPath { get; set; }
        public List<P2P_Path> P2P_Path { get; set; }
        public List<Path> Paths { get; set; }
        public PlantingTrees PlantingTrees { get; set; }
        public double OuterPathWidth { get; set; }
        public double MainPathWidth { get; set; }
        public double P2P_PathWidth { get; set; }
        public double OuterPlantingWidth { get; set; }
        public double GMainFilletRadi { get; set; }//主路MainPath全局倒角半径

        public PlaneObjectManager()
        {
            Buildings = new List<Building>();
            MainPath = new List<MainPath>();
            P2P_Path = new List<P2P_Path>();
            Paths = new List<Path>();
            Paths.AddRange(MainPath);
            Paths.AddRange(P2P_Path);
            Paths.Add(OuterPath);
        }
    }
}