using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot.Gradients
{
    public class GradientColor
    {
        private Color[] palette = Array.Empty<Color>();

        public void generatePalette(int size)
        {
            if (size <= 0 || colors.Length == 0 || locations.Length == 0 || colors.Length != locations.Length)
            {
                palette = new Color[1] { Color.Black };
                return;
            }

            palette = new Color[size + 1];
            for (int i = 0; i <= size; i++)
            {
                double percent = (double) i / size;
                palette[i] = getInterpolatedColor(percent);
            }
        }

        public Color getColor(double iteration)
        {
            if (palette == null || palette.Length == 0)
                return Color.Black;

            double idx = (iteration % palette.Length + palette.Length) % palette.Length;

            return palette[(int) idx];
        }

        private Color[] colors = Array.Empty<Color>();
        private double[] locations = Array.Empty<double>();

        public void setColors(Color[] colorArray, double[] locationArray)
        {
            if (colorArray == null || locationArray == null || colorArray.Length != locationArray.Length)
                throw new ArgumentException("Color and location arrays must be non-null and of equal length.");

            colors = colorArray;
            locations = locationArray;
        }

        private Color getInterpolatedColor(double percent)
        {
            percent = Math.Clamp(percent, 0.0, 1.0);

            if (colors.Length == 1 || percent <= locations[0])
                return colors[0];

            if (percent >= locations[^1])
                return colors[^1];

            for (int i = 1; i < locations.Length; i++)
            {
                double left = locations[i - 1];
                double right = locations[i];

                if (percent >= left && percent <= right)
                {
                    double w = right - left == 0.0 ? 0.0 : (percent - left) / (right - left);
                    return blendColor(colors[i - 1], colors[i], w);
                }
            }

            return Color.Black;
        }

        private Color blendColor(Color from, Color to, double weight)
        {
            weight = Math.Clamp(weight, 0.0, 1.0);
            int a = (int)(from.A + (to.A - from.A) * weight);
            int r = (int)(from.R + (to.R - from.R) * weight);
            int g = (int)(from.G + (to.G - from.G) * weight);
            int b = (int)(from.B + (to.B - from.B) * weight);
            return Color.FromArgb(a, r, g, b);
        }
    }
}
