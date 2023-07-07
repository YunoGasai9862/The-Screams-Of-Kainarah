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

    public static void setSliding(bool value)
    {
        ISSLIDING = value;
    }

    public static void setAttacking(bool value)
    {
        ISATTACKING = value;
    }

    public static void initializeAllVariablesTo(params bool[] boolsToInitialize)
    {
        if (boolsToInitialize.Length == 0)
        {
            return;
        }

        for (int i = 0; i < boolsToInitialize.Length; i++)
        {
            boolsToInitialize[i] = false;
        }
    }

}
