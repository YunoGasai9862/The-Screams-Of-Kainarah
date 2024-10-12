public interface IGenericDeletegate<T>
{
    public delegate T InvokeMethod();

    public abstract InvokeMethod InvokeCustomMethod { get; set; }
}
