using Assets.Scripts.Models.Reset;
using System.Collections.Generic;

namespace Assets.Scripts.GenericDelegators
{
	public class ResetControllerDelegator: BaseDelegator<ResetBundle>
	{
        private void OnEnable()
        {
            SubjectsDict = new Dictionary<string, Dictionary<string, Subject<IObserver<ResetBundle>>>>();
        }
    }
}