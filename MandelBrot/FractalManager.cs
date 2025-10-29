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

        bool julia;

        private ComplexNumber center    = new ComplexNumber(-0.50, 0.00),    //Center of the screen
                              juliaC    = new(),
                              phoenixP  = new(0.25, 0.00);

        public FractalManager(int w, int h, bool j = false) 
        {
            width = w; height = h;
            screen = new int[width, height];    //Initalize screen
            julia = j;
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

        public ComplexNumber GetPhoenixP() { return phoenixP; }
        public void SetPhoenixP(ComplexNumber p) { phoenixP = p; }

        public void Update(String type)
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.GetC(x, y, zoom, center.GetReal(), center.GetImaginary()), //Calculate the currentPoint given the (x, y) coordinates

                        z = (!julia) ? new() : currentPoint,    //If julia is false Z is a new complex number, else it's the current point
                        c = (!julia) ? currentPoint : juliaC;   //If julia is false C is the current point, else it's the C we picked for the julia set (juliaC)
                    switch (type)
                    {
                        case "mandel":          screen[x, y] = MandelMath.MandelCalc(z, c, n);  break;
                        case "burningship":     screen[x, height - y - 1] //Flip the y values so the fractal looks better
                                                    = MandelMath.BurningShipCalc(z, c, n);      break;
                        case "tricorn":         screen[x, y] = MandelMath.TricornCalc(z, c, n); break;
                        case "celtic":          screen[x, y] = MandelMath.CelticCalc(z, c, n);  break;
                        case "lambda":          screen[x, y] = MandelMath.LambdaCalc(z, c, n);  break;
                        case "phoenix":         screen[x, y] = MandelMath.PhoenixCalc(z, c, n, phoenixP); break;
                        //http://usefuljs.net/fractals/docs/mandelvariants.html

                        default: MessageBox.Show(type + " not a valid fractal type."); break;
                    }   
                }
            });
        }
    }
}
