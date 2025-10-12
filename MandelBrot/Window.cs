using System.CodeDom;
using System.Drawing.Imaging;
using System.Drawing;
using System.Windows.Forms;

namespace MandelBrot
{
    public partial class Window : Form
    {

        Bitmap canvas;

        private int width, height;

        private int iterations, paletteSize;
        private GradientColor gradient;
        private double zoom, zoomChange, powChange;
        private ComplexNumber center, jz;
        private bool mandel = true; //if true in mandel space, else julia
        private int[,] screenQ;

        public Window()
        {

            InitializeComponent();

            width = this.ClientSize.Width;
            height = this.ClientSize.Height;
            canvas = new Bitmap(width, height, PixelFormat.Format24bppRgb); //Initialize a canvas using the height & width

            //Initialize settings
            iterations = 100;                                                //How many iterations we will do
            paletteSize = 33;
            gradient = new GradientColor(); 
            gradient.setColors(Gradients.favoriteColors, Gradients.favoriteStops);   //Initiate the gradient, picked from Gradients
            gradient.generatePalette(paletteSize);                           //Calculate the gradient
            zoom = 1.0;                                                      //Initiate the 'camera' zoom
            zoomChange = 5.0;                                                //Initiate how much to zoom in by
            powChange = 0.01;                                                //Initiate how much to exponentiate z by
            center = new ComplexNumber(-0.5, 0);                             //Initiate center of 'camera'
            MandelMath.setWidthHeight(width, height);

            screenQ = new int[width, height];
            updateMandelBrot();                                               //Print the mandelbrot
            printData();

            this.MouseClick += new MouseEventHandler(clicks);                 
            this.MouseWheel += new MouseEventHandler(scrolls);              
            this.KeyDown += new KeyEventHandler(keys);
            this.Paint += MainForm_Paint; //Draws everything on the canvas to the window

        }
        //Prints out the mandelbrot given the current global vars
        private void updateMandelBrot()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.getC(x, y, zoom, center.getReal(), center.getImaginary()), z = new(); //Calculate the currentPoint given the (x, y) coordinates
                    screenQ[x, y] = MandelMath.mandelCalc(z, currentPoint, iterations);                   //Calculate q to determine if currentPoint is in the set (-1) or not in the set        
                }
            });
        }
        //Prints out the julia set given the current global vars
        private void updateJuliaSet()
        {
            Parallel.For(0, width, x =>
            {
                for (int y = 0; y < height; y++) //Loop over entire window
                {
                    ComplexNumber currentPoint = MandelMath.getC(x, y, zoom, center.getReal(), center.getImaginary());
                    screenQ[x, y] = MandelMath.mandelCalc(currentPoint, jz, iterations);
                }
            });
        }
        //Regenerate screen using updated values
        private void regen()
        {
            if (mandel) { updateMandelBrot(); }
            else        { updateJuliaSet();   }
        }
        private void printOrbit(int x, int y)
        {
            Bitmap orbits = new Bitmap(width, height);
            ComplexNumber currentPoint = MandelMath.getC(x, y, zoom, center.getReal(), center.getImaginary()), z = new();

            Point[] orbit = (mandel) ? orbit = MandelMath.mandelOrbit(z, currentPoint,  iterations, zoom, center.getReal(), center.getImaginary()) //Mandelbrot
                  : orbit = MandelMath.mandelOrbit(currentPoint, jz, iterations, zoom, center.getReal(), center.getImaginary());                   //Julia set

            using (Graphics g = Graphics.FromImage(orbits)) { for (int i = 1; i < orbit.Length; i++) g.DrawLine(Pens.Red, orbit[i - 1], orbit[i]); } //Draw a line for each orbit point

            Graphics gMain = Graphics.FromImage(canvas);
            gMain.DrawImage(orbits, Point.Empty);
            this.Invalidate();
        }
        private void printData()
        {
            int q = 0;
            for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
                {
                    q = screenQ[x, y];
                    canvas.SetPixel(x, y, (q == -1) ? Color.Black : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
                }
            this.Invalidate();
        }

        //-------Controls--------
        //Mouse:
        //  Left-Click : Zoom in  by zoomChange on mouses cords
        //  Right-Click: Zoom out by zoomChange on mouses cords
        //  Scroll-Up  : Increase pallateSize by 1 per section
        //  Scroll-Down: Decrease pallateSize by 1 per section
        //Keyboard:
        //  W: Increases the power at which Z is exponentiated
        //  S: Increases the power at which Z is exponentiated
        //  J: Print out a Julia set given the current mouse position
        //  M: Reset to MandelBrot
        //  O: Print Orbit
        //  0: Reset zoom & center to 1.00 (-0.5, 0i)
        //  1: Reset zoom & center to 0.75 (0, 0i)
        #region MandelControls
        //Left click = zoom in, right click = zoom out, middle click = increase iterations
        private void clicks(object sender, MouseEventArgs e)
        {

            int x = e.X; int y = e.Y;
            ComplexNumber clicked = MandelMath.getC(x, y, zoom, center.getReal(), center.getImaginary());
            double r = clicked.getReal(), i = clicked.getImaginary();
            center = clicked;

            switch (e.Button) 
            {
                case MouseButtons.Left:
                    zoom *= zoomChange; break;

                case MouseButtons.Right:
                    zoom *= (1 / zoomChange); break;
            }
            regen(); printData();
        }
        //Scroll up = gradient length++, scroll down = gradient length--;
        private void scrolls(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) { paletteSize += (e.Delta) / 120; }
            else { paletteSize += (e.Delta) / 120; }
            gradient.generatePalette(paletteSize);
            printData();
        }
        //
        private void keys(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    MandelMath.changePower(0.01); regen(); printData(); break;

                case Keys.S:
                    MandelMath.changePower(-0.01); regen(); break;

                case Keys.J:
                    Point screenPos1 = Cursor.Position, clientPos1 = this.PointToClient(screenPos1); //Gets the current mouses position
                    int x1 = Math.Max(Math.Min(clientPos1.X, width), 0);
                    int y1 = Math.Max(Math.Min(clientPos1.Y, height), 0);

                    jz = MandelMath.getC(x1, y1, zoom, center.getReal(), center.getImaginary());
                    mandel = false;
                    regen(); printData(); break;

                case Keys.D0:
                    zoom = 1.00; center = new ComplexNumber(-0.5, 0); regen(); break;

                case Keys.D1:
                    zoom = 0.75; center = new ComplexNumber(0.0, 0); regen(); break;

                case Keys.M:
                    mandel = true; regen(); break;

                case Keys.O:
                    Point screenPos2 = Cursor.Position, clientPos2 = this.PointToClient(screenPos2); //Gets the current mouses position
                    int x2 = Math.Max(Math.Min(clientPos2.X, width), 0);
                    int y2 = Math.Max(Math.Min(clientPos2.Y, height), 0);

                    printOrbit(x2, y2); break;
            }
        }
        #endregion
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, 0, 0); // Draw the bitmap to the window
        }
    }
}
