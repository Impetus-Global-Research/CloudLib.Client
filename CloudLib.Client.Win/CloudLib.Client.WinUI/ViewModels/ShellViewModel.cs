using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.System;
using Microsoft.Windows.System;
using CloudLib.Client.WinUI.Core.Helpers;
using CloudLib.Client.WinUI.Core.Services;
using CloudLib.Client.WinUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using CloudLib.Client.WinUI.Views;
using System.Drawing;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml;

namespace CloudLib.Client.WinUI.ViewModels
{
    public class ShellViewModel : ObservableObject
    {
        private readonly KeyboardAccelerator _altLeftKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu);
        private readonly KeyboardAccelerator _backKeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack);

        private NavigationViewItem? _footerMenuItemsSource;
        private bool _isBackEnabled;
        private IList<KeyboardAccelerator>? _keyboardAccelerators;
        private NavigationView? _navigationView;
        private NavigationViewItem? _selected;
        private ICommand? _loadedCommand;
        private ICommand? _itemInvokedCommand;
        private RelayCommand? _userProfileCommand;
        private UserViewModel? _user;
        private bool _isBusy;
        private bool _isLoggedIn;
        private bool _isAuthorized;

        private IdentityService IdentityService => Singleton<IdentityService>.Instance;

        private UserDataService UserDataService => Singleton<UserDataService>.Instance;

        public bool IsBackEnabled
        {
            get => _isBackEnabled;
            set => SetProperty(ref _isBackEnabled, value);
        }

        public NavigationViewItem? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand LoadedCommand => _loadedCommand ??= new RelayCommand(OnLoaded);

        public ICommand ItemInvokedCommand => _itemInvokedCommand ??= new RelayCommand<NavigationViewSelectionChangedEventArgs>(OnItemInvoked);

        public RelayCommand UserProfileCommand => _userProfileCommand ??= new RelayCommand(OnUserProfile, () => !IsBusy);

        public UserViewModel? User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
            //UserProfileCommand.NotifyCanExecuteChanged();
        }

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                if (_isLoggedIn != value)
                {
                    AuthChanged.Invoke(this, new AuthChangedEventArgs(_isLoggedIn, value));
                }
                SetProperty(ref _isLoggedIn, value);
            }
        }

        public event EventHandler<AuthChangedEventArgs> AuthChanged;

        public bool IsAuthorized
        {
            get => _isAuthorized;
            set => SetProperty(ref _isAuthorized, value);
        }
        public NavigationViewItem FooterMenuItemsSource
        {
            get => _footerMenuItemsSource ??= BuildDefaultFooterMenuItems();
            set => SetProperty(ref _footerMenuItemsSource, value);
        }

        public void Initialize(Frame frame, NavigationView navigationView, IList<KeyboardAccelerator>? keyboardAccelerators = default)
        {
            _navigationView = navigationView;
            _keyboardAccelerators = keyboardAccelerators ?? new List<KeyboardAccelerator>();

            AuthChanged += OnAuthChanged;

            NavigationService.Frame = frame;
            NavigationService.NavigationFailed += Frame_NavigationFailed;
            NavigationService.Navigated += Frame_Navigated;

            _navigationView.BackRequested += OnBackRequested;

            IdentityService.LoggedIn += OnLoggedIn;
            IdentityService.LoggedOut += OnLoggedOut;

            UserDataService.UserDataUpdated += OnUserDataUpdated;

            _footerMenuItemsSource = BuildDefaultFooterMenuItems();
        }

        private async void OnAuthChanged(object? sender, AuthChangedEventArgs e)
        {
            UpdateNavigationMenuFooter(sender, e.IsLoggedIn);
        }

        private void UpdateNavigationMenuFooter(object? sender, bool isLoggedIn)
        {
            NavigationViewItem navFooter = isLoggedIn 
                ? BuildFooterMenuItems() 
                : BuildDefaultFooterMenuItems();

            NavigationViewItem BuildFooterMenuItems()
            {

                NavigationViewItem profileMenuItem = new()
                {
                    Icon = User is not null
                        ? new ImageIcon { Source = User.Photo }
                        : new SymbolIcon(Symbol.Contact)
                };


                NavHelper.SetNavigateTo(profileMenuItem, typeof(LoginPage));


                return new NavigationViewItem()
                {
                    MenuItems =
                    {
                        profileMenuItem,
                    },
                };
            }
        }

        private static NavigationViewItem BuildDefaultFooterMenuItems()
        {
            var profileMenuItem = new NavigationViewItem
            {
                Icon = new SymbolIcon(Symbol.Contact)
            };

            NavHelper.SetNavigateTo(profileMenuItem, typeof(LoginPage));


            return new NavigationViewItem{
                MenuItems = 
                { 
                    profileMenuItem, 
                },
            };
        }

        internal async void OnLoaded()
        {
            // Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            // More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            _keyboardAccelerators!.Add(_altLeftKeyboardAccelerator);
            _keyboardAccelerators!.Add(_backKeyboardAccelerator);
            IsLoggedIn = IdentityService.IsLoggedIn();
            IsAuthorized = IsLoggedIn && IdentityService.IsAuthorized();
            User = await UserDataService.GetUserAsync();


        }

        private void OnUserDataUpdated(object? sender, UserViewModel? userData)
        {
            User = userData;
        }

        private void OnLoggedIn(object? sender, EventArgs e)
        {
            IsLoggedIn = true;
            IsAuthorized = IsLoggedIn && IdentityService.IsAuthorized();
            IsBusy = false;
        }

        private void OnLoggedOut(object? sender, EventArgs e)
        {
            User = null;
            IsLoggedIn = false;
            IsAuthorized = false;
            CleanRestrictedPagesFromNavigationHistory();
            GoBackToLastUnrestrictedPage();
        }

        private static void CleanRestrictedPagesFromNavigationHistory()
        {
            NavigationService.Frame?.BackStack
            .Where(b => Attribute.IsDefined(b.SourcePageType, typeof(RestrictedAttribute)))
            .ToList()
            .ForEach(page => NavigationService.Frame.BackStack.Remove(page));
        }

        private static void GoBackToLastUnrestrictedPage()
        {
            var currentPage = NavigationService.Frame?.Content as Page;
            var isCurrentPageRestricted = Attribute.IsDefined(currentPage!.GetType(), typeof(RestrictedAttribute));
            if (isCurrentPageRestricted)
            {
                NavigationService.GoBack();
            }
        }

        private async void OnUserProfile()
        {
            if (IsLoggedIn)
            {
                //NavigationService.Navigate<SettingsPage>();
            }
            else
            {
                IsBusy = true;
                var loginResult = await IdentityService.LoginAsync();
                if (loginResult != LoginResultType.Success)
                {
                    //await AuthenticationHelper.ShowLoginErrorAsync(loginResult);
                    IsBusy = false;
                }
            }
        }

        private void OnItemInvoked(NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                NavigationService.Navigate(typeof(SettingsPage), null, args.RecommendedNavigationTransitionInfo);
            }
            else
            {
                var selectedItem = args.SelectedItemContainer as NavigationViewItem;

                if (selectedItem?.GetValue(NavHelper.NavigateToProperty) is Type pageType)
                {
                    NavigationService.Navigate(pageType, null, args.RecommendedNavigationTransitionInfo);
                }
            }
        }

        private static void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => NavigationService.GoBack();

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e) => throw e.Exception;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            IsBackEnabled = NavigationService.CanGoBack;
            //if (e.SourcePageType == typeof(SettingsPage))
            //{
            //    Selected = _navigationView.SettingsItem as NavigationViewItem;
            //    return;
            //}

            var selectedItem = GetSelectedItem(_navigationView!.MenuItems, e.SourcePageType);
            if (selectedItem != null)
            {
                Selected = selectedItem;
            }
        }

        private static NavigationViewItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
        {
            foreach (var item in menuItems.OfType<NavigationViewItem>())
            {
                if (IsMenuItemForPageType(item, pageType))
                {
                    return item;
                }

                var selectedChild = GetSelectedItem(item.MenuItems, pageType);
                if (selectedChild != null)
                {
                    return selectedChild;
                }
            }

            return null;
        }

        private static bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
        {
            var pageType = menuItem.GetValue(NavHelper.NavigateToProperty) as Type;
            return pageType == sourcePageType;
        }

        private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
        {
            var keyboardAccelerator = new KeyboardAccelerator { Key = key };
            if (modifiers.HasValue)
            {
                keyboardAccelerator.Modifiers = modifiers.Value;
            }

            keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;
            return keyboardAccelerator;
        }

        private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var result = NavigationService.GoBack();
            args.Handled = result;
        }
    }
}
