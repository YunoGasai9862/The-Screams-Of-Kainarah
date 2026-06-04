using Assets.Scripts.Scene.Interface;
using System;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class Scene : MonoBehaviour, IScene
    {
        public void Broadcast<T>(T value)
        {
            throw new NotImplementedException();
        }
    }
}
