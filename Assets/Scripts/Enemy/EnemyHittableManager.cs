
using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using EnemyHittable;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(EnemyHittableManager), ContextType = typeof(EnemyHittableManager))]
public class EnemyHittableManager : MonoBehaviour, IRequest<EnemyHittableManager>
{
   private Delegator Delegator { get; set; }

    private async void Start()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }

    public Task<bool> IsEntityAnAttackObject(Collider2D collider, EnemyHittableObjects objects)
    {
        for (int i = 0; i < objects.elements.Length; i++)
        {
            var element = objects.elements[i];

            if (collider.tag == element.ObjectTag) //scriptable Object
            {
                return Task.FromResult(true);
            }
        }

        return Task.FromResult(false);
    }

    public void OnNotifySubject(IObserver<EnemyHittableManager> data, ObserverContext context, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
    
    }

    public IEnumerator Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<EnemyHittableManager>()
        {
            EntityType = typeof(EnemyHittableManager),
            Data = this

        }, this));
    }
}
