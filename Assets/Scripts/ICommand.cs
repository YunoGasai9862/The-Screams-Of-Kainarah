using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand<T>
{
    public abstract void Execute(T value= default);
    public abstract void Cancel();
}
