using System.CodeDom;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MandelBrot
{
    public partial class Window : Form
    {

        Bitmap canvas;
        FractalManager fctlMgr, jliaMgr;

        private int width, height;

        private int iterations, 
                    paletteSize;
        private Gradients.GradientColor gradient;
        private double  zoomChange = 5.00,
                        powChange  = 0.10;

        private String type = "mandel";
        private bool  julia = false;

        private Color inSetColor = Color.Black;

        private String[] options =
        {
            "Type: " , "Center (r, i):" , "Zoom", "n: ",
            "What C for Julia?:" , "Julia Center:" , "Julia Zoom"
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
            paletteSize = 33;
            gradient = new Gradients.GradientColor(); 
            gradient.setColors(Gradients.Gradients.surlendemainColors, Gradients.Gradients.surlendemainStops);   //Initiate the gradient, picked from Gradients
            gradient.generatePalette(paletteSize);                           //Calculate the gradient

            fctlMgr.Update("mandel");
            jliaMgr.SetJuliaC(new ComplexNumber(-0.75, 0.25));
            jliaMgr.SetZoom(0.90);
            jliaMgr.SetCenter(new ComplexNumber());
            jliaMgr.Update("mandel");
            printData();

            //Inputs
            this.MouseClick += new MouseEventHandler(clicks);                 
            this.MouseWheel += new MouseEventHandler(scrolls);              
            this.KeyDown += new KeyEventHandler(keys);
            this.Paint += MainForm_Paint; //Draws everything on the canvas to the window

        }
        
        
        //Regenerate screen using updated values
        private void regen()
        {
            fctlMgr.Update(type); 
            jliaMgr.Update(type);              
        }
        private void regenJulia()
        {
            jliaMgr.Update(type);
        }
        private void printOrbit(int x, int y)
        {
            
        }
        private void printData()
        {
            int q = 0;
            int [,] leftScreen = fctlMgr.GetScreen();   //Regular fractal
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)
            {
                q = leftScreen[x, y];
                canvas.SetPixel(x, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
            }
            int[,] rigthScreen = jliaMgr.GetScreen();   //Julia side
            for (int x = 0; x < width / 2; x++) for (int y = 0; y < height; y++)
                {
                    q = rigthScreen[x, y];
                    canvas.SetPixel(x + width / 2, y, (q == -1) ? inSetColor : gradient.getColor(q));  //If q is in the set (-1) paint the pixel black, else the color depends how how fast it diverged
                }
            this.Invalidate();  //Redraw screen
        }
        //Returns a string of information on the current window
        private String getInformation()
        {
            String juliaC = "";
            if (type.ToLower().Equals("mandel")) 
            {
                juliaC = fctlMgr.GetJuliaC().ToString();
                juliaC = "Julia C: " + juliaC;
            }

            return "Type?\t"            + type                  + "\n"
                    + "Center:\t"       + fctlMgr.GetCenter()   + "\n"
                    + "Zoom:\t"         + fctlMgr.GetZoom()     + "\n"
                    + "n:\t"            + fctlMgr.GetN()        + "\n"
                    + "Julia C:\t"      + jliaMgr.GetJuliaC()   + "\n"
                    + "Julia Center:\t" + jliaMgr.GetCenter()   + "\n"
                    + "Julia Zoom:\t"   + jliaMgr.GetZoom();
        }
        private String[] getInformationArray()
        {
            String[] arr = new String[7];

            arr[0] = type;
            arr[1] = fctlMgr.GetCenter().ToStringParen();
            arr[2] = fctlMgr.GetZoom()  .ToString();
            arr[3] = fctlMgr.GetN()     .ToString();
            arr[4] = jliaMgr.GetJuliaC().ToStringParen();
            arr[5] = jliaMgr.GetCenter().ToStringParen();
            arr[6] = jliaMgr.GetZoom()  .ToString();

            return arr;
        }
        private Boolean parseBool(String s)
        {
            switch (s.ToLower())
            {
                case "yup":
                case "yur":
                case "yes":
                case "y":
                case "t":
                case "true": return true; 
                    break;
                case "nah":
                case "no": 
                case "n": 
                case "f": 
                case "false": return false;
                    break;
                default: return false;
            }
        }
        private void showHelp()
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
        public static String[] getInputs(String[] prompts, String[] defaults)
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


        #region MandelControls
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
         *   W: Increases the power at which Z is exponentiated
         *   S: Increases the power at which Z is exponentiated
         *   
         *  Julia Controls:
         *   J: Print out a Julia set given the current mouse position
         *   Shift + J: Revert back
         *   
         *  Fractal Types:
         *   M: Go to Mandelbrot space
         *   B: Go to Burning Ship space
         *   T: Go to Tricorn space
         *   C: Go to Celtic space
         *   L: Go to Lambda space
         *  
         *  O: Print Orbit =====Doesn't work right now=======
         *  
         *  P: Printscreen (screenshot) [.png]
         *  0: Input custom coords & settings
         *  Zoom Defaults:
         *   1: Reset zoom & center to 1.00 (-0.50,  0.00i) Good for the entire Mandelbrot for lower n
         *   2: Reset zoom & center to 0.90 ( 0.00,  0.00i) Good for Julia sets and higer n Mandelbrots
         *   3: Reset zoom & center to 0.80 (-0.50, -0.00i) Good for Burning Ship
        **/
        private void clicks(object sender, MouseEventArgs e)
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
            regen(); printData();
        }
        private void scrolls(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0) { paletteSize += (e.Delta) / 120; }
            else { paletteSize += (e.Delta) / 120; }
            gradient.generatePalette(paletteSize);
            printData();
        }
        private void keys(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                //n / power control
                case Keys.W: fctlMgr.ChangeN( powChange ); regen(); printData(); break;
                case Keys.S: fctlMgr.ChangeN(-powChange ); regen(); printData(); break;

                //Julia control
                case Keys.J:

                    Point screenPos1 = Cursor.Position, clientPos1 = this.PointToClient(screenPos1); //Gets the current mouses position
                    int x1 = Math.Max(Math.Min(clientPos1.X, width), 0);
                    int y1 = Math.Max(Math.Min(clientPos1.Y, height), 0);

                    jliaMgr.SetJuliaC(MandelMath.GetC(x1, y1, fctlMgr.GetZoom(), fctlMgr.GetCenter().GetReal(), fctlMgr.GetCenter().GetImaginary())); //Gets the complex number we clicked on
                    julia = true;

                    regenJulia(); printData(); break;
                    
                //Prefab zooms
                case Keys.D1: 
                    fctlMgr.SetZoom(1.00); fctlMgr.SetCenter(new ComplexNumber(-0.5, 0));         
                    jliaMgr.SetZoom(1.00); jliaMgr.SetCenter(new ComplexNumber(-0.5, 0));       
                    regen(); printData(); break;
                case Keys.D2: 
                    fctlMgr.SetZoom(0.90); fctlMgr.SetCenter(new ComplexNumber(0.0, 0));          
                    jliaMgr.SetZoom(0.90); jliaMgr.SetCenter(new ComplexNumber(0.0, 0));     
                    regen(); printData(); break;
                case Keys.D3: 
                    fctlMgr.SetZoom(0.80); fctlMgr.SetCenter(new ComplexNumber(-0.50, -0.50));    
                    jliaMgr.SetZoom(0.80); jliaMgr.SetCenter(new ComplexNumber(-0.50, -0.50));    
                    regen(); printData(); break;

                //Changing fractals
                case Keys.M: type = "mandel";       regen(); printData(); break;
                case Keys.B: type = "burningship";  regen(); printData(); break;
                case Keys.T: type = "tricorn";      regen(); printData(); break;
                case Keys.C: type = "celtic";       regen(); printData(); break;

                case Keys.E:
                    String[] defaults =
                    {
                        MandelMath.GetIterations().ToString(),
                        MandelMath.GetEps().ToString()
                    };
                    String[] inputs = getInputs(settings, defaults);
                    if (inputs != null)
                    {
                        MandelMath.SetIterations(int.Parse(inputs[0]));
                        MandelMath.SetEps(Double.Parse(inputs[1]));
                        regen(); printData();
                    }
                    break;

                case Keys.O:
                    Point screenPos2 = Cursor.Position, clientPos2 = this.PointToClient(screenPos2); //Gets the current mouse position
                    int x2 = Math.Max(Math.Min(clientPos2.X, width), 0);
                    int y2 = Math.Max(Math.Min(clientPos2.Y, height), 0);

                    printOrbit(x2, y2); break;
                case Keys.P:
                    String confirm = ScreenShotter.SaveBitmapAsPng(canvas); //Pass in the canvas and the number
                    MessageBox.Show(confirm); break;
                case Keys.I:
                    MessageBox.Show(getInformation()); break;
                case Keys.D0:
                    inputs = getInputs(options, getInformationArray()); 
                    if (inputs != null) //If user input stuff parse the inputs
                    {
                        type = inputs[0];
                        fctlMgr.GetCenter().Parse(inputs[1]);
                        jliaMgr.GetCenter().Parse(inputs[5]);

                        jliaMgr.GetJuliaC().Parse(inputs[4]);

                        fctlMgr.SetZoom(Double.Parse(inputs[2]));
                        jliaMgr.SetZoom(Double.Parse(inputs[6]));

                        fctlMgr.SetN(Double.Parse(inputs[3]));
                        jliaMgr.SetN(Double.Parse(inputs[3]));

                        regen(); printData();
                    }
                    break;
                case Keys.H: showHelp(); break;

            }
        }
        #endregion
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(canvas, 0, 0); // Draw the bitmap to the window
        }
    }
}
