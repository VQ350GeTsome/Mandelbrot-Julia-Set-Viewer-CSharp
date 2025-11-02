using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace MandelBrot
{
    public class ComplexNumber
    {
        public double real = 0, imaginary = 0;
        private double? cachedR = null, cachedTheta = null;

        public ComplexNumber(double r, double i) { real = r; imaginary = i; }
        public ComplexNumber() { }

        public bool equals(ComplexNumber o)
        {
            return real == o.real && imaginary == o.imaginary;
        }

        #region operator overloaders

        //Additon operator, adds two complex number together
        public static ComplexNumber operator + (ComplexNumber a, ComplexNumber b) =>
            new(a.real + b.real, a.imaginary + b.imaginary);

        //Subtraction operator, subtracts one complex number from another
        public static ComplexNumber operator - (ComplexNumber a, ComplexNumber b) =>
            new(a.real - b.real, a.imaginary - b.imaginary);

        //Multiplication operator, multiplies two complex numbers together
        public static ComplexNumber operator * (ComplexNumber a, ComplexNumber b)
        {
            return new ((a.GetReal() * b.GetReal()) - (a.GetImaginary() * b.GetImaginary()), //The new real component is (r * or) - (i * oi) ... minus because i * i = -1
                (a.GetReal() * b.GetImaginary()) + (b.GetReal() * a.GetImaginary()));                     //The new imaginary component is (r * oi) + (i * or)
        }

        //Exponential operator, exponentiates a complex number by some double
        public static ComplexNumber operator ^ (ComplexNumber a, double n) 
        {
            double r = a.GetMagnitude();  //Cached
            double theta = a.GetAngle();  //Cached

            double newR = Math.Pow(r, n);
            double newTheta = theta * n;

            double real = newR * Math.Cos(newTheta);
            double imag = newR * Math.Sin(newTheta);

            return new (real, imag);
        }

        //Scale operator, scales a complex number by some double
        public static ComplexNumber operator | (ComplexNumber a, double n) => 
            new ComplexNumber(a.real * n, a.imaginary * n);
        

        #endregion

        #region regular operations

        //Returns the absolute value each component 
        public ComplexNumber Abs()
        {
            return new ComplexNumber(Math.Abs(real), Math.Abs(imaginary));
        }
        //Returns the conjugate of this complex number
        public ComplexNumber Conjugate()
        {
            return new ComplexNumber(real, -imaginary);
        }

        public ComplexNumber AddReal(double r)
        {
            return new ComplexNumber(real + r, imaginary);
        }
        public ComplexNumber SubtractReal(double r)
        {
            return new ComplexNumber(real - r, imaginary);
        }
        

        #endregion

        #region property getters

        //Returns the distance from (0 + 0i) (the orgin)
        public double GetDist() { return Math.Sqrt(real * real + imaginary * imaginary); }
        //Returns the square of the distance (this is so we do not have to use the expensive(ish) Math.sqrt function
        public double GetDistSqr() { return real * real + imaginary * imaginary; }
        //Get Magnitude, if it's null calculate it & save it, otherwise return it since we've calculated it previously
        public double GetMagnitude()
        {
            if (cachedR == null)
                cachedR = Math.Sqrt(real * real + imaginary * imaginary);
            return cachedR.Value;
        }
        //Get Angle, if it's null calculate it & save it, otherwise return it since we've calculated it previously
        public double GetAngle()
        {
            if (cachedTheta == null)
                cachedTheta = Math.Atan2(imaginary, real);
            return cachedTheta.Value;
        }

        #endregion

        #region getter & setters for the two componenets

        //Getters & Setters for the real & impaginary components
        public double GetReal() { return real; }
        public double GetImaginary() { return imaginary; }
        public void SetReal(int real) { this.real = real; }
        public void SetImaginary(int imaginary) { this.imaginary = imaginary; }

        #endregion

        #region String methods

        //Regular overrided ToString() returns for example: ( 1.24 , -0.12i )
        override public String ToString()
        {
            return "( " + Math.Round(real, 10) + " , " + Math.Round(imaginary, 10) + "i )";
        }
        //ToString no i verision, returns for example: (1.24, -0.12i)
        public String ToStringNoi()
        {
            return "(" + Math.Round(real, 10) + ", " + Math.Round(imaginary,10) + ")";
        }
        //Change this complex number from a String verion
        public void Parse(String s)
        {
            s = s.Substring(1, s.Length - 2); //Chop off { } || ( )
            String[] components = s.Split(new char[] { ',', '|' } );    //Split by | or ,
            components[0] = components[0].Trim();
            components[1] = components[1].Trim();       //Trim off whitespace and whatnot

            real = Double.Parse(components[0]);
            imaginary = Double.Parse(components[1]);
        }
        //String Constructor. Sets This to Parse(inputted string)
        public ComplexNumber(String s) { Parse(s); }

        #endregion
    }
}
