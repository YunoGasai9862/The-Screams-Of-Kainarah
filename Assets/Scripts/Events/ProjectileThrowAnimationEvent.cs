using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEvent
{
    private static ProjectileThrowAnimationEvent Instance = new ProjectileThrowAnimationEvent();
    public ProjectileThrowAnimationEvent() { }

    public static ProjectileThrowAnimationEvent GetInstance()
    {
        return Instance;
    }
}