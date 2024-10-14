public interface IDelegate
{
    public delegate void InvokeMethod();

    public abstract InvokeMethod InvokeCustomMethod { get; set; }
}
