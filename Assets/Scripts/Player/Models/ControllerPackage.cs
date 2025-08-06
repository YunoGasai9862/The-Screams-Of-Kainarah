using System;

public class ControllerPackage<EXECUTIONSTATE, VALUE> where EXECUTIONSTATE : Enum
{
   public EXECUTIONSTATE ExecutionState { get; set; }
    public VALUE Value { get; set; }
}