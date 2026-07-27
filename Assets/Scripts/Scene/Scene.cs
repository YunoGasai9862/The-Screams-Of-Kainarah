using Assets.Scripts.Scene.Interface;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Scene
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
                    await Task.Delay(delay);

                    return await GetSceneUtilsAsync(i, delay);
                }

                return SceneUtils;
            }

            return null;
        }

        public IEnumerator WaitForSceneUtils(int retry = 5, int delay = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                if (SceneUtils == null)
                {
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

    public class Scene : MonoBehaviour
    {
        public BaseScene BaseScene { get; set; } = new BaseScene();
    }

    public class StateMachineScene : StateMachineBehaviour
    {
        public BaseScene BaseScene { get; set; } = new BaseScene();
    }

    public class ScriptableObjectScene : ScriptableObject
    {
        public BaseScene BaseScene { get; set; } = new BaseScene();
    }
}
