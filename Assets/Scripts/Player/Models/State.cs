using NUnit.Framework;
using System;
using System.Collections.Generic;

public class State<T, Z> where T: Enum 
{
    public T CurrentState { get; set; }

    public Z CurrentValue { get; set; }

    public bool IsConcluded { get; set; }

    public ResetSystem Reset { get; set; }

    public override string ToString()
    {
        return $"Current State: {CurrentState}, IsConcluded: {IsConcluded}, ResetSystem: {Reset?.ToString()}";
    }
}

public class State<T> where T : Enum
{
    public T CurrentState { get; set; }

    public bool IsConcluded { get; set; }

    public ResetSystem Reset { get; set; }

    public override string ToString()
    {
        return $"Current State: {CurrentState}, IsConcluded: {IsConcluded}, ResetSystem: {Reset?.ToString()}";
    }
}