using Assets.Scripts.Broadcaster.Interface;
using Assets.Scripts.Polling.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Broadcaster
{
    public class Broadcaster : Scene.MonoBehaviorScene, IBroadcaster, IPoller
    {
        private SceneRegistry SceneRegistryInstance { get; set; }

        private SceneUtils SceneUtilsInstance { get; set; }

        private void Awake()
        {
            SceneUtilsInstance = FindFirstObjectByType<SceneUtils>();

            SceneRegistryInstance = SceneUtilsInstance.FindObject<SceneRegistry>();

            Broadcast(SceneUtilsInstance);
        }

        public void Broadcast<T>(T value)
        {
            foreach (KeyValuePair<int, GameObject> item in SceneRegistryInstance.GetRegisteredGameObjects())
            {
                item.Value.GetComponent<Scene.MonoBehaviorScene>().Broadcast(value);
            }
        }

        public IEnumerator Poll(int pollingIntervalInSeconds)
        {
            Broadcast(SceneUtilsInstance);

            yield return new WaitForSeconds(pollingIntervalInSeconds);
        }
    }
}
