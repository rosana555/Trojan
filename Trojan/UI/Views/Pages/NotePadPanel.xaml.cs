using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Trojan.UI.ViewModels;

namespace Trojan.UI.Views.Pages;

public partial class NotePadPanel : UserControl
{
    private bool _isUpdatingDocument;
    private MainViewModel? _mainViewModel;
    private Trojan.Core.Models.Note? _loadedNote;

    public NotePadPanel()
    {
        InitializeComponent();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnDataContextChanged;
        NoteContentRichTextBox.PreviewKeyDown += NoteContentRichTextBox_PreviewKeyDown;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        NoteContentRichTextBox.TextChanged -= NoteContentRichTextBox_TextChanged;
        NoteContentRichTextBox.TextChanged += NoteContentRichTextBox_TextChanged;
        HookToMainViewModel();
        LoadSelectedNoteIntoEditor();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        NoteContentRichTextBox.TextChanged -= NoteContentRichTextBox_TextChanged;
        UnhookFromMainViewModel();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is HelperOverlayViewModel oldVm)
        {
            oldVm.Main.PropertyChanged -= OnMainViewModelPropertyChanged;
        }

        HookToMainViewModel();
        LoadSelectedNoteIntoEditor();
    }

    private void HookToMainViewModel()
    {
        if (DataContext is not HelperOverlayViewModel vm)
        {
            return;
        }

        if (ReferenceEquals(_mainViewModel, vm.Main))
        {
            return;
        }

        UnhookFromMainViewModel();
        _mainViewModel = vm.Main;
        _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
    }

    private void UnhookFromMainViewModel()
    {
        if (_mainViewModel is null)
        {
            return;
        }

        _mainViewModel.PropertyChanged -= OnMainViewModelPropertyChanged;
        _mainViewModel = null;
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
        if (DataContext is not HelperOverlayViewModel vm)
        {
            return;
        }

        _isUpdatingDocument = true;
        NoteContentRichTextBox.Document.Blocks.Clear();

        var selectedNote = vm.Main.SelectedNote;
        _loadedNote = selectedNote;
        if (selectedNote is null)
        {
            _isUpdatingDocument = false;
            return;
        }

      
        if (!string.IsNullOrWhiteSpace(selectedNote.Content))
        {
            try
            {
                var range = new TextRange(NoteContentRichTextBox.Document.ContentStart, NoteContentRichTextBox.Document.ContentEnd);
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(selectedNote.Content));
                range.Load(stream, DataFormats.Rtf);
            }
            catch
            {
                NoteContentRichTextBox.Document.Blocks.Add(new Paragraph(new Run("")));
            }
        }

        _isUpdatingDocument = false;
    }

    private const int MaxNoteLength = 50000;

    private void NoteContentRichTextBox_TextChanged(
        object sender,
        TextChangedEventArgs e)
    {
        TextRange range = new TextRange(
            NoteContentRichTextBox.Document.ContentStart,
            NoteContentRichTextBox.Document.ContentEnd);

        if (range.Text.Length > MaxNoteLength)
        {
            MessageBox.Show(
                $"Najveèja dovoljena dolžina beležke je {MaxNoteLength:N0} znakov.",
                "Omejitev dosežena",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            range.Text = range.Text.Substring(0, MaxNoteLength);
        }
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
    private void NumberedListButton_Click(object sender, RoutedEventArgs e)
    {
        EditingCommands.ToggleNumbering.Execute(null, NoteContentRichTextBox);
    }
    private void UnderlineButton_Clic1k(object sender, RoutedEventArgs e)
    {
        EditingCommands.ToggleUnderline.Execute(null, NoteContentRichTextBox);
    }
    

    private void NotesHistoryListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is not HelperOverlayViewModel vm)
        {
            return;
        }

        vm.IsHistoryVisible = false;
        vm.IsNoteVisible = true;
    }

    private static T? FindAncestor<T>(TextPointer pointer) where T : DependencyObject
    {
        var current = pointer.Parent as DependencyObject;

        while (current != null)
        {
            if (current is T t)
            {
                return t;
            }

            current = current switch
            {
                TextElement te => te.Parent,
                _ => LogicalTreeHelper.GetParent(current)
            };
        }

        return null;
    }

    private void NoteContentRichTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        var pointer = NoteContentRichTextBox.CaretPosition;

        var table =
            FindAncestor<Table>(pointer) ??
            FindAncestor<Table>(NoteContentRichTextBox.Selection.Start) ??
            FindAncestor<Table>(NoteContentRichTextBox.Selection.End);

        if (e.Key == Key.Enter && table != null)
        {
            e.Handled = true;

            var parent = table.Parent;

            var paragraph = new Paragraph(new Run(""))
            {
                Margin = new Thickness(0)
            };

            if (parent is Section section)
            {
                section.Blocks.InsertAfter(table, paragraph);
            }
            else if (parent is FlowDocument doc)
            {
                doc.Blocks.InsertAfter(table, paragraph);
            }

            NoteContentRichTextBox.CaretPosition = paragraph.ContentStart;
            NoteContentRichTextBox.Focus();

            return;
        }

        if ((e.Key == Key.Delete || e.Key == Key.Back) && table != null)
        {
            e.Handled = true;

            DeleteTable(table);

            NoteContentRichTextBox.Focus();

            return;
        }
    }
    private void ItalicButton_Click(object sender, RoutedEventArgs e)
    {
        EditingCommands.ToggleItalic.Execute(null, NoteContentRichTextBox);
    }
    // ---------------------------
    // COLORS 
    // ---------------------------

    private void SetColor(string hex)
    {
        var range = new TextRange(NoteContentRichTextBox.Selection.Start,
                                  NoteContentRichTextBox.Selection.End);

        range.ApplyPropertyValue(TextElement.ForegroundProperty,
            new SolidColorBrush((Color)ColorConverter.ConvertFromString(hex)));
    }

    private void BlackColor_Click(object s, RoutedEventArgs e) => SetColor("#000000");
    private void BlueColor_Click(object s, RoutedEventArgs e) => SetColor("#4A90E2");
    private void GreenColor_Click(object s, RoutedEventArgs e) => SetColor("#7ED321");
    private void OrangeColor_Click(object s, RoutedEventArgs e) => SetColor("#F5A623");
    private void RedColor_Click(object s, RoutedEventArgs e) => SetColor("#D0021B");

    private void TextColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox combo)
        {
            return;
        }

        if (combo.SelectedItem is ComboBoxItem { Tag: string hex } && !string.IsNullOrWhiteSpace(hex))
        {
            SetColor(hex);

            combo.SelectedIndex = 0;
            NoteContentRichTextBox.Focus();
            return;
        }
    }

    // ---------------------------
    // TABLE
    // ---------------------------

    private void TableSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ComboBox combo)
        {
            return;
        }

        if (combo.SelectedItem is ComboBoxItem { Tag: string sizeTag } && !string.IsNullOrWhiteSpace(sizeTag))
        {
            var parts = sizeTag.Split('x');
            if (parts.Length == 2 && int.TryParse(parts[0], out var rows) && int.TryParse(parts[1], out var cols))
            {
                InsertTable(rows, cols);

                combo.SelectedIndex = 0;
                NoteContentRichTextBox.Focus();
                return;
            }
        }
    }

    private void InsertTable(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            return;
        }

        var borderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8F8791"));

        var table = new Table
        {
            CellSpacing = 0,
            BorderBrush = borderBrush,
            BorderThickness = new Thickness(1)

        };

        for (var c = 0; c < cols; c++)
        {
            table.Columns.Add(new TableColumn());
        }

        var group = new TableRowGroup();

        for (var r = 0; r < rows; r++)
        {
            var row = new TableRow();

            for (var c = 0; c < cols; c++)
            {
                row.Cells.Add(new TableCell(new Paragraph(new Run("")))
                {
                    BorderBrush = borderBrush,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(6)

                });
            }

            group.Rows.Add(row);
        }

        table.RowGroups.Add(group);

        NoteContentRichTextBox.Document.Blocks.Add(table);
    }
    private void DeleteTable(Table table)
    {
        switch (table.Parent)
        {
            case FlowDocument doc:
                doc.Blocks.Remove(table);
                break;

            case Section section:
                section.Blocks.Remove(table);
                break;
        }
    }
    private static Table? FindAncestorTable(TextPointer? pointer)
    {
        if (pointer?.Parent is not DependencyObject current)
        {
            return null;
        }

        while (current is not null)
        {
            if (current is Table table)
            {
                return table;
            }

            current = current switch
            {
                TextElement te => te.Parent,
                _ => LogicalTreeHelper.GetParent(current)
            };
        }

        return null;
    }

}
