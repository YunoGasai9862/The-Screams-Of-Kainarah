using System.Threading.Tasks;
using UnityEngine;

public abstract class EntityPreload: MonoBehaviour, IEntityPreload
{
    public abstract Task EntityPreloadAction(ActionPreloader actionPreloader);
}