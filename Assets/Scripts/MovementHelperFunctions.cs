public static class MovementHelperFunctions
{
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