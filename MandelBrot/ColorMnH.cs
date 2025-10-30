using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    /// <summary>
    /// Static class to help with managing
    /// the color methods and some post-
    /// processing effects
    /// </summary>
    public static class ColorMnH
    {
        public static String type;
        public static void SetNewType(String t)
        {
            type = t.ToLower();
            switch (type)
            {
                case "escapetime": 
                    Window.paletteScrollDelta = 1;      //Scrolls do more when we return i so lessen them to just 1
                    Window.paletteSize = 33;            //Reset the palette to the default 33 ... which i like             
                    break;
                case "smoothescapetime": 
                    Window.paletteScrollDelta = 10;     //Up the scroll delta to 10 since it takes more to see much of a change
                    Window.paletteSize = 1000;          //Up the paletteSize a ton because we multiply the return by 10
                    break;
                case "rings":
                    
                    break;
            }
        }

        public static double GetNewValue(int i, ComplexNumber z)
        {
            switch (type)
            {
                case "escapetime": ;
                    return i;                           
                    break;
                case "smoothescapetime": 
                    return 10 * (i + 1 - Math.Log(Math.Log(z.GetDistSqr())) / Math.Log(2)); 
                    break;

                default: return i; break;
            }
        }

        public static double[,] RingsPostProcessing(double[,] image)
        {
            int radius = 1,
                width  = image.GetLength(0),
                height = image.GetLength(1);

            double[,] returnImage = new double[width, height];

            for (int x = 0; x < width; x++) for (int y = 0; y < height; y++)
            {
                if (   x >= radius && x + radius < width && 
                       y >= radius && y + radius < height   )
                {
                        double  center = image[x, y],
                                above  = image[x, y - 1],
                                below  = image[x, y + 1],
                                left   = image[x - 1, y],
                                right  = image[x + 1, y];

                        bool anyDifferent = (center != above) || (center != below) ||
                                            (center != left)  || (center != right);

                        if (anyDifferent)
                        {
                            returnImage[x, y] = 1;
                        }
                }
            }
            return returnImage;
        }
    }
}
