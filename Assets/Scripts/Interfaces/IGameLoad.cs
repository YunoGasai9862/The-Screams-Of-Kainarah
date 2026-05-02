using System;
using System.Threading.Tasks;

public interface IGameLoad
{
    public Task<UnityEngine.Object> PreloadAsset<T>(EntityMetaData enttityMetaData) where T : UnityEngine.Object;
}