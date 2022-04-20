using System;

using CloudLib.Client.Win.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.Win.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; } = new MainViewModel();

        public MainPage()
        {
            InitializeComponent();
        }
    }
}