
public class Command : ICommand
{
    private IReceiver _receiver;

    public Command(IReceiver receiver)
    {
        this._receiver = receiver;
    }
    public void Cancel()
    {
        this._receiver.CancelAction();
    }

    public void Execute()
    {
        this._receiver.PerformAction();
    }
}
