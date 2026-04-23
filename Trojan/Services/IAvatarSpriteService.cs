using System.ComponentModel;
using System.Windows.Media;

namespace Trojan.Services;

public interface IAvatarSpriteService : INotifyPropertyChanged
{
    int CurrentFrameIndex { get; }
    ImageSource CurrentFrameImage { get; }

    void SetCurrentFrame(int index);
    int GetCurrentFrame();
}
