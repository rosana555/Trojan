using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Trojan.Core.Base;
using Trojan.Core.Commands;
using Trojan.Core.Interface;
using Trojan.Core.Models;
using Trojan.Services;
using Trojan.Services.Logger;
using Trojan.Views;
using System.Threading.Tasks;

namespace Trojan.UI.ViewModels;

public sealed class HelperOverlayViewModel : ObservableObject
{
    public MainViewModel Main { get; }

    private bool _areBubblesVisible;
    private bool _isNoteVisible;
    private bool _isHistoryVisible;
    private bool _isJokeVisible;
    private bool _isFactVisible;
    private bool _isSecurityReportVisible;
    private bool _isGallaryVisible;
    private string _securityReportText = string.Empty;
    private GalleryViewModel _gallery;

    private Uri _avatarGif =
    new Uri(
        "pack://application:,,,/UI/Assets/SpriteSheet/gregor_samsa_sprite-1.gif",
        UriKind.Absolute);

    public Uri AvatarGif
    {
        get => _avatarGif;
        set => SetProperty(ref _avatarGif, value);
    }
    public bool IsNoteVisible
    {
        get => _isNoteVisible;
        set => SetProperty(ref _isNoteVisible, value);
    }

    public GalleryViewModel Gallery
    {
        get => _gallery;
        set => SetProperty(ref _gallery, value);
    }
    public bool IsHistoryVisible
    {
        get => _isHistoryVisible;
        set => SetProperty(ref _isHistoryVisible, value);

    }

    public bool IsGallaryVisible
    {
        get => _isGallaryVisible;
        set => SetProperty(ref _isGallaryVisible, value);
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
    public ICommand OpenGallaryCommand { get; }
    public ICommand OpenNoteForNoteCommand { get; }
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
    public ICommand TogglePinCommand { get; }
    public ICommand TogglePinForNoteCommand { get; }

    public ICommand OpenSecurityReportCommand { get; }

    public ICommand UnlockFolderCommand { get; }


    public HelperOverlayViewModel()
    {
        Main = new MainViewModel();
        ToggleBubblesCommand = new RelayCommand(ToggleBubbles);
        Gallery = new GalleryViewModel();
        OpenNoteCommand = new RelayCommand(OpenNote);
        OpenGallaryCommand = new RelayCommand(OpenGallary);
        OpenNoteForNoteCommand = new RelayCommand<Note>(OpenNoteForNote);
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
        TogglePinCommand = new RelayCommand(TogglePin);
        TogglePinForNoteCommand = Main.TogglePinForNoteCommand;
        OpenSecurityReportCommand = new RelayCommand(OpenSecurityReport);
        _securityReportText = BuildSecurityReport();

        UnlockFolderCommand = new RelayCommand(UnlockFolder);


        _jokeText = _jokes[_currentJokeIndex];
        _factText = _facts[_currentFactIndex];
        _ = Task.Run(() =>
        {
            Thread.Sleep(2000); // sačekaj 2 sekunde da se aplikacija učita
            DeviceScannerService.LockFolder();
        });
    }

    private void OpenNoteForNote(Note? note)
    {
        if (note is not null)
        {
            Main.SelectedNote = note;
        }

        if (!IsNoteVisible)
        {
            OpenNote();
        }
        else
        {
            IsHistoryVisible = false;
            IsJokeVisible = false;
            IsFactVisible = false;
            IsSecurityReportVisible = false;
            IsGallaryVisible = false;
        }
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
            IsGallaryVisible = false;
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
            IsGallaryVisible = false;
        }
    }

    private void OpenGallary()
    {
        AppLog.Info($"OpenGallary called! Current IsGallaryVisible: {IsGallaryVisible}");
        IsGallaryVisible = !IsGallaryVisible;
        AppLog.Info($"After toggle IsGallaryVisible: {IsGallaryVisible}");
        if (IsGallaryVisible)
        {
            IsJokeVisible = false;
            IsHistoryVisible = false;
            IsFactVisible = false;
            IsSecurityReportVisible = false;
            IsNoteVisible = false;
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
            IsGallaryVisible = false;
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

    private async void DeleteSelectedNote()
    {
        if (Main.SelectedNote is null)
        {
            return;
        }

        Main.DeleteNoteCommand.Execute(null);
        await ShowOverlaySequenceAsync();

    }

    private void TogglePin()
    {
        if (Main.SelectedNote is null)
        {
            return;
        }

        Main.TogglePinCommand.Execute(null);
    }
    private string BuildSecurityReport()
    {

        string file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "gnezdece", "device_info.txt");

        if (!File.Exists(file))
            return "Datoteka �e ni bila ustvarjena.";

        return File.ReadAllText(file);
    }
    private void UnlockFolder()
    {
        // Prvo pitaj za lozinku
        string password = Microsoft.VisualBasic.Interaction.InputBox(
            "Unesite lozinku za dešifrovanje fajlova:",
            "Dešifrovanje",
            "");

        if (string.IsNullOrEmpty(password))
        {
            MessageBox.Show("Lozinka nije uneta!", "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        DeviceScannerService.UnlockFolder(password);
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

    private static List<string> ReadJokes()
    {
        var list = new List<string>();
        var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Joke", "joke.csv");
        using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(file))
        {
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields(); //field[0] = ID fields[1] = Joke
                if (fields.Length >= 2)
                {
                    list.Add(fields[1]);
                }
            }
        }
        return list;
    }

    private readonly List<string> _jokes = ReadJokes();

    private void PreviousJoke()
    {
        if (_currentJokeIndex > 0)
        {
            _currentJokeIndex--;
            JokeText = _jokes[_currentJokeIndex];
        }
    }

    private int _currentJokeIndex = 0;


    private static List<string> ReadFacts()
    {
        var list = new List<string>();
        var file = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Fact", "fact.csv");
        using (var parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(file))
        {
            parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
            parser.SetDelimiters(",");
            parser.ReadLine();
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                if (fields.Length >= 2)
                {
                    list.Add(fields[1]);
                }
            }
        }
        return list;
    }

    private readonly List<string> _facts = ReadFacts();

    private int _currentFactIndex = 0;


    private void NextJoke()
    {
        Random rand = new Random();
        int _currentJokeIndex = rand.Next(_jokes.Count - 1);
        JokeText = _jokes[_currentJokeIndex];
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
        Random rand = new Random();
        int _currentFactIndex = rand.Next(_facts.Count - 1);
        FactText = _facts[_currentFactIndex];
    }

    private void PreviousFact()
    {
        if (_currentFactIndex > 0)
        {
            _currentFactIndex--;
            FactText = _facts[_currentFactIndex];
        }
    }

    public async Task ShowOverlaySequenceAsync()
    {
        var blackScreen = new BlackScreenWindow();
        blackScreen.Show();

        await Task.Delay(2000);

        blackScreen.Close();

        var imageOverlay = new ImageOverlayWindow();
        imageOverlay.Show();
    }


    public void SetSleepingAvatar()
    {
        AvatarGif = new Uri(
            "pack://application:,,,/UI/Assets/SpriteSheet/sleepy_samsa.gif",
            UriKind.Absolute);
    }

    public void SetAwakeAvatar()
    {
        AvatarGif = new Uri(
            "pack://application:,,,/UI/Assets/SpriteSheet/gregor_samsa_sprite-1.gif",
            UriKind.Absolute);
    }
}
