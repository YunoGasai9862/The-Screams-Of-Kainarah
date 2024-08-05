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

    private async Task PreloadAssets()
    {
        foreach(AddressableAndType gameAddressable in gameAddressables.addressablesAndTypes)
        {
            AsyncOperationHandle<object> handler = gameAddressable.AssetAddress.LoadAssetAsync<object>();

            object asset = handler.Result;

            Convert.ChangeType(asset, typeof(AssetType));

            await handler.Task;
        }

    }
}