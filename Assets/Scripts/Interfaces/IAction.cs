using System;

public interface IAction
{
    public Action ExecuteActionOnLoad { get; }
}