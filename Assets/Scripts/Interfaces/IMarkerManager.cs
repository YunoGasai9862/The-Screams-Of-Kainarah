using Annotations.Enums;
using Assets.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMarkerManager
    {
        GameObject FindMarker(string markerName, string markerTag = "");

        Task<UnityEngine.Object> Load(Asset assetType, bool enforceChildHierarchy, string addressLabel, GameObject marker);

        List<ChildAssetAttribute> FindChildAssets<T>(GameObject parent) where T : class;
    }
}
