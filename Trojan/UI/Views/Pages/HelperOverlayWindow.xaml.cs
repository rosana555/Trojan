using Microsoft.Win32;
using System.Windows;
using Trojan.Core.Interface;
using Trojan.Services;
using Trojan.Services.Avatar;
using Trojan.Services.Overlay;
using Trojan.UI.ViewModels;

namespace Trojan.UI.Views.Pages;

public partial class HelperOverlayWindow : Window
{
    private const double EdgeMargin = 24;
    private readonly IOverlayPositionService _overlayPositionService;
    private readonly HelperOverlayViewModel _viewModel;

    public HelperOverlayWindow()
    {
        InitializeComponent();

        _overlayPositionService = new OverlayPositionService();
        _viewModel = new HelperOverlayViewModel();

        DataContext = _viewModel;

        Loaded += OnLoaded;
        SourceInitialized += (_, _) => RepositionToBottomRight();
        SizeChanged += (_, _) => RepositionToBottomRight();
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        RepositionToBottomRight();
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(RepositionToBottomRight);
    }

    private void RepositionToBottomRight()
    {
        _overlayPositionService.PositionBottomRight(this, EdgeMargin);
    }

}
