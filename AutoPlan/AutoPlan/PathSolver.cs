using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
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
            List<Fgh> path = pf.AStarSearch(mapRef, 1, mapX, mapY, startNodeID, endNodeID, isOrtho);
            List<Point3d> pathPt = new List<Point3d>();
            for (int i = 0; i < path.Count; i++)
            {
                Point3d point = gridCreation.Grids[path[i].x, path[i].y].Center;
                pathPt.Add(point);
            }
            pathPt.Reverse();
            return (PathCreate(pathPt));
        }
        public List<Point3d> PathCreate(List<Point3d> ptList)
        {
            ptList.Insert(0, startPoint);
            ptList.Add(endPoint);
            //Polyline path = new Polyline(ptList);
            return ptList;
        }
    }
}
