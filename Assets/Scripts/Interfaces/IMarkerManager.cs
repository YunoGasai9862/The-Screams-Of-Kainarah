using Annotations.Enums;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMarkerManager
    {
        GameObject FindMarker(string markerName, string markerTag = "");

        async Task<GameObject> Load(Asset assetType, GameObject marker);
    }
}
