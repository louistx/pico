using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Pico;

public sealed class ActionCommand : ICommand
{
    private readonly Func<Task> _execute;

    public ActionCommand(Func<Task> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { }
        remove { }
    }

    public bool CanExecute(object? parameter) => true;

    public async void Execute(object? parameter)
    {
        await _execute();
    }
}
