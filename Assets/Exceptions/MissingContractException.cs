using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Exceptions
{
    public class MissingContractException: BaseException
    {
        public MissingContractException(string exceptionMessage): base(exceptionMessage)
        {

        }
    }
}
