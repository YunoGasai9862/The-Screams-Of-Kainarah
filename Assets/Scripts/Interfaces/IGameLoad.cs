using Assets.Annotations;
using System;
using System.Threading.Tasks;

public interface IGameLoad
{
    public Task<UnityEngine.Object> PreloadAsset<T>(EntityMetaData enttityMetaData) where T : UnityEngine.Object;

    public Task<Object> PreloadChildAsset(ChildAssetAttribute childAssetAttribute);
}