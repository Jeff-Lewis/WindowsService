using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SecondService

{
    public partial class myService : ServiceBase
    {
        private bool isRunning = false;
        private String loggerName = "WindowsFormsApp2.exe";

        public myService()
        {
            InitializeComponent();
        }

        private void Run()
        {


            string exe = Process.GetCurrentProcess().MainModule.FileName;
            string path = Path.GetDirectoryName(exe);

            isRunning = true;
            while (isRunning)
            {
                ServiceUtils.startLoggerForAllUsers(path + "\\" + loggerName);
                Thread.Sleep(5000);
            }
        }

        protected override void OnStart(string[] args)
        {
            Thread t = new Thread(new ThreadStart(Run));
            t.IsBackground = true;
            t.Start();
        }

        protected override void OnStop()
        {
            isRunning = false;
        }
    }
}