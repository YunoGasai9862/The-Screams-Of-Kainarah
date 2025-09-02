
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Asset(AssetType = Asset.MONOBEHAVIOR, AddressLabel = "Player", InitialPositionX = -5.0f, InitialPositionY = 10.0f)]
public class PlayerSystem : MonoBehaviour, ISubject<IObserver<PlayerSystem>>
{
    private PlayerSystemDelegator PlayerSystemDelegator { get; set; }

    private void Awake()
    {
        PlayerSystemDelegator = Helper.GetDelegator<PlayerSystemDelegator>();
    }

    private void Start()
    {
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

    public void OnNotifySubject(IObserver<PlayerSystem> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        StartCoroutine(PlayerSystemDelegator.NotifyObserver(data, this, new NotificationContext()
        {
            SubjectType = typeof(PlayerSystem).ToString()

        }, CancellationToken.None));
    }
}
