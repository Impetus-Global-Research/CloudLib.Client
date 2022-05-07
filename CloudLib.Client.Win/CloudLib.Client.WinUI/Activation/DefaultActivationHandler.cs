using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using CloudLib.Client.WinUI.Services;
using Microsoft.UI.Xaml;

namespace CloudLib.Client.WinUI.Activation
{
    internal class DefaultActivationHandler : ActivationHandler<LaunchActivatedEventArgs>
    {
        private readonly Type _navElement;

        public DefaultActivationHandler(Type? navElement)
        {
            _navElement = navElement ?? typeof(MainWindow);
        }

        protected override async Task HandleInternalAsync(LaunchActivatedEventArgs? args)
        {
            // When the navigation stack isn't restored, navigate to the first page and configure
            // the new page by passing required information in the navigation parameter
            if (NavigationService.Frame.DispatcherQueue.HasThreadAccess)
            {
                bool success = NavigationService.Frame.DispatcherQueue.TryEnqueue(
                    () => NavigationService.Navigate(_navElement, null));
            }
        }

        protected override bool CanHandleInternal(LaunchActivatedEventArgs? args)
        {
            // None of the ActivationHandlers has handled the app activation
            return NavigationService.Frame?.Content == null && _navElement != null;
        }
    }
}
