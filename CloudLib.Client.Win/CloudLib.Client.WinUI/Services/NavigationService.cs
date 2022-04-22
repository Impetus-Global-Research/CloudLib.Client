using System;
using CloudLib.Client.WinUI.Core.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Services;

internal class NavigationService
{
    public static Frame Frame { get; set; }

    public static void Navigate(UIElement navElement, object? arguments)
    {
        var targetType = NavHelper.GetNavigateTo(navElement);

        var pageInstance = Activator.CreateInstance(targetType, arguments);

        if (pageInstance is not null)
        {
            Frame.Content = pageInstance;
        }
    }

    public static void Navigate(Type navElement, object? arguments)
    {
        if (navElement is UIElement)
        {
            Navigate(navElement, arguments);
        }
    }
}