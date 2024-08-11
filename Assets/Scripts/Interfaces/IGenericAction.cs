using System;

public interface IGenericAction<T>
{
    public Action<T> ExecuteActionOnLoad { get; }
}