using Annotations.Enums;
using Assets.Scripts.Broadcaster.Interface;
using Assets.Scripts.Polling.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Broadcaster
{
    [AssetAttribute(Asset.MONOBEHAVIOR, "Broadcaster")]
    public class Broadcaster : Scene.Scene, IBroadcaster, IPoller
    {
        private SceneRegistry SceneRegistryInstance { get; set; }

        private SceneUtils SceneUtilsInstance { get; set; }

        private void Awake()
        {
            SceneUtilsInstance = FindFirstObjectByType<SceneUtils>();

            Debug.Log($"[Broadcaster]SceneUtilsInstance: {SceneUtilsInstance}");

            if (SceneUtilsInstance == null)
            {
                Debug.Log($"[Broadcaster]SceneUtils is null!");
                return;
            }

            SceneRegistryInstance = SceneUtilsInstance.FindObject<SceneRegistry>();

            Debug.Log($"[Broadcaster]SceneRegistryInstance: {SceneRegistryInstance}");

            Broadcast(SceneUtilsInstance);
        }

        public void Broadcast<T>(T value)
        {
            foreach (KeyValuePair<int, GameObject> item in SceneRegistryInstance.GetRegisteredGameObjects())
            {
                item.Value.GetComponent<Scene.Scene>().Broadcast(value);
            }
        }

        public IEnumerator Poll(int pollingIntervalInSeconds)
        {
            Broadcast(SceneUtilsInstance);

            yield return new WaitForSeconds(pollingIntervalInSeconds);
        }
    }
}
