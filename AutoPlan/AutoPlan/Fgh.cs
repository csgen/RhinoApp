using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class Fgh//平面格子结构
    {
        public int x = 0;
        public int y = 0;
        public int g = 0;
        public int h = 0;
        public int f = 0;
        public Fgh father = null;
        public Fgh(int x0, int y0)
        {
            x = x0;
            y = y0;
        }
        public Fgh(int x0, int y0, int g0, int h0, int f0, Fgh father0)
        {
            x = x0; y = y0; g = g0; h = h0; f = f0; father = father0;
        }
    }
}
