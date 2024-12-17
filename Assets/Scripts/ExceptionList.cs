using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExceptionList
{
    public class NullException : BaseException
    {
        public NullException(string exceptionMessage) : base(exceptionMessage) //consturctor
        {
        }
    }

    public class MissingScriptException : BaseException
    {
        public MissingScriptException(string exceptionMessage) : base(exceptionMessage)
        {
        }
    }

    public class TypeMistMatchException: BaseException
    {
        public TypeMistMatchException(string exceptionMessage) : base(exceptionMessage)
        {

        }
    }
}
