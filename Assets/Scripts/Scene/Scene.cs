using Assets.Scripts.Scene.Interface;
using System;
using System.Collections.Generic;
using System.Text;
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
