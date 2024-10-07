public interface IDeletegate
{
    public delegate void InvokeMethod();

    public abstract InvokeMethod InvokeCustomMethod { get; set; }
}