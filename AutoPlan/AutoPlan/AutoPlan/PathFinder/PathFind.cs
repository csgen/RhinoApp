using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPlan.AutoPlan.PathFinder
{
    internal class PathFind
    {
        private void FindFghByList(List<Fgh> list, int x, int y, out Fgh fgh, out int index)
        {
            fgh = null;
            index = 0;
            for (int i = 0; i < list.Count; i++)
            {
                Fgh v = list[i];
                if (v.x == x && v.y == y)
                {
                    fgh = v;
                    index = i;
                }
            }
        }
        public List<Fgh> AStarSearch(byte[,] mapRef, int roadValue, int mapMaxW, int mapMaxH, int[] startPoint, int[] endPoint, bool isFourDir)
        {
            int endPointX = endPoint[0];
            int endPointY = endPoint[1];

            if (mapRef[endPointX, endPointY] != roadValue)//避障，roadValue为可通过的值
            {
                return new List<Fgh>();
            }

            int[,] dir = null;
            if (isFourDir)
            {
                dir = new int[,] { { 1, 0 }, { 0, 1 }, { 0, -1 }, { -1, 0 } };
            }
            else
            {
                dir = new int[,] { { 1, 1 }, { 1, 0 }, { 1, -1 }, { 0, 1 }, { 0, -1 }, { -1, 1 }, { -1, 0 }, { -1, -1 } };
            }
            List<Fgh> tOpenList = new List<Fgh>();//搜索列表
            List<Fgh> tCloseList = new List<Fgh>();//忽略列表

            tOpenList.Add(new Fgh(startPoint[0], startPoint[1]));//起点

            while (tOpenList.Count > 0)
            {
                int index = 0;
                Fgh curBestFgh = tOpenList[index];
                for (int i = 0; i < tOpenList.Count; i++)
                {
                    if (curBestFgh.f > tOpenList[i].f)//最优f
                    {
                        curBestFgh = tOpenList[i];
                        index = i;
                    }
                }
                tOpenList.RemoveAt(index);
                if (curBestFgh.x == endPointX && curBestFgh.y == endPointY)//找到终点
                {
                    List<Fgh> path = new List<Fgh>();
                    while (curBestFgh != null)
                    {
                        path.Add(new Fgh(curBestFgh.x, curBestFgh.y));
                        curBestFgh = curBestFgh.father;
                    }
                    return path;
                }
                int len = isFourDir ? 4 : 8;
                for (int i = 0; i < len; i++)
                {
                    int x = curBestFgh.x + dir[i, 0];
                    int y = curBestFgh.y + dir[i, 1];
                    if (0 <= x && x < mapMaxW && 0 <= y && y < mapMaxH)
                    {
                        if (mapRef[x, y] == roadValue || x == endPointX && y == endPointY)
                        {
                            //fgh
                            int father_g = curBestFgh.father != null ? curBestFgh.father.g : 0;
                            int g = father_g + (x != curBestFgh.x && y != curBestFgh.y ? 14 : 10);
                            int h = 0;
                            if (isFourDir)
                            {
                                h = (Math.Abs(x - endPointX) + Math.Abs(y - endPointY)) * 10;
                            }
                            else
                            {
                                int dx = Math.Abs(x - endPointX);
                                int dy = Math.Abs(y - endPointY);
                                int minD = Math.Min(dx, dy);
                                int maxD = Math.Max(dx, dy);
                                h = minD * 14 + (maxD - minD) * 10;
                            }
                            int f = g + h;//f=到起点步数g+到终点步数h
                            Fgh cFgh = new Fgh(x, y, g, h, f, curBestFgh);

                            //Add or Remove fgh
                            Fgh openFgh;
                            Fgh closeFgh;
                            int openIndex;
                            int closeIndex;
                            FindFghByList(tOpenList, x, y, out openFgh, out openIndex);
                            FindFghByList(tCloseList, x, y, out closeFgh, out closeIndex);
                            if (openFgh == null && closeFgh == null)
                            {
                                tOpenList.Add(cFgh);
                            }
                            else if (openFgh != null)
                            {
                                if (openFgh.f > cFgh.f) tOpenList[openIndex] = cFgh;
                            }
                            else if (closeFgh.f > cFgh.f)
                            {
                                tOpenList.Add(cFgh);
                                tCloseList.RemoveAt(closeIndex);
                            }
                        }
                    }

                }
                tCloseList.Add(curBestFgh);
            }
            return new List<Fgh>();
        }
    }
}
