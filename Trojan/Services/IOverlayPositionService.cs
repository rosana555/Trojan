using System.Windows;

namespace Trojan.Services;

public interface IOverlayPositionService
{
    void PositionBottomRight(Window window, double edgeMargin);
}
