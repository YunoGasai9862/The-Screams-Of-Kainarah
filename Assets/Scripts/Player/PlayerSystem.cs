
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEngine;
[Asset(AssetType = Asset.MONOBEHAVIOR, AddressLabel = "Player")]
public class PlayerSystem : MonoBehaviour, ISubject<IObserver<Health>>, ISubject<IObserver<PlayerSystem>>
{
    private HealthDelegator HealthDelegator { get; set; }

    private PlayerSystemDelegator PlayerSystemDelegator { get; set; }

    private GlobalGameStateDelegator GlobalGameStateDelegator { get; set; }

    private Health PlayerHealth { get; set; }

    private void Awake()
    {
        PlayerHealth = new Health()
        {
            MaxHealth = 100f,
            CurrentHealth = 100f,
            EntityName = name
        };

        HealthDelegator = Helper.GetDelegator<HealthDelegator>();

        PlayerSystemDelegator = Helper.GetDelegator<PlayerSystemDelegator>();
    }

    private void Start()
    {
        HealthDelegator.AddToSubjectsDict(typeof(PlayerSystem).ToString(), name, new Subject<IObserver<Health>>());
        HealthDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSystem).ToString())[name].SetSubject(this);

        PlayerSystemDelegator.AddToSubjectsDict(typeof(PlayerSystem).ToString(), name, new Subject<IObserver<PlayerSystem>>());
        PlayerSystemDelegator.GetSubsetSubjectsDictionary(typeof(PlayerSystem).ToString())[name].SetSubject(this);
    }

    private Task<List<string>> GetPlayerAnimationsList(Animator anim)
    {
        var animationController = anim.runtimeAnimatorController;

        List<string> animationNames = new List<string>();
        
        foreach(AnimationClip clip in animationController.animationClips)
        {
            animationNames.Add(clip.name);
        }

        return Task.FromResult(animationNames);
    }
    public async Task<string> GetAnimationName(Animator anim, string search) 
    {
        List<string> animationNames = await GetPlayerAnimationsList(anim);

        return animationNames.Where(e => e.Equals(search) || e.Contains(search)).FirstOrDefault();
    }

    public void OnNotifySubject(IObserver<Health> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(HealthDelegator.NotifyObserver(data, PlayerHealth, new NotificationContext()
        {
            SubjectType = typeof(PlayerSystem).ToString()

        }, CancellationToken.None));
    }

    public void OnNotifySubject(IObserver<PlayerSystem> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PlayerSystemDelegator.NotifyObserver(data, this, new NotificationContext()
        {
            SubjectType = typeof(PlayerSystem).ToString()

        }, CancellationToken.None));
    }
}
