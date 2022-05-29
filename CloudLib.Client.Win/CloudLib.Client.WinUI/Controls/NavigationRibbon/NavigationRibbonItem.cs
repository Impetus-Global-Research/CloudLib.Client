/*
 * Ported from the UWP CommunityToolkit repo https://github.com/yoshiask/WindowsCommunityToolkit/blob/71efaa753e1a5427bbde36faecafc4379f1c018f/Microsoft.Toolkit.Uwp.UI.Controls/TabbedCommandBar/TabbedCommandBarItem.cs
 * for use with WinUI3 projects. Mostly a direct migration of the code with very few changes.
 * Original Repository Contributors:
 *   - https://github.com/yoshiask
 */

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Controls
{
    /// <summary>
    /// A <see cref="CommandBar"/> to be displayed in the host <see cref="NavigationRibbon"/>
    /// </summary>
    [TemplatePart(Name = "PrimaryItemsControl", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "MoreButton", Type = typeof(Button))]
    public class NavigationRibbonItem : CommandBar
    {
        private ItemsControl? _primaryItemsControl;
        private Button? _moreButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationRibbonItem"/> class.
        /// </summary>
        public NavigationRibbonItem()
        {
            DefaultStyleKey = typeof(NavigationRibbonItem);
            
        }

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(NavigationRibbonItem), new PropertyMetadata(string.Empty));

        public bool IsContextual
        {
            get { return (bool)GetValue(IsContextualProperty); }
            set { SetValue(IsContextualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContextualProperty =
            DependencyProperty.Register("IsContextual", typeof(bool), typeof(NavigationRibbonItem), new PropertyMetadata(false));



        public HorizontalAlignment OverflowButtonAlignment
        {
            get { return (HorizontalAlignment)GetValue(OverflowButtonAlignmentProperty); }
            set { SetValue(OverflowButtonAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for .  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverflowButtonAlignmentProperty =
            DependencyProperty.Register("OverflowButtonAlignment", typeof(HorizontalAlignment), typeof(NavigationRibbonItem), new PropertyMetadata(HorizontalAlignment.Left));



        public HorizontalAlignment CommandAlignment
        {
            get { return (HorizontalAlignment)GetValue(CommandAlignmentProperty); }
            set { SetValue(CommandAlignmentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CommandAlignment.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandAlignmentProperty =
            DependencyProperty.Register("CommandAlignment", typeof(HorizontalAlignment), typeof(NavigationRibbonItem), new PropertyMetadata(HorizontalAlignment.Stretch));



        public int SmallerPaneToggleButtonWidth
        {
            get { return (int)GetValue(SmallerPaneToggleButtonWidthProperty); }
            set { SetValue(SmallerPaneToggleButtonWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SmallerPaneToggleButtonWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SmallerPaneToggleButtonWidthProperty =
            DependencyProperty.Register("SmallerPaneToggleButtonWidth", typeof(int), typeof(NavigationRibbonItem), new PropertyMetadata(41));



        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _primaryItemsControl = GetTemplateChild("PrimaryItemsControl") as ItemsControl;
            if (_primaryItemsControl is not null)
            {
                _primaryItemsControl.HorizontalAlignment = CommandAlignment;
                RegisterPropertyChangedCallback(CommandAlignmentProperty, 
                    (sender, dp) => 
                        _primaryItemsControl.HorizontalAlignment = (HorizontalAlignment)sender.GetValue(dp));
            }

            _moreButton = GetTemplateChild("MoreButton") as Button;
            if (_moreButton is not null)
            {
                _moreButton.HorizontalAlignment = OverflowButtonAlignment;
                RegisterPropertyChangedCallback(OverflowButtonAlignmentProperty,
                    (sender, dp) =>
                        _moreButton.HorizontalAlignment = (HorizontalAlignment)sender.GetValue(dp));
            }
        }
    }
}