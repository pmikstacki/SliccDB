using System.Windows;
using Caliburn.Micro;
using SliccDB.Explorer.ViewModels;

namespace SliccDB.Explorer
{
    public class SliccBootstrapper : BootstrapperBase
    {
        public SliccBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewForAsync<MainWindowViewModel>();
        }
    }
}
