using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEventWOT
{
    private static UnityEvent Instance = new UnityEvent();
    public ProjectileThrowAnimationEvent() { }

    public override UnityEvent GetInstance()
    {
        return Instance;
    }
    public static void AddEventListener(UnityAction value)
    {
       Instance.AddListener(value);
    }

}