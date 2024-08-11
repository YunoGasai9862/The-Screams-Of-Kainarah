using System;

public interface IAction<T>
{
    public Action<T> ExecuteActionOnLoad { get; set; }
}