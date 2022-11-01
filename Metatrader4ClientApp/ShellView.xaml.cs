using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using MahApps.Metro.Controls;

namespace Metatrader4ClientApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The ActiveItem_TransitionCompleted.
        /// </summary>
        /// <param name="sender">The sender <see cref="object"/>.</param>
        /// <param name="e">The e <see cref="System.Windows.RoutedEventArgs"/>.</param>
        private void ActiveItem_TransitionCompleted(object sender, System.Windows.RoutedEventArgs e)
        {
            var sd = (sender as TransitioningContentControl).Content;
        }

        private void HamburgerMenuControl_ItemClick(object sender, ItemClickEventArgs args)
        {
            // TODO; Don t remove this until otherwise hambuger menu show nothing
            this.HamburgerMenuControl.Content = args.ClickedItem;
        }
    }
}
