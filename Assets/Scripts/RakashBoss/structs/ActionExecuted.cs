public struct ActionExecuted {  }

public class  ActionExecuted<T>
{
    public T Result { get; set; }

    public ActionExecuted(T item) { 
    
        Result = item;  
    }
}
