using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using static AssetAddressableAndType;
using System;

public class GameLoad : MonoBehaviour
{
    [SerializeField]
    AssetAddressableAndType gameAddressables;

    public AssetTypeParser AssetTypeParser { get; set; }

    private void Awake()
    {
        AssetTypeParser = new AssetTypeParser();    
    }

    private async Task PreloadAssets()
    {
        foreach(AddressableAndType gameAddressable in gameAddressables.addressablesAndTypes)
        {
            AsyncOperationHandle<object> handler = gameAddressable.AssetAddress.LoadAssetAsync<object>();

            object asset = handler.Result;

            AssetTypeParser.ParseType(asset, gameAddressable.AssetType);

            await handler.Task;
        }

    }
}