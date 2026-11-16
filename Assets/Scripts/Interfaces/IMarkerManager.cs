using Annotations.Enums;
using Assets.Scripts.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMarkerManager
    {
        GameObject FindMarker(string markerName, string markerTag = "");

        Task<UnityEngine.Object> Load(Asset assetType, bool enforceChildHierarchy, string addressLabel, GameObject marker);

        List<ChildAssetAttributeDto> FindChildAssets<T>(GameObject parent, Vector3 childsPosition) where T : class;
    }
}
