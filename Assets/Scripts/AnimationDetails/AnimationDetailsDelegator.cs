using System.Collections.Generic;

public class AnimationDetailsDelegator: BaseDelegator<AnimationDetails>
{
    private void Awake()
    {
        SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<AnimationDetails>>>>();
    }
}