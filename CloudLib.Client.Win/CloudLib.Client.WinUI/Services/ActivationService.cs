using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudLib.Client.WinUI;
using CloudLib.Client.WinUI.Activation;
using CloudLib.Client.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Core.Services
{
    internal class ActivationService
    {
        private readonly Type _defaultNavItem;
        private object _lastActivationArgs;
        public App App { get; }
        public Lazy<UIElement?> Shell { get; }

        public ActivationService(App app, Type defaultNavItem, Lazy<UIElement?> shell)
        {
            _defaultNavItem = defaultNavItem;
            App = app;
            Shell = shell;
        }

        public async Task ActivateAsync(object arguments)
        {
            if (IsInteractive(arguments))
            {
                await InitializeAsync();

                //TODO: Authenticate with STS and obtain token here

                Window.Current.Content ??= Shell?.Value ?? new Frame();
            }

            await HandleActivationAsync(arguments);
            _lastActivationArgs = arguments;

            if (IsInteractive(arguments))
            {
                //Ensure that the current Window is active
                Window.Current.Activate();

                //All tasks which need to be completed AFTER ACTIVATION
                await StartupAsync();
            }
        }

        private async Task StartupAsync()
        {
            await Task.CompletedTask;
        }

        private async Task HandleActivationAsync(object arguments)
        {
            var handler = GetActivationHandlers()
                .FirstOrDefault(h => h.CanHandle(arguments));

            if (handler != null)
                await handler.HandleAsync(arguments);

            if (IsInteractive(arguments))
            {
                var defaultHandler = new DefaultActivationHandler(_defaultNavItem);
                if (defaultHandler.CanHandle(arguments))
                {
                    await defaultHandler.HandleAsync(arguments);
                }
            }
        }

        private IEnumerable<SchemeActivationHandler> GetActivationHandlers()
        {
            yield return new SchemeActivationHandler();
        }

        private async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        private bool IsInteractive(object arguments)
        {
            //TODO: Determine from the passed arguments if the app should activate interactively or not
            return false;
        }
    }
}
