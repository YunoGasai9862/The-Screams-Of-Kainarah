using Assets.Scripts.Scene.Interface;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Scene
{
    public class BaseScene : IScene
    {
        private SceneUtils SceneUtils { get; set; }

        public async Task<SceneUtils> GetSceneUtilsAsync(int retry, int delay = 3)
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
        
        public SceneUtils GetSceneUtils()
        {
            return SceneUtils;
        }
    }

    public class Scene : MonoBehaviour
    {
        protected BaseScene BaseScene { get; set; } = new BaseScene();
    }

    public class StateMachineScene : StateMachineBehaviour
    {
        protected BaseScene BaseScene { get; set; } = new BaseScene();
    }

    public class ScriptableObjectScene : ScriptableObject
    {
        protected BaseScene BaseScene { get; set; } = new BaseScene();
    }
}
