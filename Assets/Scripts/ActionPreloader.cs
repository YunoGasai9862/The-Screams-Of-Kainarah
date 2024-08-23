using System;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPreloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    [SerializeField]
    IEntityPreload entities;

    private void Awake()
    {
        
    }

    //array needed so i can group them, and on start of this script, that array will be executed!
    public async Task PreloadAssetWithAction<T, TAction>(IEntityPreload asset, Action<TAction> action, TAction value)
    {
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(asset);

        action.Invoke(value);
    }

    public async Task PreloadAssetWithAction<T>(IEntityPreload asset, Action action)
    {
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(asset);

        action.Invoke();
    }

}