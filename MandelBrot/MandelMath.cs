using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    internal class MandelMath
    {
        private static double epsilon = 0.000000001,
            power = 2.0; //What to exponentiate Z by (Z = Z^(power) + C)
                
        private static int width, height;

        //Sets width and height
        public static void setWidthHeight(int w, int h) { width = w; height = h; }
        //Sets iterations
        public static void setMaxIterations(int maxIterations) { maxIterations = maxIterations; }
        //Using c & z we calculate if it's bounded, -1 = bounded, else equals the amount of iterations before it was deemed unbounded
        public static int mandelCalc(ComplexNumber z, ComplexNumber c, int iterations)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = ComplexNumber.pow(z, power).add(c);   //Mutate z
                dist = z.getDistSqr();                    //Gets the distance from the orgin
                if (dist > 4) { return i; }               //If z is deemed unbounded (>2) return i
                if (dist < epsilon) { return -1; }        //If z is at the orgin return -1
            }
            return -1; 
        }
        //Returns an array of points that is the orbit
        public static Point[] mandelOrbit(ComplexNumber z, ComplexNumber c, int iterations, double zoom, double centerReal, double centerImag)
        {
            double dist = 0;
            Point[] points = new Point[iterations];
            for (int i = 0; i < iterations; i++)
            {
                z = ComplexNumber.pow(z, power).add(c);        //Mutate z
                dist = z.getDistSqr();                         //Gets the distance from the orgin
                points[i] = getXYfromC(z, zoom, centerReal, centerImag); 
                if (dist > 4) { return points; }               //If z is deemed unbounded (>2) return the points array
                if (dist < epsilon) { return points; }         //If z is at the orgin return the point array
            }
            return points;
        }
        public static ComplexNumber getC(int x, int y, double zoom, double centerReal, double centerImag)
        {
            double aspectRatio = (double)height / width;

            double viewWidth = 3.0 / zoom; // base width of view, scaled by zoom
            double viewHeight = viewWidth * aspectRatio;

            double realMin = centerReal - viewWidth / 2;
            double realMax = centerReal + viewWidth / 2;
            double imagMin = centerImag - viewHeight / 2;
            double imagMax = centerImag + viewHeight / 2;

            double real = realMin + (x / (double)width) * (realMax - realMin);
            double imag = imagMax - (y / (double)height) * (imagMax - imagMin); // flip Y-axis

            return new ComplexNumber(real, imag);
        }
        public static Point getXYfromC(ComplexNumber c, double zoom, double centerReal, double centerImag)
        {
            double aspectRatio = (double)height / width;

            double viewWidth = 3.0 / zoom;
            double viewHeight = viewWidth * aspectRatio;

            double realMin = centerReal - viewWidth / 2;
            double realMax = centerReal + viewWidth / 2;
            double imagMin = centerImag - viewHeight / 2;
            double imagMax = centerImag + viewHeight / 2;

            double x = ((c.getReal() - realMin) / (realMax - realMin)) * width;
            double y = ((imagMax - c.getImaginary()) / (imagMax - imagMin)) * height;

            return new Point((int)x, (int)y);
        }
        public static void changePower(double delta) { power += delta; power = (power < 1) ? 1 : power; }
    }
}
