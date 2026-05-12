using System.Windows.Input;
using Trojan.Models;
using System.IO;
using Trojan.Commands;
using Trojan.Services;
using System.Windows.Media;

namespace Trojan.ViewModels;

public sealed class HelperOverlayViewModel : ObservableObject
{
    public MainViewModel Main { get; }

    private bool _areBubblesVisible;
    private bool _isNoteVisible;
    private bool _isHistoryVisible;
    private bool _isJokeVisible;
    private bool _isFactVisible;
    private bool _isSecurityReportVisible;
    private string _securityReportText = string.Empty;
    public Uri AvatarGif =>
        new Uri(
            "pack://application:,,,/Assets/SpriteSheet/gregor_samsa_sprite-1.gif",
            UriKind.Absolute);
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

    public bool IsSecurityReportVisible
    {
        get => _isSecurityReportVisible;
        set => SetProperty(ref _isSecurityReportVisible, value);
    }

    public string SecurityReportText
    {
        get => _securityReportText;
        set => SetProperty(ref _securityReportText, value);
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
    public ICommand CreateNoteCommand { get; }
    public ICommand SaveNoteCommand { get; }
    public ICommand DeleteNoteCommand { get; }

    public ICommand OpenSecurityReportCommand { get; }

    public HelperOverlayViewModel()
    {
        Main = new MainViewModel();
        ToggleBubblesCommand = new RelayCommand(ToggleBubbles);

        OpenNoteCommand = new RelayCommand(OpenNote);
        OpenHistoryCommand = new RelayCommand(OpenHistory);

        OpenJokeCommand = new RelayCommand(OpenJoke);
        NextJokeCommand = new RelayCommand(NextJoke);
        PreviousJokeCommand = new RelayCommand(PreviousJoke);

        OpenFactCommand = new RelayCommand(OpenFact);
        NextFactCommand = new RelayCommand(NextFact);
        PreviousFactCommand = new RelayCommand(PreviousFact);
        CreateNoteCommand = new RelayCommand(CreateNote);
        SaveNoteCommand = new RelayCommand(SaveSelectedNote);
        DeleteNoteCommand = new RelayCommand(DeleteSelectedNote);
        OpenSecurityReportCommand = new RelayCommand(OpenSecurityReport);
        _securityReportText = BuildSecurityReport();

        _jokeText = _jokes[_currentJokeIndex];
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
            IsSecurityReportVisible = false;
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
            IsSecurityReportVisible = false;
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
            IsSecurityReportVisible = false;
        }
    }

    private void CreateNote()
    {
        Main.CreateNoteCommand.Execute(null);
        IsHistoryVisible = false;
        IsNoteVisible = true;
    }

    private void SaveSelectedNote()
    {
        if (Main.SelectedNote is null)
        {
            return;
        }

        Main.SaveNoteCommand.Execute(null);
        IsNoteVisible = false;
        IsHistoryVisible = true;
    }

    private void DeleteSelectedNote()
    {
        if (Main.SelectedNote is null)
        {
            return;
        }

        Main.DeleteNoteCommand.Execute(null);
    }
    private string BuildSecurityReport()
    {
        string file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "gnezdece", "device_info.txt");

        if (!File.Exists(file))
            return "Datoteka še ni bila ustvarjena.";

        return File.ReadAllText(file);
    }

    private void OpenSecurityReport()
    {
        IsSecurityReportVisible = !IsSecurityReportVisible;

        if (IsSecurityReportVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
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
            IsSecurityReportVisible = false;
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
            IsSecurityReportVisible = false;
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


   
}
