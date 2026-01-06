
using System.Threading.Tasks;
using UnityEngine;
using EnemyHittable;
using System.Threading;
public class EnemyHittableManager : MonoBehaviour, ISubject<EnemyHittableManager>
{
   private EnemyHittableManagerDelegator EnemyHittableManagerDelegator { get; set; }

    private async void Start()
    {
        EnemyHittableManagerDelegator = await Helper.GetDelegator<EnemyHittableManagerDelegator>();

        EnemyHittableManagerDelegator.AddToSubjectsDict(typeof(EnemyHittableManager).ToString(), gameObject.name, new Subject<EnemyHittableManager>(this, typeof(EnemyHittableManager)));
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
        StartCoroutine(EnemyHittableManagerDelegator.NotifyObserver(data, this, new ObserverContext()
        {
            SubjectType = typeof(EnemyHittableManager)

        }, CancellationToken.None));
    }
}
