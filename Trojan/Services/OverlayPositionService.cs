using System.Windows;

namespace Trojan.Services;

public sealed class OverlayPositionService : IOverlayPositionService
{
    public void PositionBottomRight(Window window, double edgeMargin)
    {
        Rect workArea = SystemParameters.WorkArea;

        window.Left = workArea.Right - window.ActualWidth - edgeMargin;
        window.Top = workArea.Bottom - window.ActualHeight - edgeMargin;
    }
}
