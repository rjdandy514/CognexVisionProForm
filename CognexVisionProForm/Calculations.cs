using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CognexVisionProForm
{
    internal class Calculations
    {
        public Double DegreeToRadians(double Deg)
        {
            double Rad = 0;
            Rad = Deg * (Math.PI / 180.0);
            return Rad;
        }

        public Double RadiansToDegree(double Rad)
        {
            double Deg = 0;
            Deg = Rad * (180 / Math.PI);
            return Deg;
        }

        public double DistanceBetweenPoints(double X1, double Y1, double X2, double Y2)
        {
            double dDistance;
            dDistance = Math.Sqrt(Math.Pow((Y2 - Y1),2) + Math.Pow((X2 - X1), 2));
            return dDistance;
        }
    }
}
