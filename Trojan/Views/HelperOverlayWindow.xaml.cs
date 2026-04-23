using Microsoft.Win32;
using System.Windows;
using Trojan.Services;
using Trojan.ViewModels;

namespace Trojan.Views;

public partial class HelperOverlayWindow : Window
{
    private const double EdgeMargin = 24;
    private readonly IOverlayPositionService _overlayPositionService;
    private readonly IAvatarSpriteService _avatarSpriteService;

    public HelperOverlayWindow()
    {
        InitializeComponent();
        _avatarSpriteService = new AvatarSpriteService();
        _overlayPositionService = new OverlayPositionService();
        DataContext = new HelperOverlayViewModel(_avatarSpriteService);

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
