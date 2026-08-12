using Assets.Scripts.BaseScene.Interface;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BaseScene
{
    public class BaseScene : MonoBehaviour, IScene
    {
        private SceneUtils SceneUtils { get; set; }

        public async Task<SceneUtils> GetSceneUtilsAsync(int retry = 5, int delay = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                if (SceneUtils == null)
                {
                    Debug.Log($"[BaseScene] SceneUtils is null, waiting for it to be assigned... Attempt {i + 1} of {retry}");
                    await Task.Delay(delay * 1000);
                }else
                {
                    break;
                }
            }

            Debug.Log($"Returning SceneUtils: {SceneUtils}");

            return SceneUtils;
        }

        public IEnumerator WaitForSceneUtils(int retry = 5, int delay = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                if (SceneUtils == null)
                {
                    Debug.Log($"[BaseScene] SceneUtils is null, waiting for it to be assigned... Attempt {i + 1} of {retry}");

                    yield return new WaitForSeconds(delay);

                    yield return StartCoroutine(WaitForSceneUtils(i, delay));
                }
            }
        }

        public SceneUtils GetSceneUtils()
        {
            return SceneUtils;
        }


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

    public class MonoBehaviorScene : MonoBehaviour
    {
        public async Task<BaseScene> GetBaseScene()
        {
            BaseScene scene = null ;

            scene = await FindFirstObjectByTypeAsync<BaseScene>();

            Debug.Log($"Object: {name}, BaseSceneReference.BaseSceneReference : {scene}");

            return scene;
        }

        public BaseScene GetBaseSceneSync()
        {
            BaseScene scene = null;

            scene = GetComponent<BaseScene>();

            Debug.Log($"Object: {name}, BaseSceneReference.BaseSceneReference : {scene}");

            return scene;
        }

        public async Task<T> FindFirstObjectByTypeAsync<T>(int retry = 5, int delay = 3) where T : UnityEngine.Object
        {
            T component = null;

            for (int i = 0; i < retry; i++)
            {

                component = GameObject.FindFirstObjectByType<T>();

                if (component == null)
                {
                    await Task.Delay(delay * 1000);

                    continue;
                }
                else
                {
                    break;
                }
            }

            return component;
        }

    }

    public class StateMachineScene : StateMachineBehaviour
    {
        //think of this!
        public async Task<BaseScene> GetBaseScene()
        {
            BaseScene scene = null;

            scene = await FindFirstObjectByTypeAsync<BaseScene>();

            Debug.Log($"Object: {name}, BaseSceneReference.BaseSceneReference : {scene}");

            return scene;
        }

        public async Task<T> FindFirstObjectByTypeAsync<T>(int retry = 5, int delay = 3) where T : UnityEngine.Object
        {
            T component = null;

            for (int i = 0; i < retry; i++)
            {

                component = GameObject.FindFirstObjectByType<T>();

                if (component == null)
                {
                    await Task.Delay(delay * 1000);

                    continue;
                }
                else
                {
                    break;
                }
            }

            return component;
        }
    }

    public class ScriptableObjectScene : ScriptableObject
    {
        //think of this!
        public async Task<BaseScene> GetBaseScene()
        {
            BaseScene scene = null;

            scene = await FindFirstObjectByTypeAsync<BaseScene>();

            Debug.Log($"Object: {name}, BaseSceneReference.BaseSceneReference : {scene}");

            return scene;
        }

        public async Task<T> FindFirstObjectByTypeAsync<T>(int retry = 5, int delay = 3) where T : UnityEngine.Object
        {
            T component = null;

            for (int i = 0; i < retry; i++)
            {

                component = GameObject.FindFirstObjectByType<T>();

                if (component == null)
                {
                    await Task.Delay(delay * 1000);

                    continue;
                }
                else
                {
                    break;
                }
            }

            return component;
        }
    }
}
