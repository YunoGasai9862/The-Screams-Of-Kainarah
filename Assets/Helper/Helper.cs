#nullable enable
using Annotations.Enums;
using Assets.Exceptions;
using ObserverPattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Helper: MonoBehaviour
{
    public static Task<string[]> SplitStringOnSeparator(string text, string separator)
    {
        const int EMPTY_STRING_ARRAY_SIZE = 0;
        string[] separatedText = text.Split(separator); 
        if(separatedText.Length > 0 )
        {
            return Task.FromResult(separatedText);
        }

        return Task.FromResult(new string[EMPTY_STRING_ARRAY_SIZE]);
    }

    public static IEnumerator WaitUntilVariableIsNonNull<T>(T variable)
    {
        yield return new WaitUntil(() => variable != null);
    }

    public static IEnumerator Wait(int waitLimitInSeconds)
    {
        yield return new WaitForSeconds(waitLimitInSeconds);
    }

    public static async Task<T> GetDelegator<T>(int retryLimit = 3, int waitLimitInSeconds = 6) where T : UnityEngine.Object
    {
        for (int i = 0; i < retryLimit; i++)
        {
            T delegator = FindObject<T>();

            Debug.Log($"Delegator: {delegator}, Type: {typeof(T).Name}");

            if (delegator == null)
            {
                await Task.Delay(waitLimitInSeconds * 1000);

                continue;
            }

            return delegator;
        }

        throw new DelegatorNotFoundException($" {typeof(T).Name} Not Found in the Scene");
    }

    public static IEnumerator GetDelegator<T>(Action<T> callback, int retryLimit = 3, int waitLimitInSeconds = 6) where T : UnityEngine.Object
    {
        for (int i = 0; i < retryLimit; i++)
        {
            T delegator = FindObject<T>();

            Debug.Log($"Delegator: {delegator}, Type: {typeof(T).Name}");

            if (delegator == null)
            {
                yield return new WaitForSeconds(waitLimitInSeconds);

                continue;
            }

            callback.Invoke(delegator);
        }

        throw new DelegatorNotFoundException($" {typeof(T).Name} Not Found in the Scene");
    }

    public static T GetFromEntityPoolManager<T>(EntityPoolManager entityPoolManager, string key) where T : ScriptableObject
    {
        List<EntityPool> entityPools = entityPoolManager.GetPooledEntity(key);

        if (entityPools.Count == 0)
        {
            Debug.LogError($"No {key} Scriptable Object found in the Entity Pool Manager. Please add one during the preloading process!");

            return null;
        }

        return (T)entityPools[0].Entity;
    }

    public static async Task<T> GetCustomEvent<T>(int retryLimit = 3, int waitLimitInSeconds = 3) where T : UnityEngine.Object
    {
        for (int i = 0; i < retryLimit; i++)
        {
            T customEvent = FindObject<T>();

            if (customEvent == null)
            {
                await Task.Delay(waitLimitInSeconds * 1000);

                continue;
            }

            return customEvent;
        }

        throw new CustomEventNotFoundException($" {typeof(T).Name} Not Found in the Scene");
    }

    public static dynamic Convert(AnimatorControllerParameterType type, dynamic value)
    {
        switch (type)
        {
            case AnimatorControllerParameterType.Bool:
                if (!(value is bool))
                {
                    throw new ApplicationException("Value is not of bool type, but was specified as bool!" + $" value: {value}");
                }
                return (bool) value;
            case AnimatorControllerParameterType.Int:
                if (!(value is int))
                {
                    throw new ApplicationException("Value is not of int type, but was specified as int!" + $" value: {value}");
                }
                return (int) value;
            case AnimatorControllerParameterType.Float:
                if (!(value is float))
                {
                    throw new ApplicationException("Value is not of float type, but was specified as float!" + $" value: {value}");
                }
                return (float) value;
        }
        return null;
    }

    public static List<T> GetAttribute<T>(List<Type> types, List<Type> genericType = null, List<Type> nonGenericType = null) where T : Attribute
    {
        if (genericType == null && nonGenericType == null)
        {
            throw new MissingArgumentException($"One of them must be provided : genericInterfaceTypes or nonGenericInterfaceTyles!");
        }

        List<T> foundAttributes = new List<T>();

        foreach (Type type in types)
        {
            List<T> attributes = type.GetCustomAttributes<T>().ToList();

            if (attributes == null || attributes.Count == 0)
            {
                Debug.Log($"No custom attribute found for type: {type.FullName}");

                continue;
            }

            string joinedGenericType = string.Join<Type>(",", genericType?.ToArray());

            string joinedNonGenericType = string.Join<Type>(",", nonGenericType?.ToArray());

            Debug.Log($"Custom attributes found for type: {type.FullName} - Count: {attributes.Count} - joinedGenericInterfaceTypes: {joinedGenericType} - joinedNonGenericInterfaceTypes: {joinedNonGenericType} - Total Interfaces: {type.GetInterfaces().Count()}");

            if (genericType!=null && !type.GetInterfaces().Any(interf => genericType.Any(possibleInterfaceType => interf.IsGenericType && possibleInterfaceType.GetGenericTypeDefinition() == interf.GetGenericTypeDefinition())))
            {
                throw new MissingContractException($"The underlying type must implement one of the interfaces: {joinedGenericType}!");
            }

            if (nonGenericType != null && !type.GetInterfaces().Any(interf => nonGenericType.Any(possibleInterfaceType => possibleInterfaceType == interf)))
            {
                throw new MissingContractException($"The underlying type must implement one of the interfaces: {joinedNonGenericType}!");
            }

            attributes.ForEach(attribute =>
            {
                Debug.Log($"Adding: {attribute}");
                foundAttributes.Add(attribute);
            });
        }

        Debug.Log($"Total attributes found: {foundAttributes.Count} for the attribute type: {typeof(T).Name}");

        return foundAttributes;
    }

    public static async Task<T> FindObjectAsync<T>(Type gameObjectType, int retryLimit = 3, int waitLimitInSeconds = 3) where T: class
    {
        for(int i = 0; i < retryLimit; i++)
        {
            GameObject? gameObject = FindFirstObjectByType(gameObjectType) as GameObject;

            if (gameObject == null)
            {
                Debug.Log($"GameObject - {gameObjectType} not found, retrying...");

                await Task.Delay(waitLimitInSeconds * 1000);

                continue;
            }

            if (!(gameObject is T))
            {
                throw new ApplicationException($" {gameObjectType.Name} Does not Implement {typeof(T).Name}");
            }

            return gameObject as T;
        }

        throw new ApplicationException($" {gameObjectType.Name} Not Found in the Scene");
    }

    public static bool IsInterfacePresent(GameObject gameObject, Type typeToSearch)
    {
        return typeToSearch.IsAssignableFrom(gameObject.GetType());
    }

    public static async Task<TYPE> FindReceiver<TYPE, IMPLEMENTATION>(int retryLimit = 3, int waitLimitInSeconds = 3) where TYPE: MonoBehaviour
    {

        for (int i = 0; i < retryLimit; i++)
        {
            TYPE receiver = (TYPE)(UnityEngine.Object)FindFirstObjectByType<TYPE>();

            if (receiver == null)
            {
                await Task.Delay(waitLimitInSeconds * 1000);

                continue;
            }

            if (!(receiver is IMPLEMENTATION))
            {
                throw new ApplicationException($" {typeof(TYPE).Name} Does not Implement {typeof(IMPLEMENTATION)}");
            }

            return receiver;
        }

        throw new ReceiverNotFounderException($" {typeof(TYPE).Name} Not Found in the Scene");
    }

    public static TYPE FindObject<TYPE>() where TYPE : UnityEngine.Object
    {
        return (TYPE)(UnityEngine.Object)FindFirstObjectByType<TYPE>();
    }


    public static Task<int> PlayerFlipped(Transform transform)
    {
        return transform.localScale.x < 0 ? Task.FromResult(-1) : Task.FromResult(1);
    }

    public static async Task<List<T>> GetGameObjectsWithCustomAttributes<T>() where T: System.Attribute
    {
        List<T> objectsWithCustomAttributes = new List<T>();

        System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach(System.Type type in types)
        {
            List<T> customAttributes = type.GetCustomAttributes<T>().ToList();

            if (customAttributes.Count == 0)
            {
                continue;
            }

            objectsWithCustomAttributes.AddRange(customAttributes);
        }

        return objectsWithCustomAttributes;
    }

    public static bool DoesFileExist(string path)
    {
        if (path == null)
        {
            throw new ApplicationException("File path is missing!");
        }

        return new FileInfo(path).Exists;
    }

    public static bool IsSubjectNull<T>(Subject<T> subject)
    {
        return subject == null || subject.ISubject == null;
    }

    public static bool IsObjectNull(System.Object obj)
    {
        return obj == null;
    }

    public static bool AreObjectsNull(List<UnityEngine.Object> objects)
    {
        foreach (UnityEngine.Object obj in objects)
        {
            if (IsObjectNull(obj))
            {
                return true;
            }
        }

        return false;
    }

    public static float GetSecondsFromMilliSeconds(int milliSeconds)
    {
        return milliSeconds / 1000.0f;
    }

    public static ObserverContext BuildNotificationContext(GameObject gameObject, Type subjectType, Type entityType)
    {
        return new ObserverContext()
        {
            Instance = gameObject,
            SubjectType = subjectType,
            EntityType = entityType
        };
    }
    public static ObserverContext<T> BuildNotificationContext<T>(GameObject gameObject, Type subjectType, Type entityType)
    {
        return new ObserverContext<T>()
        {
            Instance = gameObject,
            SubjectType = subjectType,
            EntityType = entityType
        };
    }

    public static void ValidateLightSourcePresence(Light2D light2D)
    {
        if (light2D == null)
        {
            throw new ApplicationException("LightSource is not Present!");
        }
    }

    public static float CalculateScreenWidth(Camera _mainCamera)
    {
        return _mainCamera.aspect * _mainCamera.orthographicSize;
    }

    public static IEnumerator TuneDownIntensityToZero(Light2D _light)
    {
        while (_light.intensity > 0f)
        {
            _light.intensity -= 10 * Time.deltaTime;

            yield return new WaitForSeconds(.1f);
        }

    }
    public static Vector2 FlipTheObjectToFaceParent(ref SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offsetX)
    {
        Vector2 flipped = Vector2.zero;

        if (spriteRenderer.flipX)
        {
            flipped = new Vector2(parentPos.x + offsetX, position.y);
        }
        else
        {
            flipped = new Vector2(parentPos.x - offsetX, position.y);

        }
        return flipped;
    }

    public static bool CheckDistance(Transform firstEntityTransform, Transform secondEntityTransform, float distanceLessThan, float distanceGreaterThan)
    {
        return Vector3.Distance(secondEntityTransform.position, firstEntityTransform.position) <= distanceLessThan && Vector3.Distance(secondEntityTransform.position, firstEntityTransform.position) >= distanceGreaterThan;
    }

    public static bool IsEntityMonobehavior(Asset assetType)
    {
        return assetType.Equals(Asset.MONOBEHAVIOR);
    }

    public static Task SetAsParent(GameObject child, GameObject parent)
    {
        child.transform.parent = parent.transform;

        return Task.CompletedTask;
    }

    public static Task DestroyMultipleGameObjects(List<GameObject> gameObjects, float destroyInSeconds)
    {
        foreach (var gameObject in gameObjects)
        {
            Destroy(gameObject, destroyInSeconds);
        }
        return Task.CompletedTask;
    }

    public static Task<GameObject> InstantiatePrefabAt(Vector3 position, GameObject prefab)
    {
        return Task.FromResult(Instantiate(prefab, position, Quaternion.identity));
    }
}