using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    public class FractalManager
    {
        private int width, height;

        private double zoom = 1.00,
                          n = 2.00;
        private int[,] screen;

        private ComplexNumber center = new ComplexNumber(-0.50, 0.00),    //Center of the screen
                              juliaC = new ComplexNumber();

        public FractalManager(int w, int h) 
        {
            width = w; height = h;
            screen = new int[width, height];    //Initalize screen
        }

        //Returns the currect screen
        public int[,] GetScreen() { return screen; }

        //Returns the current zoom amount
        public double GetZoom() { return zoom; }
        //Sets the zoom to the argument
        public void SetZoom(double z) { zoom = z; }
        //Changes zoom by the argument value
        public void ChangeZoom(double delta) { zoom *= delta; }

        public double GetN() { return n; }
        public void SetN(double n) { this.n = n; }
        public void ChangeN(double delta) { n += delta; }

        public ComplexNumber GetCenter() { return center; }
        public void SetCenter(ComplexNumber c) { center = c; }

        public ComplexNumber GetJuliaC() { return juliaC; }
        public void SetJuliaC(ComplexNumber c) { juliaC = c; }

        #region Updaters

        //Fills the screen with escape times for the current Mandelbrot
        public void UpdateMandelBrot()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.GetC(x, y, zoom, center.GetReal(), center.GetImaginary()), z = new(); //Calculate the currentPoint given the (x, y) coordinates
                    screen[x, y] = MandelMath.MandelCalc(z, currentPoint, n);                   //Calculate q to determine if currentPoint is in the set (-1) or not in the set        
                }
            });
        }
        //Fills the screen with escape times for the current Julia
        public void UpdateJuliaSet()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.GetC(x, y, zoom, center.GetReal(), center.GetImaginary());
                    screen[x, y] = MandelMath.MandelCalc(currentPoint, juliaC, n);
                }
            });
        }
        //Fills the screen with escape times for the current Burning Ship
        public void UpdateBurningShip()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.GetC(x, y, zoom, center.GetReal(), center.GetImaginary()), z = new();
                    screen[x, height - y - 1] = MandelMath.BurningShipCalc(z, currentPoint, n);
                }
            });
        }

        #endregion

    }
}
