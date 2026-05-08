using System.Windows;
using Trojan.Core.Interface;

namespace Trojan.Services.Overlay;

public sealed class OverlayPositionService : IOverlayPositionService
{
    public void PositionBottomRight(Window window, double edgeMargin)
    {
        Rect workArea = SystemParameters.WorkArea;

        window.Left = workArea.Right - window.ActualWidth - edgeMargin;
        window.Top = workArea.Bottom - window.ActualHeight - edgeMargin;
    }
}
