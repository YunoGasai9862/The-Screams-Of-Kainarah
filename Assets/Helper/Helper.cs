using NUnit.Framework;
using System.Collections;
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

    public static EntityPoolManagerDelegator GetEntityPoolManagerDelegator()
    {
        EntityPoolManagerDelegator delegator = FindFirstObjectByType<EntityPoolManagerDelegator>();

        if (delegator == null)
        {
            throw new DelegatorNotFoundException("Entity Pool Manager Delegator Not Found in the Scene");
        }

        return delegator;
    }
}