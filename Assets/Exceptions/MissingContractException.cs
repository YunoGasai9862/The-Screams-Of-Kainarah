using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Exceptions
{
    public class MissingContextException : BaseException
    {
        public MissingContextException(string exceptionMessage): base(exceptionMessage)
        {

        }
    }
}
