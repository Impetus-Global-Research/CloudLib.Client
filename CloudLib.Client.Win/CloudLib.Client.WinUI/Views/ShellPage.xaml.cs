using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using CloudLib.Client.WinUI.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudLib.Client.WinUI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class ShellPage : Page
    {
        public ShellPage()
        {

            this.InitializeComponent();
            ViewModel.Initialize(ShellFrame, NavigationView, new List<KeyboardAccelerator>());
        }


        public ShellViewModel ViewModel { get; } = new();

        private void NavigationView_OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadedCommand.Execute(e);
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if(ViewModel.ItemInvokedCommand.CanExecute(args))
            {
                ViewModel.ItemInvokedCommand.Execute(args);
            }
        }
    }
}
