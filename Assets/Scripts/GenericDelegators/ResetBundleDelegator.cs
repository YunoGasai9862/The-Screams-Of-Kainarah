using Assets.Scripts.Models.Reset;
using System.Collections.Generic;

namespace Assets.Scripts.GenericDelegators
{
	public class ResetBundleDelegator: BaseDelegator<ResetBundle>
	{
        private void OnEnable()
        {
            SubjectsDict = new Dictionary<string, Dictionary<string, Subject<ResetBundle>>>();

            SubjectObserversDict = new Dictionary<string, List<Association<ResetBundle>>>();
        }
    }
}