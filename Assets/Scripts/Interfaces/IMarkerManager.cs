using Annotations.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMarkerManager
    {
        GameObject FindMarker(string markerName, string markerTag = "");

        Task<UnityEngine.Object> Load(Asset assetType, string addressLabel, GameObject marker);

        List<GameObject> FindChildAssets<T>(GameObject parent) where T : class;
    }
}
