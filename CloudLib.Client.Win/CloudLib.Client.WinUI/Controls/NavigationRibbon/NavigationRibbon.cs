/*
 * Ported from the UWP CommunityToolkit repo https://github.com/yoshiask/WindowsCommunityToolkit/blob/71efaa753e1a5427bbde36faecafc4379f1c018f/Microsoft.Toolkit.Uwp.UI.Controls/TabbedCommandBar/TabbedCommandBar.cs
 * for use with WinUI3 projects. Mostly a direct migration of the code with very few changes.
 * Original Repository Contributors:
 *   - https://github.com/yoshiask
 *   - https://github.com/dpaulino
 *   - https://github.com/michael-hawker
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

using Windows.Foundation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudLib.Client.WinUI.Controls
{
    [ContentProperty(Name = nameof(MenuItems))]
    [TemplatePart(Name = "PART_NavigationRibbonContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_NavigationRibbonContentBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_TabChangedStoryboard", Type = typeof(Storyboard))]
    public class NavigationRibbon : NavigationView
    {
        private ContentControl? _ribbonContent = null;
        private Border? _ribbonContentBorder = null;
        private Storyboard? _tabChangedStoryboard = null;

        /// <summary>
        /// The last <see cref="NavigationRibbonItem"/> which was selected. <code>null</code> by default
        /// </summary>
        private NavigationRibbonItem? _previousSelectedItem = null;
        private long _visibilityChangedToken;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationRibbon"/> class.
        /// </summary>
        public NavigationRibbon()
        {
            DefaultStyleKey = typeof(NavigationRibbon);
            DefaultStyleResourceUri = new Uri("ms-appx:///Themes/Generic.xaml");
            SelectionChanged += NavigationRibbon_SelectedItemChanged;
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_ribbonContent is not null)
            {
                _ribbonContent.Content = null;
            }

            _ribbonContent = GetTemplateChild("PART_RibbonContent") as ContentControl;
            _ribbonContentBorder = GetTemplateChild("PART_RibbonContentBorder") as Border;
            _tabChangedStoryboard = GetTemplateChild("PART_TabChangedStoryboard") as Storyboard;

            SelectedItem = MenuItems.FirstOrDefault();
        }


        private void NavigationRibbon_SelectedItemChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (sender.SelectedItem is not NavigationRibbonItem item || item.Visibility == Visibility.Collapsed)
            {
                // Handles the state where the Visibility is null, but the item is still selectable
                sender.SelectedItem = sender.MenuItems.FirstOrDefault();
                return;
            }

            // Remove the visibility PropertyChanged handler from previously selected item
            if (_previousSelectedItem is not null)
            {
                _previousSelectedItem.UnregisterPropertyChangedCallback(NavigationRibbonItem.VisibilityProperty, _visibilityChangedToken);
            }

            // Register new callback for the currently selected item
            _previousSelectedItem = item;
            _visibilityChangedToken = _previousSelectedItem.RegisterPropertyChangedCallback(NavigationRibbonItem.VisibilityProperty, SelectedItemVisibilityChanged);

            // Set the ribbon background and start the transition animation
            _tabChangedStoryboard?.Begin();
        }

        private void SelectedItemVisibilityChanged(DependencyObject sender, DependencyProperty dp)
        {
            if (sender.GetValue(dp) is Visibility vis && vis is Visibility.Collapsed)
            {
                /*
                 * TASK: Possible bug here (will cause WinUI to throw an exception if run when the tabs overflow) not positive
                 * if the bug still exists within WinUI3 or not.
                 */
                SelectedItem = MenuItems.FirstOrDefault();
            }
        }
    }
}
