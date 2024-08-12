using System;

public interface IGenericAction<T>
{
    public abstract Action<T> ExecuteActionOnLoad { get; }

    public abstract Type GetActionType();
}