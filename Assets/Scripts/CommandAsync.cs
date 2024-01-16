public class CommandAsync<T> : ICommand<T>
{
    private IReceiverAsync<T> _receiver;

    public CommandAsync(IReceiverAsync<T> receiver)
    {
        this._receiver = receiver;
    }
    public void Cancel()
    {
        this._receiver.CancelAction();
    }
    public void Execute(T value = default)
    {
        this._receiver.PerformAction(value);
    }
}
