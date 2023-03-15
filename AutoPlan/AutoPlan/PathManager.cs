using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan
{
    internal class PathManager
    {
        public OuterPath OuterPath { get; set; }
        public MainPath MainPath { get; set; }
        public P2P_Path P2P_Path { get; set; }
    }
}
