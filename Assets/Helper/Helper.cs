using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using static ExceptionList;

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

    public static T GetDelegator<T>() where T: UnityEngine.Object
    {
        T delegator = (T)(UnityEngine.Object) FindFirstObjectByType<T>();

        if (delegator == null)
        {
            throw new DelegatorNotFoundException($" {typeof(T).Name} Not Found in the Scene");
        }

        return delegator;
    }

    public static T GetCustomEvent<T>() where T : UnityEngine.Object
    {
        T customEvent = (T)(UnityEngine.Object)FindFirstObjectByType<T>();

        if (customEvent == null)
        {
            throw new CustomEventNotFoundException($" {typeof(T).Name} Not Found in the Scene");
        }

        return customEvent;
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

    public static bool IsSubjectNull<T>(Subject<IObserver<T>> subject)
    {
        return subject == null || subject.GetSubject() == null;
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

    public static int GetSecondsFromMilliSeconds(int milliSeconds)
    {
        return milliSeconds / 1000;
    }

    public static NotificationContext BuildNotificationContext(string name, string tag, string subjectType)
    {
        return new NotificationContext()
        {
            ObserverName = name,
            ObserverTag = tag,
            SubjectType = subjectType
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