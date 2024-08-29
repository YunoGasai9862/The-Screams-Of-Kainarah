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
            //separate this somehow now!
            await preloadEntity.EntityMB.EntityPreloadAction(preloadEntity.AssetAddress, preloader);
            await preloadEntity.EntitySO.EntityPreloadAction(preloadEntity.AssetAddress, preloader);
        }
    }
}