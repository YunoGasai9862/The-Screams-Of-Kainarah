using Assets.Annotations;
using System.Collections;
using System.Threading.Tasks;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

[Observer(ObserverType = typeof(EntitiesToResetActionListener), SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(EntitiesToReset))]
public class EntitiesToResetActionListener : MonoBehaviour, INotify<EntitiesToReset>
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
