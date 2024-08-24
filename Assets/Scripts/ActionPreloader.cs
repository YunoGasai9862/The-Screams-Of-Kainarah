using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ActionPreloader: MonoBehaviour, IPreloadWithAction, IPreloadWithGenericAction
{
    [SerializeField]
    List<PreloadEntity> entities;

    private async void Awake()
    {
        await PreloadEntities(entities);
    }
    public async Task PreloadAssetWithAction<T, TAction>(AssetReference assetReference, Action<TAction> action, TAction value)
    {
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(assetReference);

        action.Invoke(value);
    }

    public async Task PreloadAssetWithAction<T>(AssetReference assetReference, Action action)
    {
        await SceneSingleton.GetGameLoader().PreloadAsset<T>(assetReference);

        action.Invoke();
    }

    private async Task PreloadEntities(List<PreloadEntity> entities)
    {
        foreach (var entity in entities)
        {
            await entity.entityPreload.EntityPreloadAction(this);
        }
    }
}