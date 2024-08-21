using System;
using System.Threading.Tasks;

public interface IEntityPreload
{
    public Task EntityPreload(ActionPreloader actionPreloader);
}