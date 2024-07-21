using NUnit.Framework;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameLoad : MonoBehaviour
{
    [SerializeField]
    AssetReference[] gameAddressables;
    private void Start()
    {

    }

    private async Task PreloadAssets()
    {
        await Task.CompletedTask;
    }
}