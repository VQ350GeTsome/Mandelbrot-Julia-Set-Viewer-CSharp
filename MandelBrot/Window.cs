using System.Drawing.Imaging;
using System.Windows.Forms.VisualStyles;

namespace MandelBrot
{
    public partial class Window : Form
    {

        Bitmap canvas;
        FractalManager mandelMgr, juliaMgr;

        private int width, height;

        public static int   paletteSize         = 33,
                            paletteScrollDelta  =  1;

        private int ringSize    =    5;
        private bool ringEdges  = true;
            

        private Gradients.GradientColor gradient = new Gradients.GradientColor();

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

            width = this.ClientSize.Width; height = this.ClientSize.Height; //Get the width & height of the screen
            canvas = new Bitmap(width, height, PixelFormat.Format24bppRgb); //Initialize a canvas using the height & width

            mandelMgr = new FractalManager(width / 2, height);              //Instantiate the mandel type manager field with the width & height
            juliaMgr  = new FractalManager(width / 2, height, true);        //Instantiate the julia  type manager field
            MandelMath.SetWidthHeight(width / 2, height);

            gradient.generatePalette(paletteSize);                       //Calculate the gradient

            mandelMgr.Update("mandel");
            juliaMgr.SetJuliaC(new(-0.75, 0.25)); 
            juliaMgr.SetZoom(0.90);
            juliaMgr.SetCenter(new ComplexNumber());
            juliaMgr.Update("mandel");
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

            double[,] screen = mandelMgr.GetScreen();   //Get the right side screen
            if (colorMethods[colorMethod].Equals("rings"))
            {
                screen = ColorMnH.RingsPostProcessing(screen, ringSize, edge: ringEdges);
            }
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)    //Loop over it and print it to the screen
            {
                q = screen[x, y];   //Get the escape iterations
                canvas.SetPixel(x, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
            }

            //Right side (Julia set)

            screen = juliaMgr.GetScreen();
            if (colorMethods[colorMethod].Equals("rings"))
            {
                screen = ColorMnH.RingsPostProcessing(screen, ringSize, edge: ringEdges);
            }
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)
                {
                    q = screen[x, y];
                    canvas.SetPixel(x + width / 2, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
                }
            this.Invalidate();  //Redraw screen
        }
        //Regenerates both sides of the screen
        private void RegenBoth() { mandelMgr.Update(type); juliaMgr.Update(type); }
        //Regenerates the Mandel type side only
        private void RegenMandel() { mandelMgr.Update(type); }
        //Regenerates the Julia type side only
        private void RegenJulia() { juliaMgr.Update(type); }
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
                phoenP = mandelMgr.GetPhoenixP().ToString();
                phoenP = "Phoenix C: " + phoenP;
            }

            return "Type?\t\t"          + type                  + "\n"
                    + "Center:\t\t"     + mandelMgr.GetCenter()   + "\n"
                    + "Zoom:\t\t"       + mandelMgr.GetZoom()     + "\n"
                    + "n:\t\t"          + mandelMgr.GetN()        + "\n"
                    + "Julia C:\t\t"    + juliaMgr.GetJuliaC()   + "\n"
                    + "Julia Center:\t" + juliaMgr.GetCenter()   + "\n"
                    + "Julia Zoom:\t"   + juliaMgr.GetZoom()     + "\n"
                    + phoenP + "\n";
                
        }
        //Returns an array of String with the current information so it's readable by the program
        private String[] GetInformationArray()
        {
            String[] arr = new String[8];

            arr[0] = type;
            arr[1] = mandelMgr.GetCenter()    .ToStringNoi();
            arr[2] = mandelMgr.GetZoom()      .ToString();
            arr[3] = mandelMgr.GetN()         .ToString();
            arr[4] = juliaMgr.GetJuliaC()    .ToStringNoi();
            arr[5] = juliaMgr.GetCenter()    .ToStringNoi();
            arr[6] = juliaMgr.GetZoom()      .ToString();
            arr[7] = mandelMgr.GetPhoenixP()  .ToStringNoi();

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
                clicked = MandelMath.GetC(x, y, mandelMgr.GetZoom(), mandelMgr.GetCenter().GetReal(), mandelMgr.GetCenter().GetImaginary());
                mandelMgr.SetCenter(clicked);
            }
            else
            {
                clicked = MandelMath.GetC(x - (width / 2), y, juliaMgr.GetZoom(), juliaMgr.GetCenter().GetReal(), juliaMgr.GetCenter().GetImaginary());
                juliaMgr.SetCenter(clicked);
            }
            switch (e.Button)
            {
                case MouseButtons.Left:
                    if (mandelSide) mandelMgr.ChangeZoom(zoomChange);
                    else            juliaMgr.ChangeZoom(zoomChange);
                    break;
                case MouseButtons.Right:
                    if (mandelSide) mandelMgr.ChangeZoom(1.0 / zoomChange);
                    else            juliaMgr.ChangeZoom(1.0 / zoomChange);
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
                    mandelMgr.ChangeN(powChange); juliaMgr.ChangeN(powChange);
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.W:
                    mandelMgr.ChangeN(-powChange); juliaMgr.ChangeN(-powChange);
                    RegenBoth(); PrintData(); break;

                //Julia control
                case System.Windows.Forms.Keys.J:

                    Point screenPos1 = Cursor.Position, clientPos1 = this.PointToClient(screenPos1); //Gets the current mouses position
                    int x1 = Math.Max(Math.Min(clientPos1.X, width), 0);
                    int y1 = Math.Max(Math.Min(clientPos1.Y, height), 0);

                    juliaMgr.SetJuliaC(MandelMath.GetC(x1, y1, mandelMgr.GetZoom(), mandelMgr.GetCenter().GetReal(), mandelMgr.GetCenter().GetImaginary())); //Gets the complex number we clicked on

                    RegenJulia(); PrintData(); break;
                    
                //Prefab zooms
                case System.Windows.Forms.Keys.D1:
                    mandelMgr.SetZoom(1.00); mandelMgr.SetCenter(new ComplexNumber(-0.5, 0));
                    juliaMgr.SetZoom(1.00); juliaMgr.SetCenter(new ComplexNumber(-0.5, 0));
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.D2:
                    mandelMgr.SetZoom(0.90); mandelMgr.SetCenter(new ComplexNumber(0.0, 0));
                    juliaMgr.SetZoom(0.90); juliaMgr.SetCenter(new ComplexNumber(0.0, 0));
                    RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.D3:
                    mandelMgr.SetZoom(0.80); mandelMgr.SetCenter(new ComplexNumber(-0.50, -0.50));
                    juliaMgr.SetZoom(0.80); juliaMgr.SetCenter(new ComplexNumber(-0.50, -0.50));
                    RegenBoth(); PrintData(); break;

                //Changing fractals
                case System.Windows.Forms.Keys.Z: type = "mandel"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.X: type = "burningship"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.C: type = "tricorn"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.V: type = "celtic"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.B: type = "lambda"; RegenBoth(); PrintData(); break;
                case System.Windows.Forms.Keys.N: type = "phoenix"; juliaMgr.SetJuliaC(new(.5666667, 0)); RegenBoth(); PrintData(); break;

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
                        mandelMgr.GetCenter().Parse(inputs[1]);
                        juliaMgr.GetCenter().Parse(inputs[5]);

                        juliaMgr.GetJuliaC().Parse(inputs[4]);

                        mandelMgr.SetZoom(double.Parse(inputs[2]));
                        juliaMgr.SetZoom(double.Parse(inputs[6]));

                        mandelMgr.SetN(double.Parse(inputs[3]));
                        juliaMgr.SetN(double.Parse(inputs[3]));

                        mandelMgr.GetPhoenixP().Parse(inputs[7]);
                        juliaMgr.GetPhoenixP().Parse(inputs[7]);

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
