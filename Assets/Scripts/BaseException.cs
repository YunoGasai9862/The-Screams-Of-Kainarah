using System;


public class BaseException : Exception
{
    private string exceptionMessage;

    public BaseException(string exceptionMessage)
    {
        this.exceptionMessage = exceptionMessage;
    }

    public string ExceptionMessage { set => exceptionMessage = value; get => exceptionMessage; }

}
