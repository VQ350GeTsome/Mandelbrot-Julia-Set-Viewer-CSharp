using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    internal class MandelMath
    {
        private static double epsilon = 0.000001;
        private static int iterations = 200; //1000 is good enough for doubles
                
        private static int width, height;

        //Sets width and height
        public static void SetWidthHeight(int w, int h) { width = w; height = h; }

        #region fractals

        //Using c, z & n we calculate if it's bounded, -1 = bounded, else equals the amount of iterations before it was deemed unbounded
        public static double MandelCalc(ComplexNumber z, ComplexNumber c, double n)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = (z^n) + c;                            
                dist = z.GetDistSqr();                          //Gets the distance from the orgin (squared)
                if (dist > 4) { return GetValue(i, z); }        //If z is deemed unbounded (>4) return i
                if (dist < epsilon) { return -1; }              //If z is at the orgin return -1
            }
            return -1; 
        }
        //Using c, z & n we do the same thing as MandelCalc but we change the formula a bit so it's the burning ship set
        public static double BurningShipCalc(ComplexNumber z, ComplexNumber c, double n)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = (z.Abs()^n) + c;                            //The burning ship is z = (|real(z)| + |imag(z)|)^n + c
                dist = z.GetDistSqr();                          //Gets the distance from the orgin (squared)
                if (dist > 4) { return GetValue(i, z); }        //If z is deemed unbounded (>4) return i
                if (dist < epsilon) { return -1; }              //If z is at ts
            }
            return -1;
        }
        //Just like the other methods, we return the iteration count or -1 if it's in the set for the tricorn set
        public static double TricornCalc(ComplexNumber z, ComplexNumber c, double n)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = (z.Conjugate() ^ n) + c;                //The Tricorn fractal is z = conjugate(z)^n + c
                dist = z.GetDistSqr();                          //Gets the distance from the orgin (squared)
                if (dist > 4) { return GetValue(i, z); }        //If z is deemed unbounded (>4) return i
                if (dist < epsilon) { return -1; }              //If z is at ts
            }
            return -1;
        }

        public static double CelticCalc(ComplexNumber z, ComplexNumber c, double n)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = z^n;                                   
                z = new ComplexNumber(Math.Abs(z.GetReal()), z.GetImaginary());
                z = z + c;                                      //The Celtic fractal is z = (real(|z^2|) + imag(z^2)) + c
                dist = z.GetDistSqr();                          //Gets the distance from the orgin (squared)
                if (dist > 4) { return GetValue(i, z); }        //If z is deemed unbounded (>4) return i
                if (dist < epsilon) { return -1; }              //If z is at ts
            }
            return -1;
        }

        public static double LambdaCalc(ComplexNumber z, ComplexNumber l, double n)
        {
            double dist = 0;
            for (int i = 0; i < iterations; i++)
            {
                z = l * (z * (z.SubtractReal(-1)));             //Lambda fractal is z = l * z(1 - z)
                dist = z.GetDistSqr();                          //Gets the distance from the orgin (squared)
                if (dist > 4) { return GetValue(i, z); }        //If z is deemed unbounded (>4) return i
                if (dist < epsilon) { return -1; }              //If z is at ts
            }
            return -1;
        }
        public static double PhoenixCalc(ComplexNumber z, ComplexNumber c, double n, ComplexNumber p)
        {
            double dist = 0;
            ComplexNumber z_n1 = new(0, 0); // z_{n-1}
            for (int i = 0; i < iterations; i++)
            {
                ComplexNumber z_1 = (z^n) + c + (p * z_n1);
                dist = z_1.GetDistSqr();
                if (dist > 4) { return GetValue(i, z_1); }
                if (dist < epsilon) { return -1; }

                z_n1 = z;
                z = z_1;
            }
            return -1;
        }

        #endregion

        //Returns an array of points that is the orbit
        public static Point[] MandelOrbit(ComplexNumber z, ComplexNumber c, double n, double zoom, double centerReal, double centerImag)
        {
            return null;
        }

        private static double GetValue(int i, ComplexNumber z)
        {
            return ColorMnH.GetNewValue(i, z);
        }

        #region Complex Number <-> (x, y) translators

        public static ComplexNumber GetC(int x, int y, double zoom, double centerReal, double centerImag)
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
        public static Point GetXYfromC(ComplexNumber c, double zoom, double centerReal, double centerImag)
        {
            double aspectRatio = (double)height / width;

            double viewWidth = 3.0 / zoom;
            double viewHeight = viewWidth * aspectRatio;

            double realMin = centerReal - viewWidth / 2;
            double realMax = centerReal + viewWidth / 2;
            double imagMin = centerImag - viewHeight / 2;
            double imagMax = centerImag + viewHeight / 2;

            double x = ((c.GetReal() - realMin) / (realMax - realMin)) * width;
            double y = ((imagMax - c.GetImaginary()) / (imagMax - imagMin)) * height;

            return new Point((int)x, (int)y);
        }

        #endregion

        #region Render getters

        public static int GetIterations() { return iterations; }
        public static void SetIterations(int i) { iterations = i; }
        public static double GetEps() { return epsilon; }
        public static void SetEps(double eps) { epsilon = eps; }

        #endregion

    }
}
