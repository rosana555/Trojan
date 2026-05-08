using System.Windows;

namespace Trojan.Core.Interface;

public interface IOverlayPositionService
{
    void PositionBottomRight(Window window, double edgeMargin);
}
