using System;

public class PlayerState<T> where T: Enum
{
    public T CurrentState { get; set; }

    public bool IsConcluded { get; set; }
}