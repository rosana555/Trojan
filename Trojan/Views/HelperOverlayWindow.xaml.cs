using Microsoft.Win32;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Trojan.Services;
using Trojan.Utilities;
using Trojan.ViewModels;

namespace Trojan.Views;

public partial class HelperOverlayWindow : Window
{
    private const double EdgeMargin = 24;
    private readonly IOverlayPositionService _overlayPositionService;
    private readonly HelperOverlayViewModel _viewModel;
    private bool _isUpdatingDocument;

    public HelperOverlayWindow()
    {
        InitializeComponent();

        _overlayPositionService = new OverlayPositionService();
        _viewModel = new HelperOverlayViewModel();

        DataContext = _viewModel;
        _viewModel.Main.PropertyChanged += OnMainViewModelPropertyChanged;

        Loaded += OnLoaded;
        SourceInitialized += (_, _) => RepositionToBottomRight();
        SizeChanged += (_, _) => RepositionToBottomRight();
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RepositionToBottomRight();
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
        LoadSelectedNoteIntoEditor();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _viewModel.Main.PropertyChanged -= OnMainViewModelPropertyChanged;
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(RepositionToBottomRight);
    }

    private void RepositionToBottomRight()
    {
        _overlayPositionService.PositionBottomRight(this, EdgeMargin);
    }

    private void OnMainViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(MainViewModel.SelectedNote))
        {
            LoadSelectedNoteIntoEditor();
        }
    }

    private void LoadSelectedNoteIntoEditor()
    {
        _isUpdatingDocument = true;
        NoteContentRichTextBox.Document.Blocks.Clear();

        var selectedNote = _viewModel.Main.SelectedNote;
        if (selectedNote is null)
        {
            _isUpdatingDocument = false;
            return;
        }

        var parsed = NoteContentSerializer.DeserializeForEditor(selectedNote.Content);
        if (parsed.IsRtf)
        {
            if (!string.IsNullOrWhiteSpace(parsed.Content))
            {
                try
                {
                    var range = new TextRange(NoteContentRichTextBox.Document.ContentStart, NoteContentRichTextBox.Document.ContentEnd);
                    using var stream = new MemoryStream(Encoding.UTF8.GetBytes(parsed.Content));
                    range.Load(stream, DataFormats.Rtf);
                }
                catch
                {
                    NoteContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run("")));
                }
            }
        }
        else
        {
            NoteContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run(parsed.Content)));
        }

        _isUpdatingDocument = false;
    }

    private void NoteContentRichTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isUpdatingDocument || _viewModel.Main.SelectedNote is null)
        {
            return;
        }

        _viewModel.Main.SelectedNote.Content = NoteContentSerializer.SerializeRtf(GetCurrentEditorRtf());
        _viewModel.Main.MarkNoteDirty();
    }

    private string GetCurrentEditorRtf()
    {
        var range = new TextRange(NoteContentRichTextBox.Document.ContentStart, NoteContentRichTextBox.Document.ContentEnd);
        using var stream = new MemoryStream();
        range.Save(stream, DataFormats.Rtf);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    private void BoldButton_Click(object sender, RoutedEventArgs e)
    {
        EditingCommands.ToggleBold.Execute(null, NoteContentRichTextBox);
    }

    private void BulletListButton_Click(object sender, RoutedEventArgs e)
    {
        EditingCommands.ToggleBullets.Execute(null, NoteContentRichTextBox);
    }

    private void NotesHistoryListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        _viewModel.IsHistoryVisible = false;
        _viewModel.IsNoteVisible = true;
    }
}
