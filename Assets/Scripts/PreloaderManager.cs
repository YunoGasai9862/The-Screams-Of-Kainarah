using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PreloaderManager: MonoBehaviour
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    List<PreloadEntity> preloadEntities;

    private async void Awake()
    {
        await PreloadEntities(preloadEntities, preloader);
    }
    private async Task PreloadEntities(List<PreloadEntity> preloadEntities, Preloader preloader)
    {
        foreach (var preloadEntity in preloadEntities)
        {
            await preloadEntity.Entity.EntityPreloadAction(preloadEntity.AssetAddress, preloader);
        }
    }
}