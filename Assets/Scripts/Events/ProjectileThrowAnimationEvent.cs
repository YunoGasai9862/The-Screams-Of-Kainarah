using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEventWOT
{
    private static ProjectileThrowAnimationEvent Instance = new ProjectileThrowAnimationEvent();
    public ProjectileThrowAnimationEvent() { }

    public static ProjectileThrowAnimationEvent GetInstance()
    {
        return Instance;
    }
}