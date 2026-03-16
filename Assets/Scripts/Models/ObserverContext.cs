using System;

public class ObserverContext: Context
{
    public Type SubjectType { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()} Subject Type: {SubjectType}";
    }
}

public class ObserverContext<T> : ObserverContext
{
    private Type ContextType { get; set; }
    public override string ToString()
    {
        return $"{base.ToString()}, ContextType: {typeof(T)}";
    }
}
