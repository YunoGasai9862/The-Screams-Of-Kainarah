
public interface IGenericDelegate<T>
{
    public delegate void InvokeMethod(T value);

    public InvokeMethod InvokeCustomMethod { get; set; }
}