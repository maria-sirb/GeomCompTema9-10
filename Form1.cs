using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GeomCompTema9
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //Partitionarea unui poligon simplu in poligoane monotone(care nu au varfuri reflexe):
        //Parcurgem varfurile poligonului sortate in sens invers trigonometric
        //Trasam de la ficare varf reflex cate o diagonala
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen greenPen = new Pen(Color.LimeGreen, 1);
            StreamReader sr = new StreamReader(@"..\..\TextFile1.txt");
            Polygon pol = new Polygon(g, sr);
            List<Point> pointsList = new List<Point>(pol.PointsList);
            List<List<Point>> polygons = new List<List<Point>>(); //lista de poligoane monotone
            polygons.Add(pointsList);
            for(int i = 0; i < pointsList.Count; i++)
            {
                Point curr = pointsList[i];
                Point beforeCurr;
                Point afterCurr;
                if(i == 0)
                {
                    beforeCurr = pointsList[pointsList.Count - 1];
                    afterCurr = pointsList[1];
                }
                else if(i == pointsList.Count - 1)
                {
                    beforeCurr = pointsList[i - 1];
                    afterCurr = pointsList[0];
                }
                else
                {
                    beforeCurr = pointsList[i - 1];
                    afterCurr = pointsList[i + 1];

                }
                if(Orientation(beforeCurr, curr, afterCurr) == 1) // daca varful curent este reflex
                {
                    Point closest = new Point();
                    //daca varful curent este mai jos decat varfurile adiacente cautam cel mai apropiat
                    //punct care se afla deasupra lui si cu care formeaza o diagonala
                    if(beforeCurr.Y > curr.Y && afterCurr.Y > curr.Y)
                    {
                        closest = new Point(-1, -1); 
                        for(int j = 0; j < pointsList.Count; j++)
                        {
                            if(IsDiagonal(pointsList, curr, pointsList[j]) && pointsList[j].Y < curr.Y && pointsList[j].Y > closest.Y )
                            {
                                closest = pointsList[j];
                            }
                        }
                        g.DrawLine(greenPen, closest, curr);
                        
                    }
                    else if(beforeCurr.Y < curr.Y && afterCurr.Y < curr.Y)
                    {
                        //daca varful curent este sub cele doua varfuri adiacente cautam cel mai apropiat
                        //punct care se afla sub el si cu care formeaza o diagonala
                        closest = new Point(800, 800);
                        for(int j = 0; j < pointsList.Count; j++)
                        {
                            if (IsDiagonal(pointsList, curr, pointsList[j]) && pointsList[j].Y > curr.Y && pointsList[j].Y < closest.Y)
                            {
                                closest = pointsList[j];
                            }
                        }
                        g.DrawLine(greenPen, closest, curr);
                    }
                    for(int j = 0; j < polygons.Count; j++)
                    {
                        //cautam poligonul monoton in care s-a trasat ultima diagonala
                        //si il impartim in doua
                        if(polygons[j].Contains(closest) && polygons[j].Contains(curr)) 
                        {
                            int closestIdx = polygons[j].IndexOf(closest);
                            int currIdx = polygons[j].IndexOf(curr);
                            //cream un nou poligon pe care il vom adauga in lista poligoanelor monotone
                            List<Point> newPolygon = new List<Point>();
                            for(int k = currIdx; k <= closestIdx; k++)
                            {
                                newPolygon.Add(polygons[j][k]);
                            }
                            if(closestIdx < currIdx)
                            {
                                closestIdx = polygons[j].Count - closestIdx;
                            }
                            //din fostul poligon mai mare stergem varfurile pe care le am mutat in
                            //noul poligon dupa impartirea in doua
                            polygons[j].RemoveRange(currIdx + 1, closestIdx - currIdx - 1);
                            polygons.Add(newPolygon);
                            break;
                        }
                    }
                }
               

            }
            //triangulam fiecare poligon monoton din lista daca are mai mult de 3 varfuri
            for (int i = 0; i < polygons.Count; i++)
            {
                if (polygons[i].Count > 3)
                
                {
                   /* for(int j = 0; j < polygons[i].Count - 1; j++)
                    {
                        g.DrawLine(Pens.Pink, polygons[i][j], polygons[i][j + 1]);
                    }*/
                    Triangulare(polygons[i], g);
                }
            }


        }
        /// <summary>
        /// Trianguleaza un poligon monoton
        /// </summary>
        /// <param name="pol"></param>
        /// <param name="g"></param>
        private static void Triangulare(List<Point> pol, Graphics g)
        {
            //cele doua lanturi poligonale
            List<Point> l1 = new List<Point>();
            List<Point> l2 = new List<Point>();
            int ymin = 800, ymax = -1;
            Point up = new Point();
            Point down = new Point();
            int upIdx = -1, downIdx = -1;
            for(int i = 0; i < pol.Count; i++)
            {
                if(pol[i].Y < ymin)
                {
                    ymin = pol[i].Y;
                    up = pol[i];
                    upIdx = i;
                }
                if(pol[i].Y > ymax)
                {
                    ymax = pol[i].Y;
                    down = pol[i];
                    downIdx = i;
                }
            }
            //int start = upIdx;
           int j = upIdx;
            l2.Add(pol[downIdx]);
            while (true)
            {
                
                if (j >= upIdx && j <= downIdx)
                {
                    l1.Add(pol[j]);

                }
                
                else 
                {
                    l2.Add(pol[j]);
                }
                j = (j + 1) % pol.Count;
                if (j == upIdx)
                    break;

           }
            l2.Add(pol[upIdx]);
            Sort(pol);
           /* for (int i = 0; i < l1.Count - 1; i++)
                     {
                         g.DrawLine(Pens.Aqua, l1[i], l1[i + 1]);
                     }
            for (int i = 0; i < l2.Count - 1; i++)
            {
                g.DrawLine(Pens.Pink, l2[i], l2[i + 1]);
            }*/
            
           // g.DrawLine(Pens.Red, pol[0], pol[pol.Count - 1]);
            Stack<Point> st = new Stack<Point>();
            st.Push(pol[0]);
            st.Push(pol[1]);
          
            for (int i = 2; i < pol.Count - 1; i++)
            {
              
               //daca varful i si ultimul varf din stiva sunt pe lanturi diferite
               //trasam diagonale intre varful i si toate varfurile din stiva mai putin ultimul
               //scoatem toate varfurile din stiva
               //adaugam inapoi varfurile i si i - 1
                if(st.Count > 0 && (l1.Contains(pol[i]) && !l1.Contains(st.Peek()) )||
                    (l2.Contains(pol[i]) && !l2.Contains(st.Peek())))
                {
                    while(st.Count > 1)
                    {
                        g.DrawLine(Pens.Orange, pol[i], st.Peek());
                        st.Pop();
                    }
                    st.Pop();
                    st.Push(pol[i - 1]);
                    st.Push(pol[i]);

                }
                //daca varful i si ultimul din sitva sunt pe acelasi lant poligonal
                //scoatem din stiva toate varfurile cu care acesta formeaza diagonale
                //adaugam inapoi ultimul varf scos din stiva si varful i
                else
                {
                    Point lastDeleted = new Point();
                    if(st.Count > 0)
                       lastDeleted = st.Pop();
                    Stack<Point> notDiagonals = new Stack<Point>();
                    while (st.Count > 0)
                    {
                        
                        if(IsDiagonal(pol, pol[i], st.Peek()))
                        {
                            g.DrawLine(Pens.Orange, pol[i], st.Peek());
                            lastDeleted = st.Pop();
                        }
                        else
                        {
                            notDiagonals.Push(st.Pop());
                        }
                    }
                    while(notDiagonals.Count > 0)
                    {
                        st.Push(notDiagonals.Pop());
                    }
                    st.Push(lastDeleted);
                    st.Push(pol[i]);
                }
            }
           
           //trasam diagonale de la ultimul varf al poligonului la toate varfurile ramase in stiva
           //in afara de primul si ultimul
            if (st.Count > 0)
                st.Pop();
            while(st.Count > 1)
            {
                g.DrawLine(Pens.Orange, pol[pol.Count - 1], st.Pop());

            }
            st.Pop();


        }
        /// <summary>
        /// Sorteaza punctele poligonului in functie de ordonata
        /// </summary>
        /// <param name="pol"></param>
        private static void Sort(List<Point> pol)
        {
            bool ok;
            do
            {
                ok = true;
                for(int i = 0; i < pol.Count - 1; i++)
                {
                    if(pol[i].Y > pol[i + 1].Y)
                    {
                        Point aux = pol[i];
                        pol[i] = pol[i + 1];
                        pol[i + 1] = aux;
                        ok = false;
                    }
                }
            } while (ok == false);
           
        }

        /// <summary>
        /// Checks if three points turn left or right
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="C"></param>
        /// <returns>0 for collinear, 1 for rigth turn, 2 for left turn</returns>
        private static int Orientation(Point A, Point B, Point C)
        {
            int value = (B.Y - A.Y) * (C.X - B.X) - (C.Y - B.Y) * (B.X - A.X);
            if (value == 0)
            {
                return 0;  //collinear points
            }
            else
            {
                return value > 0 ? 1 : 2;  //clockwise(right) / counterclockwise(left) orientation
            }
        }
        /// <summary>
        /// Checks if two line segments intersect
        /// </summary>
        /// <param name="P1"></param>
        /// <param name="Q1"></param>
        /// <param name="P2"></param>
        /// <param name="Q2"></param>
        /// <returns>true / false</returns>
        private static bool DoIntersect(Point P1, Point Q1, Point P2, Point Q2)
        {
            int o1 = Orientation(P1, Q1, P2);
            int o2 = Orientation(P1, Q1, Q2);
            int o3 = Orientation(P2, Q2, P1);
            int o4 = Orientation(P2, Q2, Q1);

            if (o1 != o2 && o3 != o4)
                return true;
            else return false;
        }
        /// <summary>
        /// Checks if a line segment P1P2 intersects the sides of a polygon(list)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="P1"></param>
        /// <param name="P2"></param>
        /// <returns>true / false</returns>
        private static bool IntersectsSides(List<Point> l, Point P1, Point P2)
        {
            for (int i = 0; i < l.Count - 1; i++)
            {
                if (l[i] != P1 && l[i] != P2 && l[i + 1] != P1 && l[i + 1] != P2)
                {
                    if (DoIntersect(P1, P2, l[i], l[i + 1]))
                        return true;

                }
            }
            return false;
        }
        /// <summary>
        /// checks if a line segment is inside a polygon(list)
        /// </summary>
        /// <param name="l"></param>
        /// <param name="s"></param>
        /// <returns>true / false</returns>
        private static bool IsInside(List<Point> l, Point A, Point B)
        {
            //if segment doesn't intersect sides of the polygon  =>it's enough to check
            //if the middle is inside 
            Point middle = new Point();
            middle.X = (B.X + A.X) / 2;
            middle.Y = (B.Y + A.Y) / 2;
            Point extreme = new Point(800, middle.Y);
            int count = 0, i = 0;
            do
            {
                int next = (i + 1) % l.Count;
                if (DoIntersect(l[i], l[next], middle, extreme))
                {
                    count++;
                }
                i = next;
            } while (i != 0);
            return count % 2 == 1;

        }
        /// <summary>
        /// Checks if a line segment is a diagonal in polygon 
        /// </summary>
        /// <param name="l"></param>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        private static bool IsDiagonal(List<Point> l, Point A, Point B)
        {
            return !IntersectsSides(l, A, B) && IsInside(l, A, B);
        }
    }
}
