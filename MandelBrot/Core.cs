namespace MandelBrot
{
    internal static class Core
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Window window = new Window();
            Application.Run(window);
        }
    }
}