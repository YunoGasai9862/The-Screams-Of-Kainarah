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

            SceneRegistryInstance = GetSceneRegistryInstance(SceneUtilsInstance);

            if (SceneRegistryInstance == null)
            {
                Debug.Log($"[Broadcaster]SceneRegistryInstance is null...");
                return;
            }

            Broadcast(SceneUtilsInstance);
        }

        public void Broadcast<T>(T value)
        {
            if (SceneRegistryInstance == null)
            {
                //try fetching it again!
                SceneRegistryInstance = GetSceneRegistryInstance(SceneUtilsInstance);

                if (SceneRegistryInstance == null)
                {
                    Debug.Log($"[Broadcaster]SceneRegistryInstance is null...");
                    return;
                }
            }

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

        private SceneRegistry GetSceneRegistryInstance(SceneUtils sceneUtils)
        {
            if (sceneUtils == null)
            {
                Debug.Log($"[Broadcaster]SceneUtils is null!");
                return null;
            }

            SceneRegistry sceneRegistry = sceneUtils.FindObject<SceneRegistry>();

            Debug.Log($"[Broadcaster]SceneRegistryInstance: {sceneRegistry}");

           return sceneRegistry;
        }
    }
}
