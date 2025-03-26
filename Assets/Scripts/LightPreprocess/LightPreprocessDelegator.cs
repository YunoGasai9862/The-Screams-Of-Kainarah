using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegator: BaseDelegatorEnhanced<ILightPreprocess>
{
    //ensure this is in the base class + you fill this in the base class
    //then update the logic (already using reflection)
    private List<SubjectAttribute> observersWithSubjectAttributes = new List<SubjectAttribute>(); 
    private void OnEnable()
    {
        SubjectsDict = new Dictionary<string, SubjectNotifier<IObserverEnhanced<ILightPreprocess>>>();

    }
    private async void Start()
    {
        observersWithSubjectAttributes = await Helper.GetGameObjectsWithCustomAttribute<SubjectAttribute>();
    }
}