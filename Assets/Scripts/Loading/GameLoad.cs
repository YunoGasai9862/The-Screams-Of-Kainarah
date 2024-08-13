using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

public class GameLoad<T, TAction> : MonoBehaviour, IGameLoad<T,TAction>
{
    [SerializeField]
    AssetAddressableAndType gameAddressables;
    IAssetPreload[] assets;

    public AssetTypeParser AssetTypeParser { get; set; }

    private void Awake()
    {
        AssetTypeParser = new AssetTypeParser();
         
       // await PreloadAssets();
    }

    //continue with this appraoch
    private async Task PreloadAssets(T objectType, TAction action)
    {
        foreach (IAssetPreload asset in assets)
        {
            AssetReference assetReference = asset.AssetAddress;

            AsyncOperationHandle<T> handler = Addressables.LoadAssetAsync<T>(assetReference);

            await handler.Task;

            T loadedAsset = handler.Result;

            Addressables.Release(handler);
        }

    }
}