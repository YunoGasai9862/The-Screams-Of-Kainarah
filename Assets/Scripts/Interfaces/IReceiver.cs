
public interface IReceiver<T>: IReceiverBase<T>
{
    T PerformAction(T value = default);
    T CancelAction();
}

public interface IReceiver<T, Z> : IReceiverBase<T>
{
    Z PerformAction(T value = default);
    Z CancelAction();
}
