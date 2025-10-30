using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;

namespace MandelBrot
{
    public partial class Window : Form
    {

        Bitmap canvas;
        FractalManager fctlMgr, jliaMgr;

        private int width, height;

        public static int   paletteSize,
                            paletteScrollDelta = 1,
                            ringSize = 1;

        private Gradients.GradientColor gradient;

        private double  zoomChange = 5.00,
                        powChange  = 0.10;

        private String  type = "mandel"; //Right side type

        private int colorMethod = 0;
        private String[] colorMethods =
        {
            "escapetime", "rings", "smoothescapetime"
        };

        private Color inSetColor = Color.Black;

        private String[] options =
        {
            "Type: " , "Center (r, i): " , "Zoom: ", "n: ",
            "What C for Julia?: " , "Julia Center: " , "Julia Zoom: ",
            "Phoenix P: "
        },
            settings =
        {
            "Iterations: ", "Epsilon: "
        };

        public Window()
        {

            InitializeComponent();

            width = this.ClientSize.Width;
            height = this.ClientSize.Height;
            canvas = new Bitmap(width, height, PixelFormat.Format24bppRgb); //Initialize a canvas using the height & width

            fctlMgr = new FractalManager(width / 2, height);
            jliaMgr = new FractalManager(width / 2, height, true);
            MandelMath.SetWidthHeight(width / 2, height);

            //Initialize Color setting    
            gradient = new Gradients.GradientColor();   //Initialize the gradient manager
            gradient.setColors(Gradients.Gradients.surlendemainColors, Gradients.Gradients.surlendemainStops);   //Initiate the gradient, picked from Gradients
            ColorMnH.SetNewType("escapetime");                       //Set the colormethods default type
            gradient.generatePalette(paletteSize);                       //Calculate the gradient

            fctlMgr.Update("mandel");
            jliaMgr.SetJuliaC(new(-0.75, 0.25));
            jliaMgr.SetZoom(0.90);
            jliaMgr.SetCenter(new ComplexNumber());
            jliaMgr.Update("mandel");
            PrintData();

            //Inputs
            this.MouseClick += new MouseEventHandler(Clicks);                 
            this.MouseWheel += new MouseEventHandler(Scrolls);              
            this.KeyDown    += new KeyEventHandler(Keys);
            this.Paint      += MainForm_Paint; //Draws everything on the canvas to the window

        }
        
        //Prints all the data to the screen using the current color method and gradient
        private void PrintData()
        {
            double q = 0;

            //Left side (regular fractal / Julia map)

            double[,] screen = fctlMgr.GetScreen();   //Get the right side screen
            if (colorMethods[colorMethod].Equals("rings"))
            {
                screen = ColorMnH.RingsPostProcessing(screen, ringSize);
            }
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)    //Loop over it and print it to the screen
            {
                q = screen[x, y];   //Get the escape iterations
                canvas.SetPixel(x, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
            }

            //Right side (Julia set)

            screen = jliaMgr.GetScreen();
            if (colorMethods[colorMethod].Equals("rings"))
            {
                screen = ColorMnH.RingsPostProcessing(screen, ringSize);
            }
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)
                {
                    q = screen[x, y];
                    canvas.SetPixel(x + width / 2, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
                }
            this.Invalidate();  //Redraw screen
        }
        //Regenerates both sides of the screen
        private void RegenBoth() { fctlMgr.Update(type); jliaMgr.Update(type); }
        //Regenerates the Mandel type side only
        private void RegenMandel() { fctlMgr.Update(type); }
        //Regenerates the Julia type side only
        private void RegenJulia() { jliaMgr.Update(type); }
        private void PrintOrbit(int x, int y)
        {
            
        }

        #region Information Helpers

        //Returns a String with the current information readable by the user
        private String GetInformation()
        {
            String phoenP = ""; //Additional information if it's the phoenix fractal
            if (type.ToLower().Equals("phoenix")) 
            {
                phoenP = fctlMgr.GetPhoenixP().ToString();
                phoenP = "Phoenix C: " + phoenP;
            }

            return "Type?\t"            + type                  + "\n"
                    + "Center:\t"       + fctlMgr.GetCenter()   + "\n"
                    + "Zoom:\t"         + fctlMgr.GetZoom()     + "\n"
                    + "n:\t"            + fctlMgr.GetN()        + "\n"
                    + "Julia C:\t"      + jliaMgr.GetJuliaC()   + "\n"
                    + "Julia Center:\t" + jliaMgr.GetCenter()   + "\n"
                    + "Julia Zoom:\t"   + jliaMgr.GetZoom()     + "\n"
                    + phoenP + "\n";
                
        }
        //Returns an array of String with the current information so it's readable by the program
        private String[] GetInformationArray()
        {
            String[] arr = new String[8];

            arr[0] = type;
            arr[1] = fctlMgr.GetCenter()    .ToStringNoi();
            arr[2] = fctlMgr.GetZoom()      .ToString();
            arr[3] = fctlMgr.GetN()         .ToString();
            arr[4] = jliaMgr.GetJuliaC()    .ToStringNoi();
            arr[5] = jliaMgr.GetCenter()    .ToStringNoi();
            arr[6] = jliaMgr.GetZoom()      .ToString();
            arr[7] = fctlMgr.GetPhoenixP()  .ToStringNoi();

            return arr;
        }
        private void ShowHelp()
        {
            Form helpForm = new Form
            {
                Text = "Controls Help",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterScreen
            };

            TextBox helpText = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Font = new System.Drawing.Font("Consolas", 12),
                ScrollBars = ScrollBars.Vertical,
                Text = @"
------- Controls -------

Mouse:
    Left-Click   : Zoom in by zoomChange on mouse coordinates
    Right-Click  : Zoom out by zoomChange on mouse coordinates
    Scroll-Up    : Increase paletteSize by 1 per section
    Scroll-Down  : Decrease paletteSize by 1 per section

Keyboard:
    W : Increase the power at which Z is exponentiated
    S : Decrease the power at which Z is exponentiated
    J : Print out a Julia set at current mouse position
    M : Reset to Mandelbrot
    O : Print Orbit
    Q : Increase iterations
    A : Decrease iterations
    P : Screenshot (saved as .png)
    T : Input custom coordinates & settings
    0 : Reset zoom & center to 1.00 (-0.5, 0i)
    1 : Reset zoom & center to 0.75 (0, 0i)"
            };

            helpForm.Controls.Add(helpText);
            helpForm.ShowDialog();
        }
        public static String[] GetInputs(String[] prompts, String[] defaults)
        {

            int padding = 50;

            Form form = new Form()
            {
                Width = 350,
                Height = 100 + prompts.Length * padding,
                Text = "Enter Settings",
                StartPosition = FormStartPosition.CenterScreen
            };

            TextBox[] inputBoxes = new TextBox[prompts.Length];

            for (int i = 0; i < prompts.Length; i++)
            {
                Label label = new Label()
                {
                    Left = 10,
                    Top = 10 + i * padding,
                    Width = 300,
                    Text = prompts[i]
                };

                TextBox textBox = new TextBox()
                {
                    Left = 10,
                    Top = 30 + i * padding,
                    Width = 300,
                    Text = (defaults != null && i < defaults.Length) ? defaults[i] : ""
                };

                form.Controls.Add(label);
                form.Controls.Add(textBox);
                inputBoxes[i] = textBox;
            }

            Button confirm = new Button()
            {
                Text = "OK",
                Left = 230,
                Width = 80,
                Top = 20 + prompts.Length * padding,
                DialogResult = DialogResult.OK
            };

            confirm.Click += (sender, e) => form.Close();
            form.Controls.Add(confirm);
            form.AcceptButton = confirm;

            if (form.ShowDialog() == DialogResult.OK)
            {
                string[] results = new string[prompts.Length];
                for (int i = 0; i < prompts.Length; i++)
                {
                    results[i] = inputBoxes[i].Text;
                }
                return results;
            }

            return null;
        }

        #endregion

        #region Fractal Control Handlers
        /**
         * -------Controls--------
         * Mouse:
         *  Left-Click : Zoom in  by zoomChange on mouses cords
         *  Right-Click: Zoom out by zoomChange on mouses cords
         *  Scroll-Up  : Increase pallateSize by 1 per section
         *  Scroll-Down: Decrease pallateSize by 1 per section
         *  
         * Keyboard:
         * 
         *  n Control:
         *   Q: Increases the power at which Z is exponentiated
         *   W: Decreases the power at which Z is exponentiated
         *   
         *  Julia Controls:
         *   J: Print out a Julia set given the current mouse position
         *          
         *  Fractal Types:
         *   Z: Go to Mandelbrot space
         *   X: Go to Burning Ship space
         *   C: Go to Tricorn space
         *   V: Go to Celtic space
         *   B: Go to Lambda space
         *   N: Go to Phoenix space
         *  
         *  O: Print Orbit =====Doesn't work right now=======
         *  
         *  P: Printscreen (screenshot) [.png]
         *  0: Input custom coords & settings
         *  E: Input custom render settings
         *  
         *  Zoom Defaults:
         *   1: Reset zoom & center to 1.00 (-0.50,  0.00i) Good for the entire Mandelbrot for lower n
         *   2: Reset zoom & center to 0.90 ( 0.00,  0.00i) Good for Julia sets and higer n Mandelbrots
         *   3: Reset zoom & center to 0.80 (-0.50, -0.00i) Good for Burning Ship
        **/
        private void Clicks(object sender, MouseEventArgs e)
        {

            int x = e.X; int y = e.Y;

            if (type.ToLower().Equals("burningship"))   //We flip the Burning Ship so we gotta flip the y values when we click too
            {
                y = height - y;
            }
            bool mandelSide = (x < width / 2);

            ComplexNumber clicked;

            if (mandelSide)
            {
                clicked = MandelMath.GetC(x, y, fctlMgr.GetZoom(), fctlMgr.GetCenter().GetReal(), fctlMgr.GetCenter().GetImaginary());
                fctlMgr.SetCenter(clicked);
            }
            else
            {
                clicked = MandelMath.GetC(x - (width / 2), y, jliaMgr.GetZoom(), jliaMgr.GetCenter().GetReal(), jliaMgr.GetCenter().GetImaginary());
                jliaMgr.SetCenter(clicked);
            }
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (mandelSide) fctlMgr.ChangeZoom(zoomChange);
                    else            jliaMgr.ChangeZoom(zoomChange);
                    break;
                case MouseButtons.Right:
                    if (mandelSide) fctlMgr.ChangeZoom(1.0 / zoomChange);
                    else            jliaMgr.ChangeZoom(1.0 / zoomChange);
                    break;
            }
            if (mandelSide) RegenMandel(); else RegenJulia(); //regenerate the one that has been changed

            PrintData();
        }
        private void Scrolls(object sender, MouseEventArgs e)
        {
            paletteSize += ((e.Delta) / 120) * paletteScrollDelta; 
            gradient.generatePalette(paletteSize);
            PrintData();
        }
        private void Keys(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //n / power control
                case System.Windows.Forms.Keys.Q:
                    fctlMgr.ChangeN(powChange); jliaMgr.ChangeN(powChange);
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.W:
                    fctlMgr.ChangeN(-powChange); jliaMgr.ChangeN(-powChange);
                    RegenBoth(); PrintData(); break;

                //Julia control
                case System.Windows.Forms.Keys.J:

                    Point screenPos1 = Cursor.Position, clientPos1 = this.PointToClient(screenPos1); //Gets the current mouses position
                    int x1 = Math.Max(Math.Min(clientPos1.X, width), 0);
                    int y1 = Math.Max(Math.Min(clientPos1.Y, height), 0);

                    jliaMgr.SetJuliaC(MandelMath.GetC(x1, y1, fctlMgr.GetZoom(), fctlMgr.GetCenter().GetReal(), fctlMgr.GetCenter().GetImaginary())); //Gets the complex number we clicked on

                    RegenJulia(); PrintData(); break;
                    
                //Prefab zooms
                case System.Windows.Forms.Keys.D1:
                    fctlMgr.SetZoom(1.00); fctlMgr.SetCenter(new ComplexNumber(-0.5, 0));
                    jliaMgr.SetZoom(1.00); jliaMgr.SetCenter(new ComplexNumber(-0.5, 0));
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.D2:
                    fctlMgr.SetZoom(0.90); fctlMgr.SetCenter(new ComplexNumber(0.0, 0));
                    jliaMgr.SetZoom(0.90); jliaMgr.SetCenter(new ComplexNumber(0.0, 0));
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.D3:
                    fctlMgr.SetZoom(0.80); fctlMgr.SetCenter(new ComplexNumber(-0.50, -0.50));
                    jliaMgr.SetZoom(0.80); jliaMgr.SetCenter(new ComplexNumber(-0.50, -0.50));
                    RegenBoth(); PrintData(); break;

                //Changing fractals
                case System.Windows.Forms.Keys.Z: type = "mandel"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.X: type = "burningship"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.C: type = "tricorn"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.V: type = "celtic"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.B: type = "lambda"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.N: type = "phoenix"; jliaMgr.SetJuliaC(new(.5666667, 0)); RegenBoth(); PrintData(); break;

                case System.Windows.Forms.Keys.E:
                    String[] defaults =
                    {
                        MandelMath.GetIterations().ToString(),
                        MandelMath.GetEps().ToString()
                    };
                    String[] inputs = GetInputs(settings, defaults);
                    if (inputs != null)
                    {
                        MandelMath.SetIterations(int.Parse(inputs[0]));
                        MandelMath.SetEps(double.Parse(inputs[1]));
                        RegenBoth(); PrintData();
                    }
                    break;

                case System.Windows.Forms.Keys.O:
                    Point screenPos2 = Cursor.Position, clientPos2 = this.PointToClient(screenPos2); //Gets the current mouse position
                    int x2 = Math.Max(Math.Min(clientPos2.X, width), 0);
                    int y2 = Math.Max(Math.Min(clientPos2.Y, height), 0);

                    PrintOrbit(x2, y2); break;
                case System.Windows.Forms.Keys.P:
                    String confirm = ScreenShotter.SaveBitmapAsPng(canvas); //Pass in the canvas and the number
                    MessageBox.Show(confirm); break;
                case System.Windows.Forms.Keys.I:
                    MessageBox.Show(GetInformation()); break;
                case System.Windows.Forms.Keys.D0:
                    inputs = GetInputs(options, GetInformationArray()); 
                    if (inputs != null) //If user input stuff parse the inputs
                    {
                        type = inputs[0];
                        fctlMgr.GetCenter().Parse(inputs[1]);
                        jliaMgr.GetCenter().Parse(inputs[5]);

                        jliaMgr.GetJuliaC().Parse(inputs[4]);

                        fctlMgr.SetZoom(double.Parse(inputs[2]));
                        jliaMgr.SetZoom(double.Parse(inputs[6]));

                        fctlMgr.SetN(double.Parse(inputs[3]));
                        jliaMgr.SetN(double.Parse(inputs[3]));

                        fctlMgr.GetPhoenixP().Parse(inputs[7]);
                        jliaMgr.GetPhoenixP().Parse(inputs[7]);

                        RegenBoth(); PrintData();
                    }
                    break;
                case System.Windows.Forms.Keys.H: ShowHelp(); break;
                case System.Windows.Forms.Keys.A:
                    colorMethod++;                                          //Increment colorMethod
                    colorMethod = (colorMethod + 1) % colorMethods.Length;  //Wrap colorMethod around the list
                    ColorMnH.SetNewType(colorMethods[colorMethod]);     //Set the colorMethod to the new type
                    gradient.generatePalette(paletteSize);                  //Regenerate the palette
                    RegenBoth(); PrintData(); break;                        //Regenerate the data & print it to the screen

            }
        }
        #endregion

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, 0, 0); // Draw the bitmap to the window
        }
    }
}
