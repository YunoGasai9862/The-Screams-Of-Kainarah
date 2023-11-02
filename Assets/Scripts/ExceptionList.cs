using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExceptionList
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

}
