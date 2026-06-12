using Assets.Scripts.Scene;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class ActionExecutor : Scene, IAction
{
    public Task ExecuteAction<T>(Action<T> action, T value)
    {
        action(value);

        return Task.CompletedTask;
    }
}