using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using CloudLib.Client.WinUI.Services;
using CloudLib.Client.WinUI.Views;
using Microsoft.UI.Xaml.Controls;
// ReSharper disable InconsistentNaming

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CloudLib.Client.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();

            _activationService =
                new Lazy<ActivationService>(() => new ActivationService(this,
                    typeof(MainPage),
                    new Lazy<ShellPage>(CreateShell)));
        }

        private async void OnGettingFocus(object sender, GettingFocusEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ShellPage CreateShell()
        {
            var shell = new ShellPage();
            NavigationService.Frame = shell.ShellFrame;
            return shell;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            m_window = new MainWindow();

            if (m_window.Content is null)
            {
                m_window.Content = ActivationService.Shell.Value;

                m_window.Content.GettingFocus += OnGettingFocus;
            }

            m_window.Activate();

            //await ActivationService.ActivateAsync(args);
        }



        private MainWindow m_window;
        private readonly Lazy<ActivationService> _activationService;
        internal ActivationService ActivationService => _activationService.Value;
    }
}
