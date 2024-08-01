using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour
{
    [SerializeField]
    AssetAddressableAndType[] gameAddressables;
    private void Start()
    {

    }

    private async Task PreloadAssets()
    {
        foreach(AssetAddressableAndType gameAddressable in gameAddressables)
        {
          Type type = gameAddressable.AssetType;
        }

        await Task.CompletedTask;
    }

    //use this approach to load assets
    private async Task LoadAssetType<T>()
    {
       
    }  
}