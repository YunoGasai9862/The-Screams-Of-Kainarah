using Assets.Scripts.Broadcaster.Interface;
using Assets.Scripts.Scene;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Broadcaster
{
    public class Broadcaster : Scene.Scene, IBroadcaster
    {
        private SceneRegistry SceneRegistryInstance { get; set; }

        private SceneUtils SceneUtilsInstance { get; set; }

        private void Awake()
        {
            SceneUtilsInstance = FindFirstObjectByType<SceneUtils>();

            SceneRegistryInstance = SceneUtilsInstance.FindObject<SceneRegistry>();
        }

        public void Broadcast<T>(T value)
        {
            foreach (KeyValuePair<int, GameObject> item in SceneRegistryInstance.GetRegisteredGameObjects())
            {
                item.Value.GetComponent<Scene.Scene>().Broadcast(value);
            }
        }
    }
}
