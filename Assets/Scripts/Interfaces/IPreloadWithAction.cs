
using System.Threading.Tasks;
using System;
using UnityEngine.AddressableAssets;
public interface IPreloadWithAction
{
    public Task ExecuteAction<TAction>(Action<TAction> action, TAction value);
}