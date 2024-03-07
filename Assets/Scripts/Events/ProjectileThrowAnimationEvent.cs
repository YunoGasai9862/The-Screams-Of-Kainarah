using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrowAnimationEvent: UnityEventWOT
{
    private static ProjectileThrowAnimationEvent Instance = new ProjectileThrowAnimationEvent();
    public ProjectileThrowAnimationEvent() { }

    public override UnityEventWOT GetInstance()
    {
        return Instance;
    }
    public static void AddEventListener(UnityAction value)
    {
        Instance.AddListener(value);
    }

}