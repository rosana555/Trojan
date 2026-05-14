using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trojan.Services.Logger
{
    public static class AppLog
    {
        public static void Info(string msg)
        {
            System.Diagnostics.Debug.WriteLine($"[INFO] {msg}");
        }
    }
}
