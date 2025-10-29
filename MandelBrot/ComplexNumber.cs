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
        private double real = 0, imaginary = 0;
        private double? cachedR = null, cachedTheta = null;

        public ComplexNumber(double r, double i) { real = r; imaginary = i; }
        public ComplexNumber() { }

        #region static operations

        //Multiplies this complex number with another (other)
        private static ComplexNumber Multiply(ComplexNumber p, ComplexNumber q)
        {
            return new ComplexNumber((p.GetReal() * q.GetReal()) - (p.GetImaginary() * q.GetImaginary()), //The new real component is (r * or) - (i * oi) ... minus because i * i = -1
                (p.GetReal() * q.GetImaginary()) + (q.GetReal() * p.GetImaginary()));                     //The new imaginary component is (r * oi) + (i * or)
        }
        //Returns a complex number that is it^n
        public static ComplexNumber Pow(ComplexNumber o, double n) 
        {
            double r = o.GetMagnitude();  //Cached
            double theta = o.GetAngle();  //Cached

            double newR = Math.Pow(r, n);
            double newTheta = theta * n;

            double real = newR * Math.Cos(newTheta);
            double imag = newR * Math.Sin(newTheta);

            return new ComplexNumber(real, imag);
        }

        #endregion

        #region regular operations

        //Adds two complex numbers
        public ComplexNumber Add(ComplexNumber other)
        {
            return new ComplexNumber(real + other.GetReal(), imaginary + other.GetImaginary());
        }
        //Subtracts two complex numbers
        public ComplexNumber Subtract(ComplexNumber o)
        {
            return new ComplexNumber(real - o.GetReal(), imaginary - o.GetImaginary());
        }
        //Raises the complex number to the nth power
        public ComplexNumber Pow(double n)
        {
            double r = GetMagnitude();
            double theta = GetAngle();

            double newR = Math.Pow(r, n);
            double newTheta = theta * n;

            double real = newR * Math.Cos(newTheta);
            double imag = newR * Math.Sin(newTheta);

            return new ComplexNumber(real, imag);
        }
        //Scales the complex number by the input
        public ComplexNumber Scale(double s)
        {
            return new ComplexNumber(real * s, imaginary * s);
        }
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
        //Multiplies this number with another
        public ComplexNumber Multiply(ComplexNumber o)
        {
            double real = (this.real * o.GetReal()) - (this.imaginary * o.imaginary);
            double imag = (this.real * o.GetImaginary()) + (this.imaginary * o.GetReal());
            return new ComplexNumber(real, imag);
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

        #region ToString(), Parse(String) methods

        //Regular ToString : { r , i }
        override public String ToString()
        {
            return "{ " + Math.Round(real, 10) + " | " + Math.Round(imaginary, 10) + "i }";
        }
        //ToString Parenthesis verison : (r, i)
        public String ToStringParen()
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
