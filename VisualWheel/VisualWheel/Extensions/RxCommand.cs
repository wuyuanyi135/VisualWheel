using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;

namespace VisualWheel.Extensions
{
    interface IRxCommand
    {
        BehaviorSubject<bool> Executing { get; }
        bool DisableWhenExecuting { get; set; }
    }
    
    public class RxCommand : RxCommand<object> {
        public RxCommand(Action executeMethod): this(executeMethod, () => true)
        {
        }

        public RxCommand(Action executeMethod, Func<bool> canExecuteMethod) : base(o => executeMethod() , o => canExecuteMethod())
        {
        }
    }
    public class RxCommand<T> : DelegateCommand<T> , IRxCommand
    {
        public bool IsExecuting { get; private set; } = false;

        public BehaviorSubject<bool> Executing { get; }
        public bool DisableWhenExecuting { get; set; } = true;

        public RxCommand(Action<T> executeMethod) : this(executeMethod, arg => true)
        {
            
        }

        public RxCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod) : base(executeMethod, canExecuteMethod)
        {
            Executing = new BehaviorSubject<bool>(false);
            if (DisableWhenExecuting)
            {
                Executing.Subscribe(b => RaiseCanExecuteChanged());
            }
        }

        // intercept and store the can execute value for auto disabling
        protected override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && !IsExecuting;
        }

        protected override void Execute(object parameter)
        {
            Task.Run(() => {
                IsExecuting = true;
                Executing.OnNext(true);
                try
                {
                    base.Execute(parameter);
                }
                catch (Exception e)
                {
                    Executing.OnError(e);
                }
                IsExecuting = false;
                Executing.OnNext(false);
            });
        }
    }
}
