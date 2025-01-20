using NUnit.Framework;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Helper
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
        Debug.Log($"Waiting for the variable to become non null : {variable}");
        yield return new WaitUntil(() => variable != null);
    }
}