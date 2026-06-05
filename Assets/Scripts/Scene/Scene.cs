using Assets.Scripts.Scene.Interface;
using System;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class Scene : MonoBehaviour, IScene
    {
        public virtual void Broadcast(dynamic value)
        {
        }

        public virtual void Broadcast<T>(T value)
        {
        }
    }
}
