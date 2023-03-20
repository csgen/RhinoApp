using AutoPlan;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan.PathFinder
{
    internal class PathSolver
    {
        Point3d startPoint { set; get; }
        Point3d endPoint { set; get; }
        List<Curve> obstacles { get; set; }
        int resolution { get; set; }
        bool isOrtho { get; set; }
        public PathSolver(Point3d startPoint, Point3d endPoint, List<Curve> obstacles, int resolution, bool isOrtho)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.obstacles = obstacles;
            this.resolution = resolution;
            this.isOrtho = isOrtho;
        }
        public List<Point3d> PathRhinoSolver()
        {
            GridCreation gridCreation = new GridCreation(startPoint, endPoint, resolution, obstacles);
            List<Fgh> pathGrid = FindInRhino(gridCreation);
            //while (pathGrid.Count == 0)//用于保证能找到路径，但是会大量减慢速度，待研究
            //{
            //    resolution += 5;
            //    gridCreation = new GridCreation(startPoint, endPoint, resolution, obstacles);
            //    pathGrid = FindInRhino(gridCreation);
            //}
            List<Point3d> pathPt = new List<Point3d>();
            for (int i = 0; i < pathGrid.Count; i++)
            {
                Point3d point = gridCreation.Grids[pathGrid[i].x, pathGrid[i].y].Center;
                pathPt.Add(point);
            }
            pathPt.Reverse();
            //return (PathCreate(pathPt));

            return pathPt;
        }
        public List<Point3d> PathCreate(List<Point3d> ptList)
        {
            ptList.Insert(0, startPoint);
            ptList.Add(endPoint);
            //Polyline path = new Polyline(ptList);
            return ptList;
        }
        private List<Fgh> FindInRhino(GridCreation gridCreation)
        {
            //GridCreation gridCreation = new GridCreation(startPoint, endPoint, rhinoResolution, obstacles);
            PathFind pf = new PathFind();
            int mapX = gridCreation.Grids.GetLength(0);
            int mapY = gridCreation.Grids.GetLength(1);
            byte[,] mapRef = new byte[mapX, mapY];
            for (int x = 0; x < mapX; x++)
            {
                for (int y = 0; y < mapY; y++)
                {
                    mapRef[x, y] = (byte)gridCreation.EnvValue[x, y];
                }
            }
            int[] startNodeID = gridCreation.StartNodeId;
            int[] endNodeID = gridCreation.EndNodeId;
            List<Fgh> pathGrid = pf.AStarSearch(mapRef, 1, mapX, mapY, startNodeID, endNodeID, isOrtho);
            return pathGrid;
        }
    }
}
