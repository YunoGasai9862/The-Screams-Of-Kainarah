using Assets.Scripts.BaseScene;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class ActionExecutor : MonoBehaviorScene, IAction
{
    public Task ExecuteAction<T>(Action<T> action, T value)
    {
        action(value);

        return Task.CompletedTask;
    }
}