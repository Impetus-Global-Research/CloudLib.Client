using System;

using CloudLib.Client.Behaviors;
using CloudLib.Client.Services.Ink;
using CloudLib.Client.ViewModels;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace CloudLib.Client.Views
{
    // For more information regarding Windows Ink documentation and samples see https://github.com/Microsoft/WindowsTemplateStudio/blob/release/docs/UWP/pages/ink.md
    public sealed partial class InkDrawPicturePage : Page
    {
        public InkDrawPictureViewModel ViewModel { get; } = new InkDrawPictureViewModel();

        public InkDrawPicturePage()
        {
            InitializeComponent();
            SetNavigationViewHeaderContext();
            SetNavigationViewHeaderTemplate();

            Loaded += (sender, eventArgs) =>
            {
                SetCanvasSize();
                image.SizeChanged += Image_SizeChanged;

                var strokeService = new InkStrokesService(inkCanvas.InkPresenter);

                ViewModel.Initialize(
                    strokeService,
                    new InkPointerDeviceService(inkCanvas),
                    new InkFileService(inkCanvas, strokeService),
                    new InkZoomService(canvasScroll));
            };
        }

        private void OnInkToolbarLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is InkToolbar inkToolbar)
            {
                inkToolbar.TargetInkCanvas = inkCanvas;
            }
        }

        private void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e) => SetNavigationViewHeaderTemplate();

        private void SetNavigationViewHeaderTemplate()
        {
            if (visualStateGroup.CurrentState != null)
            {
                switch (visualStateGroup.CurrentState.Name)
                {
                    case "BigVisualState":
                        NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["BigHeaderTemplate"] as DataTemplate);
                        bottomCommandBar.Visibility = Visibility.Collapsed;
                        break;
                    case "SmallVisualState":
                        NavigationViewHeaderBehavior.SetHeaderTemplate(this, Resources["SmallHeaderTemplate"] as DataTemplate);
                        bottomCommandBar.Visibility = Visibility.Visible;
                        break;
                }
            }
        }

        private void SetNavigationViewHeaderContext()
        {
            var headerContextBinding = new Binding
            {
                Source = ViewModel,
                Mode = BindingMode.OneWay,
            };

            SetBinding(NavigationViewHeaderBehavior.HeaderContextProperty, headerContextBinding);
        }

        private void SetCanvasSize()
        {
            inkCanvas.Width = Math.Max(canvasScroll.ViewportWidth, 1000);
            inkCanvas.Height = Math.Max(canvasScroll.ViewportHeight, 1000);
        }

        private void Image_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Height == 0 || e.NewSize.Width == 0)
            {
                SetCanvasSize();
            }
            else
            {
                inkCanvas.Width = e.NewSize.Width;
                inkCanvas.Height = e.NewSize.Height;
            }
        }
    }
}
