using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.SingletonsAndGlobals.SOManager
{
    [Subject(SubjectType = typeof(ScriptableObjectManager), ContextType = typeof(CheckPoints))]
    public class ScriptableObjectManager: MonoBehaviour, IRequest<CheckPoints>
    {
        [SerializeField]
        private CheckPoints checkPoints;

        private Delegator Delegator { get; set; }

        private async void Awake()
        {
            Delegator = await Helper.GetDelegator<Delegator>();
        }

        public IEnumerator Request()
        {
            yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<CheckPoints>() { EntityType = typeof(ScriptableObjectManager), Data = checkPoints }, this));
        }
    }
}
