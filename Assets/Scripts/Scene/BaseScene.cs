using Assets.Scripts.BaseScene.Interface;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.BaseScene
{
    public class BaseScene : MonoBehaviour, IScene
    {
        private SceneUtils SceneUtils { get; set; }

        public static BaseScene BaseSceneReference { get; set; }

        private void Start()
        {
            if (BaseSceneReference == null)
            {
                BaseSceneReference = GetComponent<BaseScene>();
            }
        }

        public async Task<SceneUtils> GetSceneUtilsAsync(int retry = 5, int delay = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                if (SceneUtils == null)
                {
                    await Task.Delay(delay);
                }else
                {
                    break;
                }
            }

            return SceneUtils;
        }

        public IEnumerator WaitForSceneUtils(int retry = 5, int delay = 3)
        {
            for (int i = 0; i < retry; i++)
            {
                if (SceneUtils == null)
                {
                    Debug.Log("SceneUtils is null, waiting for it to be assigned...");

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
        //check for null reference and assign BaseSceneReference if null
        public BaseScene BaseScene { get 
            {  
                if (BaseScene.BaseSceneReference == null) 
                {
                    Debug.Log("BaseSceneReference is null, assigning it now...");
                    BaseScene.BaseSceneReference = GetComponent<BaseScene>();
                    Debug.Log($"BaseSceneReference.BaseSceneReference : {BaseScene.BaseSceneReference}");
                }

                return BaseScene.BaseSceneReference;
             } 
        }
    }

    public class StateMachineScene : StateMachineBehaviour
    {
        //check for null reference and assign BaseSceneReference if null
        public BaseScene BaseScene { get; set; } = BaseScene.BaseSceneReference;
    }

    public class ScriptableObjectScene : ScriptableObject
    {
        //check for null reference and assign BaseSceneReference if null
        public BaseScene BaseScene { get; set; } = BaseScene.BaseSceneReference;
    }
}
