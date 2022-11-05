namespace Metatrader4ClientApp.Dialog
{
    using MaterialDesignThemes.Wpf;
    using Metatrader4ClientApp.Infrastructure.Services;
    using Prism.Common;
    using Prism.Regions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using Unity;

    public class DialogService2 : IDialogService2
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public DialogService2(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Show(string name, dynamic parameters, System.Action<dynamic> callback)
        {
            IRegion region = _regionManager.Regions["DialogRegion"];
            var view = _container.Resolve(typeof(object), name);

            if (!(view is UIElement))
            {
                throw new ArgumentException("A dialog must be a UIElement");
            }
            var dialog = view as FrameworkElement;

            if (!(dialog.DataContext is IDialogAware))
            {
                throw new ArgumentException("A dialog's ViewModel must implement IDialogAware interface");
            }
            var viewModel = dialog.DataContext as IDialogAware;

            DialogHost dialogHost = FindChild<DialogHost>(Application.Current.MainWindow, default);

            ConfigureEvents(dialogHost, viewModel, callback);
            MvvmHelpers.ViewAndViewModelAction<IDialogAware>(viewModel, d => d.OnDialogOpened(parameters));

            _ = region.Add(dialog);
            region.Activate(dialog);

            dialogHost.IsOpen = true;
        }

        private void ConfigureEvents(DialogHost dialogHost, IDialogAware viewModel, Action<dynamic> callback)
        {
            dynamic temp = default;

            dialogHost.DialogOpened += DialogOpenedHandler;
            dialogHost.DialogClosing += DialogClosedHandler;

            void DialogOpenedHandler(object sender, RoutedEventArgs e)
            {
                dialogHost.DialogOpened -= DialogOpenedHandler;
                viewModel.RequestClose += RequestCloseHandler;
            }

            void RequestCloseHandler(dynamic result)
            {
                temp = result;
                dialogHost.IsOpen = false;
            }

            void DialogClosedHandler(object sender, RoutedEventArgs e)
            {
                dialogHost.DialogClosing -= DialogClosedHandler;
                viewModel.RequestClose -= RequestCloseHandler;

                viewModel.OnDialogClosed();

                _ = callback?.Invoke(temp);
            }
        }

        public void ShowDialog(string name, dynamic parameters, System.Action<dynamic> callback) => throw new System.NotImplementedException();

        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null)
            {
                return default;
            }

            T foundChild = default;

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T))
                {
                    foundChild = FindChild<T>(child, childName);
                    if (foundChild != null)
                    {
                        break;
                    }
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        foundChild = (T)child;
                        break;
                    }
                }
                else
                {
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }
    }
}