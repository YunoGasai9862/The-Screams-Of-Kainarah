using System;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
public interface IPreloadWithGenericAction
{
    public Task ExecuteGenericAction(Action action);
}