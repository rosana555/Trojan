using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Trojan.Core.Base;
using Trojan.Core.Commands;
using Trojan.Core.Models;
using Trojan.Data.DataBase;
using Trojan.Services.Logger;

namespace Trojan.UI.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private readonly DispatcherTimer _autosaveTimer;
        public static MainViewModel? Instance { get; private set; }
        private bool _hasPendingChanges;
        private Note? _lastDeletedNote;
        public bool CanUndoDelete => _lastDeletedNote is not null;
        private Note? _pendingNote;

        // =========================
        // NOTES
        // =========================

        private ObservableCollection<Note> _notes = new();

        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // =========================
        // CALENDAR
        // =========================

        private ObservableCollection<CalendarEvent> _calendarEvents = new();

        public ObservableCollection<CalendarEvent> CalendarEvents
        {
            get => _calendarEvents;
            set => SetProperty(ref _calendarEvents, value);
        }

        // =========================
        // JOKES
        // =========================

        private ObservableCollection<Joke> _jokes = new();

        public ObservableCollection<Joke> Jokes
        {
            get => _jokes;
            set => SetProperty(ref _jokes, value);
        }

        // =========================
        // GALLERY
        // =========================

        public GalleryViewModel Gallery { get; }

        // =========================
        // SELECTED NOTE
        // =========================

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

                if (_hasPendingChanges &&
                    _pendingNote is not null &&
                    ReferenceEquals(_pendingNote, _selectedNote))
                {
                    _autosaveTimer.Stop();

                    DataBaseUtil.SaveNote(_pendingNote);

                    _hasPendingChanges = false;

                    _pendingNote = null;
                }

                if (_selectedNote is not null)
                {
                    _selectedNote.PropertyChanged -=
                        OnSelectedNotePropertyChanged;
                }

                SetProperty(ref _selectedNote, value);

                if (_selectedNote is not null)
                {
                    _selectedNote.PropertyChanged +=
                        OnSelectedNotePropertyChanged;
                }

                _saveNoteCommand.RaiseCanExecuteChanged();
                _deleteNoteCommand.RaiseCanExecuteChanged();
                _togglePinCommand.RaiseCanExecuteChanged();
            }
        }

        // =========================
        // COMMANDS
        // =========================

        private readonly RelayCommand _createNoteCommand;

        public RelayCommand CreateNoteCommand =>
            _createNoteCommand;

        private readonly RelayCommand _saveNoteCommand;

        public RelayCommand SaveNoteCommand =>
            _saveNoteCommand;

        private readonly RelayCommand _deleteNoteCommand;

        public RelayCommand DeleteNoteCommand =>
            _deleteNoteCommand;

        private readonly RelayCommand _togglePinCommand;

        public RelayCommand TogglePinCommand =>
            _togglePinCommand;

        private readonly RelayCommand _undoDeleteCommand;

        public RelayCommand UndoDeleteCommand =>
            _undoDeleteCommand;

        private readonly RelayCommand<Note> _togglePinForNoteCommand;

        public RelayCommand<Note> TogglePinForNoteCommand =>
            _togglePinForNoteCommand;

        // =========================
        // CONSTRUCTOR
        // =========================

        public MainViewModel()
        {
            Instance = this;

            Gallery = new GalleryViewModel();

            _autosaveTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(800)
            };

            _autosaveTimer.Tick += OnAutosaveTick;

            _createNoteCommand =
                new RelayCommand(CreateNote);

            _saveNoteCommand =
                new RelayCommand(
                    SaveCurrentNote,
                    () => SelectedNote is not null);

            _deleteNoteCommand =
                new RelayCommand(
                    DeleteSelectedNote,
                    () => SelectedNote is not null);

            _togglePinCommand =
                new RelayCommand(
                    ToggleSelectedNotePin,
                    () => SelectedNote is not null);
            _undoDeleteCommand =
                new RelayCommand(
                    UndoDelete,
                    () => _lastDeletedNote is not null);

            _togglePinForNoteCommand =
                new RelayCommand<Note>(TogglePinForNote);

            LoadData();
        }

        // =========================
        // LOAD DATA
        // =========================

        private void LoadData()
        {
            try
            {
                Notes = new ObservableCollection<Note>(
                    DataBaseUtil.GetNotes());

                Jokes = new ObservableCollection<Joke>(
                    DataBaseUtil.GetJokes());

                CalendarEvents =
                    new ObservableCollection<CalendarEvent>(
                        DataBaseUtil.GetCalendarEvents());

                SelectedNote = Notes.FirstOrDefault();
            }
            catch (Exception ex)
            {

                Notes = new ObservableCollection<Note>();
                Jokes = new ObservableCollection<Joke>();
                CalendarEvents = new ObservableCollection<CalendarEvent>();
            }
        }

        // =========================
        // NOTES
        // =========================

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

        public void AddCalendarEvent(
            CalendarEvent newCalendarEvent)
        {
            DataBaseUtil.AddCalendarEvent(
                newCalendarEvent);

            CalendarEvents.Add(newCalendarEvent);
        }

        public void MarkNoteDirty()
        {
            if (SelectedNote is null)
            {
                return;
            }

            _hasPendingChanges = true;

            _pendingNote = SelectedNote;

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

            if (string.IsNullOrWhiteSpace(SelectedNote.Title))
            {
                MessageBox.Show(
                    "Naslov beležke ne sme biti prazen.",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            SaveNoteInternal(SelectedNote);
        }

        private void SaveNoteInternal(Note note)
        {
            try
            {
                DataBaseUtil.SaveNote(note);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Napaka pri shranjevanju beležke.",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

            _hasPendingChanges = false;

            _pendingNote = null;

            SortNotes();
        }
        private void DeleteSelectedNote()
        {
            if (SelectedNote is null)
            {
                return;
            }

            _lastDeletedNote = SelectedNote;

            Notes.Remove(SelectedNote);

            SelectedNote = Notes.FirstOrDefault();

            _undoDeleteCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanUndoDelete));
        }
        public void CommitPendingDelete()
        {
            if (_lastDeletedNote is null)
            {
                return;
            }

            DataBaseUtil.DeleteNote(_lastDeletedNote.Id);

            _lastDeletedNote = null;
        }
        private void UndoDelete()
        {
            if (_lastDeletedNote is null)
            {
                return;
            }

            Notes.Insert(0, _lastDeletedNote);

            SelectedNote = _lastDeletedNote;

            _lastDeletedNote = null;

            SortNotes();

            _undoDeleteCommand.RaiseCanExecuteChanged();

            OnPropertyChanged(nameof(CanUndoDelete));
        }
        private void ToggleSelectedNotePin()
        {
            if (SelectedNote is null)
            {
                return;
            }

            SelectedNote.IsPinned =
                !SelectedNote.IsPinned;

            DataBaseUtil.SaveNote(SelectedNote);

            SortNotes();
        }

        private void TogglePinForNote(Note? note)
        {
            if (note is null)
            {
                return;
            }

            note.IsPinned = !note.IsPinned;

            try
            {
                DataBaseUtil.SaveNote(note);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Napaka pri shranjevanju beležke.",
                    "Napaka",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

            }

            SortNotes();
        }

        private void OnSelectedNotePropertyChanged(
            object? sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(Note.Title))
            {
                MarkNoteDirty();
            }
        }

        private void OnAutosaveTick(
            object? sender,
            EventArgs e)
        {
            _autosaveTimer.Stop();

            if (_hasPendingChanges &&
                _pendingNote is not null)
            {
                SaveNoteInternal(_pendingNote);
            }
        }

        private void SortNotes()
        {
            var selectedId = SelectedNote?.Id ?? 0;

            var sorted = Notes
                .OrderByDescending(n => n.IsPinned)
                .ThenByDescending(
                    n => n.EditedAt ?? n.CreatedAt)
                .ToList();

            Notes = new ObservableCollection<Note>(
                sorted);

            SelectedNote =
                Notes.FirstOrDefault(
                    n => n.Id == selectedId)
                ?? Notes.FirstOrDefault();
        }
    }
}