using Amazon.Runtime.Internal.Transform;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
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
        T delegator = (T)(Object) FindFirstObjectByType<T>();

        if (delegator == null)
        {
            throw new DelegatorNotFoundException($" {typeof(T).Name} Not Found in the Scene");
        }

        return delegator;
    }

    public static async Task<List<T>> GetGameObjectsWithCustomAttribute<T>() where T: System.Attribute
    {
        List<T> objectsWithCustomAttributes = new List<T>();

        System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();

        foreach(System.Type type in types)
        {
            T customAttribute = type.GetCustomAttribute<T>();

            if (customAttribute == null)
            {
                continue;
            }

            objectsWithCustomAttributes.Add(customAttribute);
        }

        return objectsWithCustomAttributes;
    }

    public static Task<Dictionary<string, List<ObserverSystemAttribute>>> GenerateObserverSystemDict(List<ObserverSystemAttribute> observerSystemAttributes)
    {
        Dictionary<string, List<ObserverSystemAttribute>> observerSystemAttributesDict = new Dictionary<string, List<ObserverSystemAttribute>>();

        foreach (ObserverSystemAttribute attribute in observerSystemAttributes)
        {
            if (observerSystemAttributesDict.ContainsKey(attribute.ObserverType.ToString()))
            {
                observerSystemAttributesDict[attribute.ObserverType.ToString()].Append(attribute);

            } else
            {
                observerSystemAttributesDict.Add(attribute.ObserverType.ToString(), new List<ObserverSystemAttribute>() { attribute });
            }
        }

        return Task.FromResult(observerSystemAttributesDict);
    }
}