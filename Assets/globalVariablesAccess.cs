public static class globalVariablesAccess
{
    public static bool ISJUMPING;
    public static bool ISATTACKING;
    public static bool ISSLIDING;
    public static bool ISRUNNING;
    public static bool ISWALKING;


    public static bool boolConditionAndTester(params bool[] boolsToCheckAgainst)
    {
        if (boolsToCheckAgainst.Length == 0)
        {
            return false;
        }

        bool finalBoolValue = true; //initial set to true

        foreach (bool perCondition in boolsToCheckAgainst)
        {
            finalBoolValue = finalBoolValue && perCondition;
        }

        return finalBoolValue;
    }

    public static bool boolConditionOrTester(params bool[] boolsToCheckAgainst)
    {
        if (boolsToCheckAgainst.Length == 0)
        {
            return false;
        }

        bool finalBoolValue = true; //initial set to true

        foreach (bool perCondition in boolsToCheckAgainst)
        {
            finalBoolValue = finalBoolValue || perCondition;
        }

        return finalBoolValue;
    }
}
