using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.WinUI.UI;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Controls
{
    public sealed class NavigationRibbonSelectionChangedEventArgs : EventArgs
    {

        public bool IsSettingsSelected { get; }

        public object SelectedItem { get; }

        public NavigationRibbonItem? SelectedItemContainer { get; }

        public NavigationRibbonSelectionChangedEventArgs(NavigationViewSelectionChangedEventArgs args)
        {
            IsSettingsSelected = args.IsSettingsSelected;
            SelectedItem = args.SelectedItem;

            if (SelectedItem is NavigationRibbonItem selectedRibbonItem)
            {
                SelectedItemContainer = selectedRibbonItem;
            }
            else if (SelectedItem is DependencyObject selected && selected is not NavigationRibbonItem)
            {
                SelectedItemContainer = selected.FindAscendant<NavigationRibbonItem>();
            }
        }
    }
}
