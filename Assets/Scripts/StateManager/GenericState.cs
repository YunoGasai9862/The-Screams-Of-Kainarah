using System;

public class GenericState<T> where T: Enum
{
    public T State { get; set; }
} 