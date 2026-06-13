using Assets.Scripts.Scene.Interface;
using System;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class Scene : MonoBehaviour, IScene
    {
        private dynamic Value { get; set; }

        private SceneUtils SceneUtils { get; set; }

        public virtual void Broadcast(dynamic value)
        {
            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value;
            }
        }

        public virtual void Broadcast<T>(T value)
        {
            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value as SceneUtils;
            }
        }
    }
}
