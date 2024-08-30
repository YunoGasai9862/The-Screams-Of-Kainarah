using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PreloaderManager: MonoBehaviour
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    PreloadEntity[] preloadEntities;

    private async void Awake()
    {
        await PreloadEntities(preloadEntities, preloader);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (var preloadEntity in preloadEntities)
        {
            await preloadEntity.GetEntityToPreload().EntityPreloadAction(preloadEntity.AssetAddress, preloader);
        }
    }
}