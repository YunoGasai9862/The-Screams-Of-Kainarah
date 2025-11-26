public struct ActionExecuted
{
    public bool Result { get; set; }
}

public class ActionExecuted<T>
{
    public T Result { get; set; }

    public ActionExecuted(T item) { 
    
        Result = item;  
    }
}
