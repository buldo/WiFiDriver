using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.DependencyInjection;
using OpenHdDriver.Windows.ViewModels;

namespace OpenHdDriver.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetRequiredService<MainWindowViewModel>();
        }

        private void Expander_OnExpanded(object sender, RoutedEventArgs e)
        {
            //LogsRowDefinition.MinHeight = 80;
            //LogsRowDefinition.Height = null;
        }

        private void Expander_OnCollapsed(object sender, RoutedEventArgs e)
        {
            //LogsRowDefinition.MinHeight = 0;
            //LogsRowDefinition.Height = new GridLength(25);
        }
    }
}