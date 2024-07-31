using System.Threading.Tasks;
using UnityEngine;

public class GameLoad : MonoBehaviour
{
    [SerializeField]
    AssetAddressablesAndType[] gameAddressables;
    private void Start()
    {

    }

    private async Task PreloadAssets()
    {
        foreach(AssetAddressablesAndType gameAddressable in gameAddressables)
        {
           // gameAddressable.LoadAssetAsync<gaAssetReferenceeADdr>
        }

        await Task.CompletedTask;
    }
}