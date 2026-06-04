using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(EntitiesToResetActionListener), SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(EntitiesToReset))]
public class EntitiesToResetActionListener : Scene, INotify<EntitiesToReset>
{
    private IEnumerator ResetAttributes(EntitiesToReset Data)
    {
        foreach(var entity in Data.entitiesToReset)
        {
            Debug.Log((entity.entity, entity.absractEntity));

            entity.absractEntity.Health.CurrentHealth = entity.absractEntity.Health.MaxHealth; //reset health
        }
        
        yield return null;  
    }

    public IEnumerator Notify(EntitiesToReset value)
    {
        yield return StartCoroutine(ResetAttributes(value));
    }
}
