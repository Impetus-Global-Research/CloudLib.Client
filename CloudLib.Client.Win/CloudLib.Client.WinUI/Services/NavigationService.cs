using System;
using CloudLib.Client.WinUI.Core.Helpers;
using CloudLib.Client.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;

namespace CloudLib.Client.WinUI.Services;

public static class NavigationService
{
    private static Frame _frame;
    private static object? _lastParameters;

    public static Frame Frame
    {
        get => _frame;
        set
        {
            UnregisterFrameEvents();
            _frame = value;
            RegisterFrameEvents();
        }
    }

    private static void UnregisterFrameEvents()
    {
        if (_frame is null) return;

        _frame.Navigated -= Frame_OnNavigated;
        _frame.NavigationFailed -= Frame_OnNavigationFailed;
    }

    private static void RegisterFrameEvents()
    {
        if (_frame is null) return;

        _frame.Navigated += Frame_OnNavigated;
        _frame.NavigationFailed += Frame_OnNavigationFailed;
    }

    public static bool CanGoBack => Frame?.CanGoBack ?? false;
    public static bool CanGoForward => Frame?.CanGoForward ?? false;

    public static bool GoBack()
    {
        if (CanGoBack)
        {
            Frame?.GoBack();
            return true;
        }

        return false;
    }

    public static bool GoForward()
    {
        if (CanGoForward)
        {
            Frame?.GoForward();
            return true;
        }

        return false;
    }

    public static bool Navigate(Type targetType,
        object? parameters = null,
        NavigationTransitionInfo? infoOverride = default)
    {
        if (targetType is null || !targetType.IsAssignableTo(typeof(Page)))
        {
            throw new ArgumentException(
                $"Can not navigate to target with type `{targetType}` because the value must inherit from `Microsoft.Xaml.UI.Controls.Page`");
        }

        if (IsDifferentPage() || ParametersAreNew())
        {
            var navResult = Frame?.Navigate(targetType, parameters, infoOverride);
            switch (navResult)
            {
                case null:
                    return false;
                case true:
                    _lastParameters = parameters!;
                    break;
            }

            return ((bool)navResult)!;
        }

        return false;

        bool IsDifferentPage()
        {
            var frameContent = Frame?.Content;

            return frameContent is null || frameContent.GetType() != targetType;
        }

        bool ParametersAreNew()
            => parameters is not null && (!_lastParameters?.Equals(parameters) ?? true);
    }

    public static event NavigationFailedEventHandler NavigationFailed;
    public static event NavigatedEventHandler Navigated;

    private static void Frame_OnNavigated(object sender, NavigationEventArgs args) => Navigated?.Invoke(sender, args);
    private static void Frame_OnNavigationFailed(object sender, NavigationFailedEventArgs args) => NavigationFailed?.Invoke(sender, args);
}