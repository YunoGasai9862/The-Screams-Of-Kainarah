using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using static AssetAddressableAndType;
using System;

public class GameLoad : MonoBehaviour
{
    [SerializeField]
    AssetAddressableAndType gameAddressables;

    public AssetTypeParser AssetTypeParser { get; set; }

    private async void Awake()
    {
        AssetTypeParser = new AssetTypeParser();

        await PreloadAssets();
    }

    private async Task PreloadAssets()
    {
        foreach(AddressableAndType gameAddressable in gameAddressables.addressablesAndTypes)
        {
            AsyncOperationHandle<object> handler = gameAddressable.AssetAddress.LoadAssetAsync<object>();

            await handler.Task;

            object asset = handler.Result;

            AssetType parsedAsset = AssetTypeParser.ParseType(asset, gameAddressable.AssetType);

            //instantiate the object first - on awake those methods will run automatically
            Instantiate(parsedAsset);   

            await Task.CompletedTask;
        }

    }
}