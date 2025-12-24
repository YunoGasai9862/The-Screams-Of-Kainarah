using System;
using System.Collections.Generic;

public class EntityPoolManagerDelegator : BaseDelegator<EntityPoolManager>  
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<EntityPoolManager>>>();
    }
}