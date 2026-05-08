using System;
using System.Windows.Forms;
using OnlineBookstoreWinForms.Forms;

namespace OnlineBookstoreWinForms;

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        // Show login first; Application.Run() without a form keeps the message
        // loop alive independently — it exits only when Application.Exit() is called.
        new LoginForm().Show();
        Application.Run();
    }
}
