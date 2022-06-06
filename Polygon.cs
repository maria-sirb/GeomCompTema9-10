using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeomCompTema9
{
    class Polygon
    {
        public List<Point> PointsList { get; private set; }
        public Polygon(Graphics g, StreamReader sr)
        {
            PointsList = new List<Point>();
            // Graphics g = e.Graphics;
            Pen redPen = new Pen(Color.Red, 3);
            Pen bluePen = new Pen(Color.Blue, 1);

            using (sr)
            {
                int cornersNr = int.Parse(sr.ReadLine());
                for (int i = 1; i <= cornersNr; i++)
                {
                    string[] coord = sr.ReadLine().Split(' ');
                    Point P = new Point();
                    P.X = int.Parse(coord[0]);
                    P.Y = int.Parse(coord[1]);
                    PointsList.Add(P);
                    g.DrawEllipse(redPen, P.X, P.Y, 2, 2);

                }

                for (int i = 0; i < PointsList.Count - 1; i++)
                {
                    g.DrawLine(bluePen, PointsList[i], PointsList[i + 1]);

                }
                g.DrawLine(bluePen, PointsList[PointsList.Count - 1], PointsList[0]);
            }
        }
       
    }
}
