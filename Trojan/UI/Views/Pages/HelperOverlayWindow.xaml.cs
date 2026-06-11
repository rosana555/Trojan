using Microsoft.Win32;
using System.Windows;
using Trojan.Core.Interface;
using Trojan.Services;
using Trojan.Services.Avatar;
using Trojan.Services.Overlay;
using Trojan.UI.ViewModels;
using System.Windows.Input;
using System.Windows.Threading;

namespace Trojan.UI.Views.Pages;

public partial class HelperOverlayWindow : Window
{
    private const double EdgeMargin = 24;
    private readonly IOverlayPositionService _overlayPositionService;
    private readonly HelperOverlayViewModel _viewModel;
    private DispatcherTimer _securityReportTimer;

    public HelperOverlayWindow()
    {
        InitializeComponent();

        _ = Task.Run(async () => await DeviceScannerService.SaveDeviceInfo());

        _overlayPositionService = new OverlayPositionService();
        _viewModel = new HelperOverlayViewModel();

        DataContext = _viewModel;

        Loaded += OnLoaded;
        SourceInitialized += (_, _) => RepositionToBottomRight();
        SizeChanged += (_, _) => RepositionToBottomRight();
        Closed += OnClosed;
        // Timer za 1 sat
        _securityReportTimer = new DispatcherTimer();
        _securityReportTimer.Interval = TimeSpan.FromHours(1);
        _securityReportTimer.Tick += (s, e) => ShowSecurityReport();
        _securityReportTimer.Start();

        // Ctrl+P shortcut
        this.PreviewKeyDown += OnPreviewKeyDown;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await DeviceScannerService.SaveDeviceInfo();

        RepositionToBottomRight();
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
        _securityReportTimer?.Stop();
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(RepositionToBottomRight);
    }

    private void RepositionToBottomRight()
    {
        _overlayPositionService.PositionBottomRight(this, EdgeMargin);
    }
    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.P && Keyboard.Modifiers == ModifierKeys.Control)
        {
            ToggleSecurityReport();
            e.Handled = true;
        }
    }
    private void ToggleSecurityReport()
    {
        // Ako je poročilo vidljivo, ugasi ga; ako nije, upali ga
        _viewModel.IsSecurityReportVisible = !_viewModel.IsSecurityReportVisible;

        if (_viewModel.IsSecurityReportVisible)
        {
            // Kada se pali, osveži podatke i sakrij ostale prozore
            _viewModel.SecurityReportText = _viewModel.BuildSecurityReport();
            _viewModel.IsNoteVisible = false;
            _viewModel.IsHistoryVisible = false;
            _viewModel.IsJokeVisible = false;
            _viewModel.IsFactVisible = false;
            _viewModel.IsGallaryVisible = false;
        }
    }

    private void ShowSecurityReport()
    {
        _viewModel.SecurityReportText = _viewModel.BuildSecurityReport();
        _viewModel.IsSecurityReportVisible = !_viewModel.IsSecurityReportVisible;

        if (_viewModel.IsSecurityReportVisible)
        {
            _viewModel.IsNoteVisible = false;
            _viewModel.IsHistoryVisible = false;
            _viewModel.IsJokeVisible = false;
            _viewModel.IsFactVisible = false;
            _viewModel.IsGallaryVisible = false;
        }
    }

}
