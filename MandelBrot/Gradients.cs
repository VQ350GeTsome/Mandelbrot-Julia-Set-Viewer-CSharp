using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    /// <summary>
    /// Class that contains a wide selection of gradients.
    /// Each gradient is an array[n] of colors and a corresponding array[n] of stops. 
    /// </summary>
    internal static class Gradients
    {
        // Blazing fire gradient
        public static readonly Color[] fireColors = new Color[]
            {
                Color.FromArgb(255, 0, 0, 0),        // 0.00 black
                Color.FromArgb(255, 40, 0, 0),       // 0.02 deep maroon
                Color.FromArgb(255, 180, 20, 0),     // 0.10 red-orange
                Color.FromArgb(255, 255, 80, 0),     // 0.25 orange
                Color.FromArgb(255, 255, 200, 40),   // 0.55 yellow-orange
                Color.FromArgb(255, 255, 255, 200)   // 1.00 near-white hot
            };
        public static readonly double[] fireStops = new double[] { 0.0, 0.02, 0.10, 0.25, 0.55, 1.0 };

        // Icey aurorra gradient
        public static readonly Color[] iceAuroraColors = new Color[]
            {
                Color.FromArgb(255, 0, 0, 20),       // 0.00 deep navy
                Color.FromArgb(255, 0, 40, 100),     // 0.10 cold blue
                Color.FromArgb(255, 0, 200, 255),    // 0.30 electric cyan
                Color.FromArgb(255, 80, 255, 180),   // 0.50 mint green
                Color.FromArgb(255, 180, 255, 220),  // 0.75 pale aqua
                Color.FromArgb(255, 255, 255, 255)   // 1.00 white
            };
        public static readonly double[] iceAuroraStops = new double[] { 0.0, 0.10, 0.30, 0.50, 0.75, 1.0 };

        // Sunset warm gradient
        public static readonly Color[] sunsetColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 10, 10, 30),    // 0.00 deep indigo
            System.Drawing.Color.FromArgb(255, 60, 10, 90),    // 0.15 purple
            System.Drawing.Color.FromArgb(255, 200, 40, 120),  // 0.35 magenta
            System.Drawing.Color.FromArgb(255, 255, 120, 60),  // 0.60 orange
            System.Drawing.Color.FromArgb(255, 255, 200, 80),  // 0.80 peach
            System.Drawing.Color.FromArgb(255, 255, 240, 180)  // 1.00 soft gold
        };
        public static readonly double[] sunsetStops = new double[] { 0.0, 0.15, 0.35, 0.60, 0.80, 1.0 };

        // Deep ocean gradient
        public static readonly Color[] oceanColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 0, 0, 20),      // 0.00 almost black
            System.Drawing.Color.FromArgb(255, 0, 30, 80),     // 0.10 navy
            System.Drawing.Color.FromArgb(255, 0, 110, 160),   // 0.35 deep teal
            System.Drawing.Color.FromArgb(255, 20, 180, 200),  // 0.60 tropical blue
            System.Drawing.Color.FromArgb(255, 160, 240, 255), // 0.85 pale cyan
            System.Drawing.Color.FromArgb(255, 240, 255, 255)  // 1.00 near-white
        };
        public static readonly double[] oceanStops = new double[] { 0.0, 0.10, 0.35, 0.60, 0.85, 1.0 };

        // Forest canopy gradient
        public static readonly Color[] forestColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 5, 20, 0),      // 0.00 almost black green
            System.Drawing.Color.FromArgb(255, 10, 70, 20),    // 0.10 deep green
            System.Drawing.Color.FromArgb(255, 30, 140, 40),   // 0.35 leafy green
            System.Drawing.Color.FromArgb(255, 120, 200, 80),  // 0.65 moss
            System.Drawing.Color.FromArgb(255, 200, 240, 160), // 0.90 light leaf
            System.Drawing.Color.FromArgb(255, 250, 255, 240)  // 1.00 light cream
        };
        public static readonly double[] forestStops = new double[] { 0.0, 0.10, 0.35, 0.65, 0.90, 1.0 };

        // Neon cyber gradient
        public static readonly Color[] neonColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 5, 0, 30),      // 0.00 dark violet
            System.Drawing.Color.FromArgb(255, 60, 0, 140),    // 0.20 electric indigo
            System.Drawing.Color.FromArgb(255, 0, 200, 255),   // 0.45 electric cyan
            System.Drawing.Color.FromArgb(255, 0, 255, 140),   // 0.70 neon green
            System.Drawing.Color.FromArgb(255, 255, 80, 180)   // 1.00 hot pink
        };
        public static readonly double[] neonStops = new double[] { 0.0, 0.20, 0.45, 0.70, 1.0 };

        // Lava gradient
        public static readonly Color[] lavaColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 5, 0, 0),       // 0.00 black-red
            System.Drawing.Color.FromArgb(255, 80, 0, 0),      // 0.10 dark maroon
            System.Drawing.Color.FromArgb(255, 180, 30, 0),    // 0.35 ember
            System.Drawing.Color.FromArgb(255, 255, 90, 0),    // 0.60 molten orange
            System.Drawing.Color.FromArgb(255, 255, 200, 20),  // 0.85 bright yellow
            System.Drawing.Color.FromArgb(255, 255, 255, 180)  // 1.00 white-hot
        };
        public static readonly double[] lavaStops = new double[] { 0.0, 0.10, 0.35, 0.60, 0.85, 1.0 };

        // Pastel sunrise gradient
        public static readonly Color[] pastelColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 250, 240, 255), // 0.00 lavender
            System.Drawing.Color.FromArgb(255, 240, 220, 255), // 0.20 pale mauve
            System.Drawing.Color.FromArgb(255, 255, 200, 230), // 0.45 soft rose
            System.Drawing.Color.FromArgb(255, 255, 235, 200), // 0.70 warm cream
            System.Drawing.Color.FromArgb(255, 240, 255, 230)  // 1.00 mint cream
        };
        public static readonly double[] pastelStops = new double[] { 0.0, 0.20, 0.45, 0.70, 1.0 };

        // Galaxy gradient with deep space hues
        public static readonly Color[] galaxyColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 0, 0, 15),      // 0.00 space black
            System.Drawing.Color.FromArgb(255, 20, 10, 70),    // 0.12 deep purple
            System.Drawing.Color.FromArgb(255, 80, 40, 180),   // 0.30 violet starfield
            System.Drawing.Color.FromArgb(255, 120, 180, 255), // 0.60 bluish nebula
            System.Drawing.Color.FromArgb(255, 255, 240, 200)  // 1.00 bright core
        };
        public static readonly double[] galaxyStops = new double[] { 0.0, 0.12, 0.30, 0.60, 1.0 };

        // Earth gradient
        public static readonly Color[] earthColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 2, 20, 60),    // 0.00 deep ocean
            System.Drawing.Color.FromArgb(255, 0, 90, 140),   // 0.12 coastal blue
            System.Drawing.Color.FromArgb(255, 24, 140, 80),  // 0.30 fertile green
            System.Drawing.Color.FromArgb(255, 34, 100, 40),  // 0.55 dense forest
            System.Drawing.Color.FromArgb(255, 120, 90, 60),  // 0.78 mountain rock
            System.Drawing.Color.FromArgb(255, 245, 245, 250) // 1.00 snowcap
        };
        public static readonly double[] earthStops = new double[] { 0.0, 0.12, 0.30, 0.55, 0.78, 1.0 };

        // Sky gradient
        public static readonly Color[] skyColors = new   Color[]
        {
            System.Drawing.Color.FromArgb(255, 3, 8, 30),     // 0.00 midnight
            System.Drawing.Color.FromArgb(255, 20, 40, 90),   // 0.18 twilight
            System.Drawing.Color.FromArgb(255, 15, 110, 220), // 0.45 vivid noon blue
            System.Drawing.Color.FromArgb(255, 150, 200, 245),// 0.75 pale sky
            System.Drawing.Color.FromArgb(255, 255, 210, 160) // 1.00 warm sunrise
        };
        public static readonly double[] skyStops = new double[] { 0.0, 0.18, 0.45, 0.75, 1.0 };

        // My favorite
        public static readonly Color[] favoriteColors = new Color[]
        {
            System.Drawing.Color.FromArgb(255, 0, 0, 0),    
            System.Drawing.Color.FromArgb(255, 255, 204, 0),  
            System.Drawing.Color.FromArgb(255, 135, 31, 19), 
            System.Drawing.Color.FromArgb(255, 0, 0, 153),
            System.Drawing.Color.FromArgb(255, 0, 77, 255),
            System.Drawing.Color.FromArgb(255, 0, 0, 0)
        };
        public static readonly double[] favoriteStops = new double[] { 0.0, 0.16666, 0.3333333, 0.66666666, 0.8333333, 1.0 };

        public static readonly Color[] redBlackWoodColors = new Color[]
        {
            Color.FromArgb(255, 8, 6, 5),       // 0.00 very dark (near-black warm)
            Color.FromArgb(255, 32, 8, 6),      // 0.10 charred wood undertone
            Color.FromArgb(255, 90, 24, 12),    // 0.25 deep burnt brown
            Color.FromArgb(255, 150, 40, 18),   // 0.45 warm mahogany
            Color.FromArgb(255, 210, 70, 20),   // 0.70 vivid red-wood
            Color.FromArgb(255, 255, 100, 40)   // 1.00 hot red highlight
        };
        public static readonly double[] redBlackWoodStops = new double[] { 0.00, 0.10, 0.25, 0.45, 0.70, 1.00 };
    }
}
