public class Association<T>
{
    public IObserver<T> Observer { get; set; }
    
    public Subject<IObserver<T>> Subject { get; set; }

    public override string ToString()
    {
        return $"Observer: {Observer}, Subject : {Subject}";
    }
}