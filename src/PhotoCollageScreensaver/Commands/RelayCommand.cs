﻿using System.Windows.Input;

namespace PhotoCollageScreensaver.Commands;

public class RelayCommand : ICommand
{
    private Action<object> execute;

    private Predicate<object> canExecute;

    private event EventHandler CanExecuteChangedInternal;

    public RelayCommand(Action<object> execute)
        : this(execute, DefaultCanExecute)
    {
    }

    public RelayCommand(Action<object> execute, Predicate<object> canExecute)
    {
        this.execute = execute ?? throw new ArgumentNullException("execute");
        this.canExecute = canExecute ?? throw new ArgumentNullException("canExecute");
    }

    public event EventHandler CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
            this.CanExecuteChangedInternal += value;
        }

        remove
        {
            CommandManager.RequerySuggested -= value;
            this.CanExecuteChangedInternal -= value;
        }
    }

    public bool CanExecute(object parameter) => this.canExecute != null && this.canExecute(parameter);

    public void Execute(object parameter) => this.execute(parameter);

    public void OnCanExecuteChanged()
    {
        var handler = this.CanExecuteChangedInternal;
        if (handler != null)
        {
            //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
            handler.Invoke(this, EventArgs.Empty);
        }
    }

    public void Destroy()
    {
        this.canExecute = _ => false;
        this.execute = _ => { return; };
    }

    private static bool DefaultCanExecute(object parameter) => true;
}
