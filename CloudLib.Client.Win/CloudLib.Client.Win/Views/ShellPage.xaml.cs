using System;

using CloudLib.Client.Win.ViewModels;

using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Composition;

namespace CloudLib.Client.Win.Views
{
    // TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    public sealed partial class ShellPage : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public ShellPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            //ViewModel.Initialize(shellFrame, navigationView, KeyboardAccelerators);
        }
    }
}
