using System;
using System.Threading.Tasks;

public interface IEntityPreload: IAssetPreload
{
    public Task EntityPreload(ActionPreloader actionPreloader);
}