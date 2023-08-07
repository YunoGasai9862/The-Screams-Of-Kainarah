using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GlobalAccessAndGameHelper
{
    public class GameObjectInstantiator
    {
        private readonly GameObject _prefab;
        private GameObject _gameObject;
        public GameObjectInstantiator(GameObject _prefab)
        {
            this._prefab = _prefab;
        }

        public GameObject InstantiateGameObject(Vector3 _gameObjectPosition, Quaternion _rotationType)
        {
            _gameObject = Object.Instantiate(_prefab, _gameObjectPosition, _rotationType);
            return _gameObject;
        }

        public void DestroyGameObject(float time)
        {
            Object.Destroy(_gameObject, time);
        }

        public GameObject getGameObject()
        {
            return _gameObject;
        }

        public void setGameObjectParent(Transform parent)
        {
            _gameObject.transform.parent = parent;
        }
    }

    public static class globalVariablesAccess
    {
        public static bool ISJUMPING;
        public static bool ISATTACKING;
        public static bool ISSLIDING;
        public static bool ISRUNNING;
        public static bool ISWALKING;


        public static bool boolConditionAndTester(params bool[] boolsToCheckAgainst)
        {
            if (boolsToCheckAgainst.Length == 0)
            {
                return false;
            }

            bool finalBoolValue = true; //initial set to true

            foreach (bool perCondition in boolsToCheckAgainst)
            {
                finalBoolValue = finalBoolValue && perCondition;
            }

            return finalBoolValue;
        }

        public static bool boolConditionOrTester(params bool[] boolsToCheckAgainst)
        {
            if (boolsToCheckAgainst.Length == 0)
            {
                return false;
            }

            bool finalBoolValue = true; //initial set to true

            foreach (bool perCondition in boolsToCheckAgainst)
            {
                finalBoolValue = finalBoolValue || perCondition;
            }

            return finalBoolValue;
        }

        public static void setSliding(bool value)
        {
            ISSLIDING = value;
        }

        public static void setAttacking(bool value)
        {
            ISATTACKING = value;
        }

        public static void initializeAllVariablesTo(params bool[] boolsToInitialize)
        {
            if (boolsToInitialize.Length == 0)
            {
                return;
            }

            for (int i = 0; i < boolsToInitialize.Length; i++)
            {
                boolsToInitialize[i] = false;
            }
        }


    }

    public enum GameMusicState
    {

        BACKGROUNDMUSIC = 0, BOSSMUSIC = 1, PICKUP = 2, STOP = 3
    }

    public static class HelperFunctions
    {
        public static float CalculateScreenWidth(Camera _mainCamera)
        {
            float aspectRatio = _mainCamera.aspect;
            return aspectRatio * _mainCamera.orthographicSize;
        }

        public static IEnumerator TuneDownIntensityToZero(Light2D _light)
        {
            while (_light.intensity > 0f)
            {
                _light.intensity -= 10 * Time.deltaTime;

                Debug.Log(_light.intensity);

                yield return new WaitForSeconds(.1f);
            }

        }

    }

}
