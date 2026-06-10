using System.Speech.Synthesis;
using Trojan.Services.Logger;

namespace Trojan.Services;

public sealed class TextToSpeechService : IDisposable
{
    private readonly SpeechSynthesizer _synthesizer;
    private TaskCompletionSource<bool>? _speakCompletion;
    private bool _isSpeaking;

    public event EventHandler? SpeakingStateChanged;

    public bool IsSpeaking
    {
        get => _isSpeaking;
        private set
        {
            if (_isSpeaking == value)
            {
                return;
            }

            _isSpeaking = value;
            SpeakingStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public TextToSpeechService()
    {
        _synthesizer = new SpeechSynthesizer();
        _synthesizer.SpeakCompleted += OnSpeakCompleted;

        try
        {
            var voice = _synthesizer
                .GetInstalledVoices()
                .Select(v => v.VoiceInfo)
                .FirstOrDefault(v => v.Culture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase)
                                     && v.Gender == VoiceGender.Male)
                ?? _synthesizer
                    .GetInstalledVoices()
                    .Select(v => v.VoiceInfo)
                    .FirstOrDefault(v => v.Culture.Name.StartsWith("en", StringComparison.OrdinalIgnoreCase));

            if (voice is not null)
            {
                _synthesizer.SelectVoice(voice.Name);
            }
        }
        catch (Exception ex)
        {
            AppLog.Info($"TTS voice selection failed: {ex.Message}");
        }
    }

    public async Task SpeakAsync(string text, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return;
        }

        Stop();

        _speakCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = cancellationToken.Register(Stop);

        try
        {
            IsSpeaking = true;
            _synthesizer.SpeakAsync(text);
            await _speakCompletion.Task.ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            AppLog.Info($"TTS speak failed: {ex.Message}");
        }
        finally
        {
            IsSpeaking = false;
            _speakCompletion = null;
        }
    }

    public void Stop()
    {
        try
        {
            _synthesizer.SpeakAsyncCancelAll();
        }
        catch (Exception ex)
        {
            AppLog.Info($"TTS stop failed: {ex.Message}");
        }

        _speakCompletion?.TrySetResult(false);
        IsSpeaking = false;
    }

    private void OnSpeakCompleted(object? sender, SpeakCompletedEventArgs e)
    {
        _speakCompletion?.TrySetResult(true);
        IsSpeaking = false;
    }

    public void Dispose()
    {
        _synthesizer.SpeakCompleted -= OnSpeakCompleted;
        _synthesizer.Dispose();
    }
}
