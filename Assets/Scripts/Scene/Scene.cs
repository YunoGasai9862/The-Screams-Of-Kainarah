using Assets.Scripts.Scene.Interface;
using System;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class Scene : MonoBehaviour, IScene
    {
        protected SceneUtils SceneUtils { get; set; }

        public virtual void Broadcast(dynamic value)
        {
            Debug.Log($"Broadcasting value of type {value.GetType()}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value;
            }
        }

        public virtual void Broadcast<T>(T value)
        {
            Debug.Log($"Broadcasting value of type {typeof(T)}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value as SceneUtils;
            }
        }
    }

    public class StateMachineScene : StateMachineBehaviour, IScene
    {
        protected dynamic Value { get; set; }

        protected SceneUtils SceneUtils { get; set; }

        public virtual void Broadcast(dynamic value)
        {
            Debug.Log($"Broadcasting value of type {value.GetType()}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value;
            }
        }

        public virtual void Broadcast<T>(T value)
        {
            Debug.Log($"Broadcasting value of type {typeof(T)}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value as SceneUtils;
            }
        }
    }

    public class ScriptableObjectScene : ScriptableObject, IScene
    {
        protected dynamic Value { get; set; }

        protected SceneUtils SceneUtils { get; set; }

        public virtual void Broadcast(dynamic value)
        {
            Debug.Log($"Broadcasting value of type {value.GetType()}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value;
            }
        }

        public virtual void Broadcast<T>(T value)
        {
            Debug.Log($"Broadcasting value of type {typeof(T)}");

            if (value is SceneUtils && SceneUtils == null)
            {
                SceneUtils = value as SceneUtils;
            }
        }
    }
}
