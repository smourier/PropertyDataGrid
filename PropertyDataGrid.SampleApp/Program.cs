using System;

namespace PropertyDataGrid.SampleApp
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            ErrorBox.HandleExceptions(app);
            app.InitializeComponent();
            app.Run();
        }
    }
}