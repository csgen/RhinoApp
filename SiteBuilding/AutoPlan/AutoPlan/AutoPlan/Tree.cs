using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan
{
    internal class Tree
    {
        public double Radius { get; set; }
        //public Curve TreeCurve { get; set; }
        public Mesh TreeMesh { get; private set; }
        public Point3d TreeLocation { get; set; }

        public Tree(double radius,Point3d treeLocation)
        {
            this.Radius = radius;
            this.TreeLocation = treeLocation;
            CreateTree();
        }
        private void CreateTree()//生成圆形，并转化为多(n)边Mesh
        {
            Circle treeCurve = new Circle(TreeLocation, Radius);
            Point3d[] pL;
            int n = 10;
            
            NurbsCurve crv = treeCurve.ToNurbsCurve();
            crv.DivideByCount(n, true, out pL);

            Mesh mesh = new Rhino.Geometry.Mesh();
            mesh.Vertices.Add(treeCurve.Center);
            foreach (Point3d p in pL)
            {
                mesh.Vertices.Add(p);
            }
            for (int i = 1; i < pL.Length; i++)
            {
                mesh.Faces.AddFace(0, i, i + 1);
            }
            mesh.Faces.AddFace(0, pL.Length, 1);
            TreeMesh = mesh;
        }
    }
}
