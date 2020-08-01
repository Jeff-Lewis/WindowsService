using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            MonitoringManager.Instance.RegisterEventHandlers();
            Application.Run(new ApplicationContext());
        }
    }
}
