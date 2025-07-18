using System;

public class GenericStateBundle<T> where T: IStateBundle
{
    public T StateBundle { get; set; }
} 