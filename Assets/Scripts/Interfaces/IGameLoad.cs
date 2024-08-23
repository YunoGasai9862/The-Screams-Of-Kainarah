using System;
using System.Threading.Tasks;
public interface IGameLoad
{
    public Task PreloadAsset<T>(IEntityPreload asset);
}