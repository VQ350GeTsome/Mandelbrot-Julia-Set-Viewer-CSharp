using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    internal class ComplexNumber
    {
        private double real = 0, imaginary = 0;

        private double? cachedR = null, cachedTheta = null;

        public ComplexNumber(double real, double imaginary) { this.real = real; this.imaginary = imaginary; }
        public ComplexNumber() { }

        //Multiplies this complex number with another (other)
        private static ComplexNumber multiply(ComplexNumber p, ComplexNumber q)
        {
            return new ComplexNumber((p.getReal() * q.getReal()) - (p.getImaginary() * q.getImaginary()), //The new real component is (r * or) - (i * oi) ... minus because i * i = -1
                (p.getReal() * q.getImaginary()) + (q.getReal() * p.getImaginary()));                     //The new imaginary component is (r * oi) + (i * or)
        }
        //Returns a complex number that is this^n
        public static ComplexNumber pow(ComplexNumber o, double n) 
        {
            double r = o.getMagnitude();  //Cached
            double theta = o.getAngle();  //Cached

            double newR = Math.Pow(r, n);
            double newTheta = theta * n;

            double real = newR * Math.Cos(newTheta);
            double imag = newR * Math.Sin(newTheta);

            return new ComplexNumber(real, imag);
        }
        //Adds two complex numbers
        public ComplexNumber add(ComplexNumber other)
        {
            return new ComplexNumber(real + other.getReal(), imaginary + other.getImaginary());
        }
        //Returns the distance from (0 + 0i) (the orgin)
        public double getDist() { return Math.Sqrt(real * real + imaginary * imaginary); }
        //Returns the square of the distance so we dont have to use sqrt
        public double getDistSqr() { return real * real + imaginary * imaginary; }

        //Gets & Sets for the real & impaginary components
        public double getReal() { return real; }
        public double getImaginary() { return imaginary; }
        public void setReal(int real) { this.real = real; }
        public void setImaginary(int imaginary) { this.imaginary = imaginary; }

        //Get Magnitude, if it's null calculate it & save it, otherwise return it since we already calculated it
        public double getMagnitude()
        {
            if (cachedR == null)
                cachedR = Math.Sqrt(real * real + imaginary * imaginary);
            return cachedR.Value;
        }
        //Get Angle, if it's null calculate it & save it, otherwise return it since we already calculated it
        public double getAngle()
        {
            if (cachedTheta == null)
                cachedTheta = Math.Atan2(imaginary, real);
            return cachedTheta.Value;
        }
        override public String ToString()
        {
            return "{ " + Math.Round(real, 10) + " | " + Math.Round(imaginary, 10) + "i }";
        }
    }
}
