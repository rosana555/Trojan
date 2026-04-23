using System.Windows.Input;
using Trojan.Commands;
using Trojan.Services;
using System.Windows.Media;

namespace Trojan.ViewModels;

public sealed class HelperOverlayViewModel : ObservableObject
{
    private bool _areBubblesVisible;
    private readonly IAvatarSpriteService _avatarSpriteService;

    public bool AreBubblesVisible
    {
        get => _areBubblesVisible;
        set => SetProperty(ref _areBubblesVisible, value);
    }

    public ImageSource AvatarImage => _avatarSpriteService.CurrentFrameImage;
    public int AvatarFrameIndex
    {
        get => _avatarSpriteService.GetCurrentFrame();
        set => _avatarSpriteService.SetCurrentFrame(value);
    }

    public ICommand ToggleBubblesCommand { get; }

    public HelperOverlayViewModel(IAvatarSpriteService avatarSpriteService)
    {
        _avatarSpriteService = avatarSpriteService;
        _avatarSpriteService.PropertyChanged += OnAvatarSpriteServicePropertyChanged;
        ToggleBubblesCommand = new RelayCommand(ToggleBubbles);
    }

    private void ToggleBubbles()
    {
        AreBubblesVisible = !AreBubblesVisible;
    }

    private void OnAvatarSpriteServicePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IAvatarSpriteService.CurrentFrameImage))
        {
            OnPropertyChanged(nameof(AvatarImage));
        }

        if (e.PropertyName == nameof(IAvatarSpriteService.CurrentFrameIndex))
        {
            OnPropertyChanged(nameof(AvatarFrameIndex));
        }
    }
}
