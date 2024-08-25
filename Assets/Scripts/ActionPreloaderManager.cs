using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionPreloaderManager: MonoBehaviour
{
    [SerializeField]
    ActionPreloader actionPreloader;
    [SerializeField]
    List<PreloadEntity> preloadEntities;

    private async void Awake()
    {
        await PreloadEntities(preloadEntities, actionPreloader);
    }
    private async Task PreloadEntities(List<PreloadEntity> preloadEntities, ActionPreloader actionPreloader)
    {
        foreach (var preloadEntity in preloadEntities)
        {
            await preloadEntity.Entity.EntityPreloadAction(preloadEntity.AssetAddress, actionPreloader);
        }
    }
}