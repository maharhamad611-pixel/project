using CourseEnrollment.WindowsApp.Forms;

namespace CourseEnrollment.WindowsApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
