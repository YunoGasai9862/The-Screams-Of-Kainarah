using System;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPreloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    public async Task PreloadAssetWithAction<T, TAction>(T objectType, IAssetPreload asset, Action<TAction> action, TAction value)
    {
        //use event driven approach!!
       // await .PreloadAsset(objectType, asset);

        action.Invoke(value);
    }

    public async Task PreloadAssetWithAction<T>(T objectType, IAssetPreload asset, Action action)
    {
        //use event driven approach!!
       // await SceneSingleton.GetGameLoadManager().PreloadAsset(objectType, asset);

        action.Invoke();
    }

}