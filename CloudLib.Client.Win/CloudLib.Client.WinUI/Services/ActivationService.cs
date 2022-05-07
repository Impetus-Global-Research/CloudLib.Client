using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudLib.Client.WinUI.Activation;
using CloudLib.Client.WinUI.Views;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CloudLib.Client.WinUI.Services
{
    internal class ActivationService
    {
        private readonly Type _defaultNavItem;
        private object? _lastActivationArgs;
        public App App { get; }
        public Lazy<ShellPage> Shell { get; }

        public ActivationService(App app, Type defaultNavItem, Lazy<ShellPage> shell)
        {
            _defaultNavItem = defaultNavItem;
            App = app;
            Shell = shell;
        }

        public async Task ActivateAsync(object arguments)
        {
            if (IsInteractive(arguments))
            {
                //All tasks which need to be completed AFTER ACTIVATION
                await StartupAsync();

                //TODO: Authenticate with STS and obtain token here
            }

            await HandleActivationAsync(arguments);
            _lastActivationArgs = arguments;
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

        private IEnumerable<ActivationHandler> GetActivationHandlers()
        {
            yield return new DefaultActivationHandler(_defaultNavItem);
        }

        private async Task InitializeAsync()
        {
            await Task.CompletedTask;
        }

        private bool IsInteractive(object arguments)
        {
            //TODO: Determine from the passed arguments if the app should activate interactively or not
            return true;
        }
    }
}
