using System;

public class State<T> where T: Enum 
{
    public T CurrentState { get; set; }
    public bool IsConcluded { get; set; }
}