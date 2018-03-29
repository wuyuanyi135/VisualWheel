using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualWheel.Extensions;

namespace VisualWheel.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public RxCommand LongCommand { get; } = new RxCommand(() => Task.Delay(5000).Wait());
        public MainPageViewModel(INavigationService navigationService) 
            : base (navigationService)
        {
            Title = "Main Page";
            LongCommand.Executing.Subscribe(b => Console.WriteLine($"Executing: {b}"));
            
        }
    }
}
