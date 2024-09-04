using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PreloaderManager: MonoBehaviour
{
    [SerializeField]
    Preloader preloader;

    [SerializeField]
    GameLoad gameLoad;

    [SerializeField]
    PreloadEntity[] preloadEntities;

    private async void Awake()
    {
        //please use event system to instantiate it!
        //Instantiate(gameLoad);

        await PreloadEntities(preloadEntities, preloader);
    }
    private async Task PreloadEntities(PreloadEntity[] preloadEntities, Preloader preloader)
    {
        foreach (var preloadEntity in preloadEntities)
        {
            Debug.Log($"Preload Entity: {preloadEntity.ToString()}");
            await preloadEntity.GetEntityToPreload().EntityPreloadAction(preloadEntity.AssetAddress, preloadEntity.PreloadEntityType, preloader);
        }
    }
}