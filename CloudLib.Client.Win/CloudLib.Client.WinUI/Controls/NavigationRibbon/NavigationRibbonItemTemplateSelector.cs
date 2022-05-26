using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Controls
{
    public class NavigationRibbonItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Normal { get; set; }

        public DataTemplate Contextual { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return item is NavigationRibbonItem t && t.IsContextual ? Contextual : Normal;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) => SelectTemplateCore(item);
    }
}
