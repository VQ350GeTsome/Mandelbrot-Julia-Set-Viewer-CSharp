using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MandelBrot
{
    public static class ScreenShotter
    {

        public static String SaveBitmapAsPng(Bitmap bmp)
        {
            long ts = DateTime.UtcNow.Ticks;
            int rnd = new Random().Next(0, 10000);
            string id = $"{ts}_{rnd}";
            string currDir = Directory.GetCurrentDirectory();
            string folder = Path.Combine(currDir, "screenshots");
            Directory.CreateDirectory(folder);
            string savePath = Path.Combine(folder, id + ".png");
            bmp.Save(savePath, ImageFormat.Png);

            return savePath;
        }
    }
}
