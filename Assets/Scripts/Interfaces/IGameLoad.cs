using Assets.Annotations;
using Assets.Scripts.Models;
using System;
using System.Threading.Tasks;

public interface IGameLoad
{
    public Task<UnityEngine.Object> PreloadAsset<T>(EntityMetaData enttityMetaData) where T : UnityEngine.Object;

    public Task<object> PreloadChildAsset(ChildAssetAttributeDto childAssetAttributeDto);
}