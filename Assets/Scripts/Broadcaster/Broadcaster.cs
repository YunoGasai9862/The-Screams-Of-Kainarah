using Assets.Scripts.Broadcaster.Interface;
using Assets.Scripts.Polling.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Broadcaster
{
    public class Broadcaster : BaseScene.MonoBehaviorScene, IBroadcaster, IPoller
    {
        private SceneRegistry SceneRegistryInstance { get; set; }

        private SceneUtils SceneUtils { get; set; }

        private void Awake()
        {
            SceneUtils = FindFirstObjectByType<SceneUtils>();

            StartCoroutine(Broadcast(SceneUtils, 5));
        }

        public async void Broadcast<T>(T value, int retries = 5, int delay = 3)
        {
            if (SceneRegistryInstance == null)
            {
                Debug.Log("[Broadcaster]SceneRegistryInstance is null, cannot broadcast.");

                return;
            }

            Debug.Log($"Length of Registered GameObjects: {SceneRegistryInstance.GetRegisteredGameObjects().Count}");

            foreach (KeyValuePair<int, GameObject> item in SceneRegistryInstance.GetRegisteredGameObjects())
            {
                if (item.Value.TryGetComponent<BaseScene.MonoBehaviorScene>(out BaseScene.MonoBehaviorScene component)) {

                    Debug.Log($"Value: {item.Value} - {component}");

                    (await item.Value.GetComponent<BaseScene.MonoBehaviorScene>().GetBaseScene()).Broadcast(value);
                }
            }
        }

        public IEnumerator Poll(int pollingIntervalInSeconds)
        {
            Broadcast(SceneUtils);

            yield return new WaitForSeconds(pollingIntervalInSeconds);
        }

        private IEnumerator Broadcast(SceneUtils sceneUtils, int retryLimit = 3, int retryDelay = 3)
        {
            for (int i=0; i < retryLimit; i++)
            {
                SceneRegistry sceneRegistry = sceneUtils.FindObject<SceneRegistry>();

                if (sceneRegistry == null)
                {
                    Debug.Log($"[Broadcaster]SceneRegistryInstance is null, retrying in {retryDelay} seconds...");

                    yield return new WaitForSeconds(retryDelay);

                    continue;
                }

                SceneRegistryInstance = sceneRegistry;
            }

            if (SceneRegistryInstance == null)
            {
                Debug.Log("[Broadcaster]Failed to find SceneRegistryInstance after multiple attempts.");

                yield break;
            }

            for (int i = 0; i < retryLimit; i++)
            {
               if (SceneRegistryInstance.GetRegisteredGameObjects().Count == 0)
                {
                    Debug.Log($"[Broadcaster]No registered GameObjects found, retrying in {retryDelay} seconds...");
                    yield return new WaitForSeconds(retryDelay);
                    continue;
                }

                break;
            }

            Broadcast(SceneUtils);
        }
    }
}
