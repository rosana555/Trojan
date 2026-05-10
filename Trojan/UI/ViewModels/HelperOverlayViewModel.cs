using System.Windows.Input;
using Trojan.Core.Commands;
using System.Windows.Media;
using Trojan.Core.Base;
using Trojan.Core.Interface;

namespace Trojan.UI.ViewModels;

public sealed class HelperOverlayViewModel : ObservableObject
{
    private bool _areBubblesVisible;
    private readonly IAvatarSpriteService _avatarSpriteService;
    private bool _isNoteVisible;
    private bool _isHistoryVisible;
    private bool _isJokeVisible;
    private bool _isFactVisible;

    public bool IsNoteVisible
    {
        get => _isNoteVisible;
        set => SetProperty(ref _isNoteVisible, value);
    }

    public bool IsHistoryVisible
    {
        get => _isHistoryVisible;
        set => SetProperty(ref _isHistoryVisible, value);

    }
    public bool AreBubblesVisible
    {
        get => _areBubblesVisible;
        set => SetProperty(ref _areBubblesVisible, value);
    }

    public bool IsJokeVisible
    {
        get => _isJokeVisible;
        set => SetProperty(ref _isJokeVisible, value);

    }

    public bool IsFactVisible
    {
        get => _isFactVisible;
        set => SetProperty(ref _isFactVisible, value);
    }

    public ImageSource AvatarImage => _avatarSpriteService.CurrentFrameImage;
    public int AvatarFrameIndex
    {
        get => _avatarSpriteService.GetCurrentFrame();
        set => _avatarSpriteService.SetCurrentFrame(value);
    }

    public ICommand ToggleBubblesCommand { get; }
    public ICommand OpenNoteCommand { get; }
    public ICommand OpenHistoryCommand { get; }
    public ICommand OpenJokeCommand { get; }
    public ICommand NextJokeCommand { get; }
    public ICommand PreviousJokeCommand { get; }
    public ICommand OpenFactCommand { get; }
    public ICommand NextFactCommand { get; }
    public ICommand PreviousFactCommand { get; }


    public HelperOverlayViewModel(IAvatarSpriteService avatarSpriteService)
    {
        _avatarSpriteService = avatarSpriteService;
        _avatarSpriteService.PropertyChanged += OnAvatarSpriteServicePropertyChanged;
        ToggleBubblesCommand = new RelayCommand(ToggleBubbles);
        OpenNoteCommand = new RelayCommand(OpenNote);
        OpenHistoryCommand = new RelayCommand(OpenHistory);
        OpenJokeCommand = new RelayCommand(OpenJoke);
        NextJokeCommand = new RelayCommand(NextJoke);
        PreviousJokeCommand = new RelayCommand(PreviousJoke);
        _jokeText = _jokes[_currentJokeIndex];

        OpenFactCommand = new RelayCommand(OpenFact);
        NextFactCommand = new RelayCommand(NextFact);
        PreviousFactCommand = new RelayCommand(PreviousFact);

        _factText = _facts[_currentFactIndex];
    }

    private void ToggleBubbles()
    {
        AreBubblesVisible = !AreBubblesVisible;

        if (!AreBubblesVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
            IsJokeVisible = false;
            IsFactVisible = false;
        }
    }

    private void OpenNote()
    {
        IsNoteVisible = !IsNoteVisible;

        if (IsNoteVisible)
        {
            IsJokeVisible = false;
            IsHistoryVisible = false;
            IsFactVisible = false;
        }
    }

    private void OpenHistory()
    {
        IsHistoryVisible = !IsHistoryVisible;

        if (IsHistoryVisible)
        {
            IsNoteVisible = false;
            IsJokeVisible = false;
            IsFactVisible = false;
        }
    }

    private void OpenJoke()
    {
        IsJokeVisible = !IsJokeVisible;

        if (IsJokeVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
            IsFactVisible = false;
        }
    }

    private readonly List<string> _jokes =
[
    "Kam je šla Sally po eksploziji? Vsepovsod!",
    "Kako rečeš smrdljivemu duhu? SmrDUH!",
    "Zakaj programerji sovražijo naravo? Preveč bugov."
];

    private int _currentJokeIndex = 0;


    private readonly List<string> _facts =
[
    "Hobotnice imajo tri srca.",
    "Banane so tehnično jagodičevje.",
    "Med nikoli ne poteče."
];

    private int _currentFactIndex = 0;


    private void NextJoke()
    {
        if (_currentJokeIndex < _jokes.Count - 1)
        {
            _currentJokeIndex++;
            JokeText = _jokes[_currentJokeIndex];
        }
    }

    private void PreviousJoke()
    {
        if (_currentJokeIndex > 0)
        {
            _currentJokeIndex--;
            JokeText = _jokes[_currentJokeIndex];
        }
    }


    private string _factText;

    public string FactText
    {
        get => _factText;
        set => SetProperty(ref _factText, value);

    }
    private string _jokeText;
    public string JokeText
    {
        get => _jokeText;
        set => SetProperty(ref _jokeText, value);

    }

    private void OpenFact()
    {
        IsFactVisible = !IsFactVisible;

        if (IsFactVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
            IsJokeVisible = false;
        }
    }
    private void NextFact()
    {
        if (_currentFactIndex < _facts.Count - 1)
        {
            _currentFactIndex++;
            FactText = _facts[_currentFactIndex];
        }
    }

    private void PreviousFact()
    {
        if (_currentFactIndex > 0)
        {
            _currentFactIndex--;
            FactText = _facts[_currentFactIndex];
        }

    }
    private void OnAvatarSpriteServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAvatarSpriteService.CurrentFrameImage))
        {
            OnPropertyChanged(nameof(AvatarImage));
        }

        if (e.PropertyName == nameof(IAvatarSpriteService.CurrentFrameIndex))
        {
            OnPropertyChanged(nameof(AvatarFrameIndex));
        }
    }
}
