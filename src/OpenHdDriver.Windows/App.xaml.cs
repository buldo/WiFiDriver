using System.Configuration;
using System.Data;
using System.Drawing;
using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

using Microsoft.Extensions.DependencyInjection;
using OpenHdDriver.Windows.ViewModels;
using Rtl8812auNet.Grpc;

namespace OpenHdDriver.Windows
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon;

        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
            _taskbarIcon = (TaskbarIcon)FindResource("AppNotifyIcon")!;

        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddRtl8812auNetGrpcServer();
            services.AddSingleton<MainWindowViewModel>();

            return services.BuildServiceProvider();
        }
    }

}
