namespace CoraCatte
{
    internal static class Program
    {
        private static bool meow;

        public static bool Meow { get => meow; set => meow = value; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            meow = true;
        }
    }
}