using NUnit.Framework;
using System.Threading.Tasks;

public class Helper
{
    private const int EMPTY_STRING_ARRAY_SIZE = 0;
    public static Task<string[]> SplitStringOnSeparator(string text, string separator)
    {
        string[] separatedText = text.Split(separator); 
        if(separatedText.Length > 0 )
        {
            return Task.FromResult(separatedText);
        }

        return Task.FromResult(new string[EMPTY_STRING_ARRAY_SIZE]);
    }
}