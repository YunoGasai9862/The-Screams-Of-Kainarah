using Annotations.Enums;
using Assets.Scripts.Broadcaster.Interface;
using Assets.Scripts.Polling.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Broadcaster
{
    [AssetAttribute(Asset.MONOBEHAVIOR, "Broadcaster")]
    public class Broadcaster : BaseScene.MonoBehaviorScene, IBroadcaster, IPoller
    {
        private SceneRegistry SceneRegistryInstance { get; set; }

        private SceneUtils SceneUtils { get; set; }

        private void Awake()
        {
            SceneUtils = FindFirstObjectByType<SceneUtils>();

            Debug.Log($"[Broadcaster]SceneUtilsInstance: {SceneUtils}");

            //use IEnumerator and wait!
            StartCoroutine(Broadcast(SceneUtils, 5));
        }

        public void Broadcast<T>(T value)
        {
            foreach (KeyValuePair<int, GameObject> item in SceneRegistryInstance.GetRegisteredGameObjects())
            {
                item.Value.GetComponent<BaseScene.MonoBehaviorScene>().BaseScene.Broadcast(value);
            }
        }

        public IEnumerator Poll(int pollingIntervalInSeconds)
        {
            Broadcast(SceneUtils);

            yield return new WaitForSeconds(pollingIntervalInSeconds);
        }

        private IEnumerator Broadcast(SceneUtils sceneUtils, int retryLimit = 3, int retryDelay = 3)
        {
            if (retryLimit == 0)
            {
                yield return null;
            }

            SceneRegistry sceneRegistry = sceneUtils.FindObject<SceneRegistry>();

            if (sceneRegistry == null)
            {
                Debug.Log($"[Broadcaster]SceneRegistryInstance is null, retrying in {retryDelay} seconds...");

                yield return new WaitForSeconds(retryDelay);

                Broadcast(sceneUtils, retryLimit - 1, retryDelay);
            }

            SceneRegistryInstance = sceneRegistry;

            Broadcast(SceneUtils);
        }
    }
}
