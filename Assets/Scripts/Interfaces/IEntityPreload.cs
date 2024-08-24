using System;
using System.Threading.Tasks;

public interface IEntityPreload
{
    public Task EntityPreloadAction(ActionPreloader actionPreloader);
}