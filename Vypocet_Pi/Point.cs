using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vypocet_Pi
{
    internal class Point
    {
        private double X { get; set; }
        private double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        public double GetX()
        {
            return X;
        }
        public double GetY()
        {
            return Y;
        }
    }
}
