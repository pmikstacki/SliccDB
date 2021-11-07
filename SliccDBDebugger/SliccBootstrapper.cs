using System.Windows;
using Caliburn.Micro;
using SliccDebugger.ViewModels;

namespace SliccDebugger
{
    public class SliccBootstrapper : BootstrapperBase
    {
        public SliccBootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<MainWindowViewModel>();
        }
    }
}
