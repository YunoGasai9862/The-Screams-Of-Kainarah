using System.Collections.Generic;
using System.Threading;
using UnityEngine;
public class LightPackageDelegator: BaseDelegator<LightPackage>
{
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<LightPackage>>>();
    }
}