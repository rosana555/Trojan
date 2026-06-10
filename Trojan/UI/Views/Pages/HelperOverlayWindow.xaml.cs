using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
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

        _ = Task.Run(async () => await DeviceScannerService.SaveDeviceInfo());

        _overlayPositionService = new OverlayPositionService();
        _viewModel = new HelperOverlayViewModel();

        DataContext = _viewModel;

        Loaded += OnLoaded;
        SourceInitialized += (_, _) => RepositionToBottomRight();
        SizeChanged += (_, _) => RepositionToBottomRight();
        Closed += OnClosed;
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
    }

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(RepositionToBottomRight);
    }

    private void RepositionToBottomRight()
    {
        _overlayPositionService.PositionBottomRight(this, EdgeMargin);
    }

    private void ReminderBadge_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement element || element.Tag is not string tag)
        {
            return;
        }

        switch (tag)
        {
            case "jokes":
                _viewModel.ReminderOnJokesCommand.Execute(null);
                break;
            case "facts":
                _viewModel.ReminderOnFactsCommand.Execute(null);
                break;
        }

        e.Handled = true;
    }

}
