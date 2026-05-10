namespace Trojan.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Trojan.Commands;
using Trojan.DataBase;
using Trojan.Models;

public class MainViewModel : ObservableObject
{
    private readonly DispatcherTimer _autosaveTimer;
    private bool _hasPendingChanges;

    private ObservableCollection<Note> _notes = new();
    public ObservableCollection<Note> Notes
    {
        get => _notes;
        set => SetProperty(ref _notes, value);
    }

    private ObservableCollection<CalendarEvent> _calendarEvents = new();
    public ObservableCollection<CalendarEvent> CalendarEvents
    {
        get => _calendarEvents;
        set => SetProperty(ref _calendarEvents, value);
    }

    private ObservableCollection<GalleryItem> _galleryItems = new();
    public ObservableCollection<GalleryItem> GalleryItems
    {
        get => _galleryItems;
        set => SetProperty(ref _galleryItems, value);
    }

    private ObservableCollection<Joke> _jokes = new();
    public ObservableCollection<Joke> Jokes
    {
        get => _jokes;
        set => SetProperty(ref _jokes, value);
    }

    private Note? _selectedNote;
    public Note? SelectedNote
    {
        get => _selectedNote;
        set
        {
            if (ReferenceEquals(_selectedNote, value))
            {
                return;
            }

            if (_selectedNote is not null)
            {
                _selectedNote.PropertyChanged -= OnSelectedNotePropertyChanged;
            }

            SetProperty(ref _selectedNote, value);

            if (_selectedNote is not null)
            {
                _selectedNote.PropertyChanged += OnSelectedNotePropertyChanged;
            }

            _saveNoteCommand?.RaiseCanExecuteChanged();
            _deleteNoteCommand?.RaiseCanExecuteChanged();
        }
    }

    private readonly RelayCommand _createNoteCommand;
    public RelayCommand CreateNoteCommand => _createNoteCommand;

    private readonly RelayCommand _saveNoteCommand;
    public RelayCommand SaveNoteCommand => _saveNoteCommand;

    private readonly RelayCommand _deleteNoteCommand;
    public RelayCommand DeleteNoteCommand => _deleteNoteCommand;

    public MainViewModel()
    {
        _autosaveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(800)
        };
        _autosaveTimer.Tick += OnAutosaveTick;

        _createNoteCommand = new RelayCommand(CreateNote);
        _saveNoteCommand = new RelayCommand(SaveCurrentNote, () => SelectedNote is not null);
        _deleteNoteCommand = new RelayCommand(DeleteSelectedNote, () => SelectedNote is not null);

        LoadData();
    }

    //loads data from db to UI
    private void LoadData()
    {
        Notes = new ObservableCollection<Note>(DataBaseUtil.GetNotes());
        Jokes = new ObservableCollection<Joke>(DataBaseUtil.GetJokes());    
        CalendarEvents = new ObservableCollection<CalendarEvent>(DataBaseUtil.GetCalendarEvents());
        GalleryItems = new ObservableCollection<GalleryItem>(DataBaseUtil.GetGalleryItems());
        SelectedNote = Notes.FirstOrDefault();
    }

    // Adding functions (adds to observableCollection and db) 
    public void AddNote(Note newNote)
    {
        DataBaseUtil.AddNote(newNote);
        Notes.Insert(0, newNote);
        SelectedNote = newNote;
    }

    public void AddJoke(Joke newJoke)
    {
        DataBaseUtil.AddJoke(newJoke);
        Jokes.Add(newJoke);
    }
    public void AddCalendarEvent(CalendarEvent newCalendarEvent)
    {
        DataBaseUtil.AddCalendarEvent(newCalendarEvent);
        CalendarEvents.Add(newCalendarEvent);
    }
    public void AddGalleryItem(GalleryItem newGalleryItem)
    {
        DataBaseUtil.AddGalleryItem(newGalleryItem);
        GalleryItems.Add(newGalleryItem);
    }

    public void MarkNoteDirty()
    {
        if (SelectedNote is null)
        {
            return;
        }

        _hasPendingChanges = true;
        _autosaveTimer.Stop();
        _autosaveTimer.Start();
    }

    public void SelectNote(Note? note)
    {
        SelectedNote = note;
    }

    private void CreateNote()
    {
        var note = new Note
        {
            Title = "Nova beležka",
            Content = string.Empty
        };

        AddNote(note);
    }

    private void SaveCurrentNote()
    {
        if (SelectedNote is null)
        {
            return;
        }

        DataBaseUtil.SaveNote(SelectedNote);
        _hasPendingChanges = false;
        SortNotes();
        _saveNoteCommand.RaiseCanExecuteChanged();
        _deleteNoteCommand.RaiseCanExecuteChanged();
    }

    private void DeleteSelectedNote()
    {
        if (SelectedNote is null)
        {
            return;
        }

        var deletedId = SelectedNote.Id;
        DataBaseUtil.DeleteNote(deletedId);
        Notes.Remove(SelectedNote);
        SelectedNote = Notes.FirstOrDefault();
    }

    private void OnSelectedNotePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(Note.Title))
        {
            MarkNoteDirty();
        }
    }

    private void OnAutosaveTick(object? sender, EventArgs e)
    {
        _autosaveTimer.Stop();
        if (_hasPendingChanges)
        {
            SaveCurrentNote();
        }
    }

    private void SortNotes()
    {
        var selectedId = SelectedNote?.Id ?? 0;
        var sorted = Notes
            .OrderByDescending(n => n.EditedAt ?? n.CreatedAt)
            .ToList();

        Notes = new ObservableCollection<Note>(sorted);
        SelectedNote = Notes.FirstOrDefault(n => n.Id == selectedId) ?? Notes.FirstOrDefault();
    }
}