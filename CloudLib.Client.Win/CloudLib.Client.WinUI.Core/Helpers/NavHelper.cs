using System;
using Microsoft.UI.Xaml;

namespace CloudLib.Client.WinUI.Core.Helpers
{
    public class NavHelper
    {
        public static readonly DependencyProperty NavigateToProperty = DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavHelper), new PropertyMetadata(default(Type)));

        public static Type GetNavigateTo(UIElement element)
        {
            return (Type)element.GetValue(NavigateToProperty);
        }

        public static void SetNavigateTo(UIElement element, Type value)
        {
            element.SetValue(NavigateToProperty, value);
        }
    }
}
