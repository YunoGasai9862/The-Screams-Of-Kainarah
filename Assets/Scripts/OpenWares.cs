using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PickableItems), EntityType = typeof(OpenWares), ContextType = typeof(bool))]
[Observer(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(GameStateConsumer), EntityType = typeof(OpenWares), ContextType = typeof(GenericStateBundle<GameStateBundle>))]

public class OpenWares : MonoBehaviorScene, INotify<GenericStateBundle<GameStateBundle>>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] GameStateEvent gameStateEvent;
    private GenericStateBundle<GameStateBundle> GameStateBundle { get; set; } = new GenericStateBundle<GameStateBundle>();

    private Delegator Delegator { get; set; }

    private bool IsInventoryOpen { get; set; }

    private async void Start()
    {
       StartCoroutine((await (await GetBaseScene()).GetSceneUtilsAsync()).GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<GenericStateBundle<GameStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(OpenWares),
            SubjectType = typeof(GameStateConsumer)
        }, this);
    }

    private void OnMouseDown()
    {
        if (GameStateBundle.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !IsInventoryOpen)
        {
            WaresPanel.SetActive(true);

            GameStateBundle.StateBundle.GameState.CurrentState = GameState.SHOPPING;

            gameStateEvent.Invoke(GameStateBundle);
        }
    }

    public IEnumerator Notify(bool value)
    {
        IsInventoryOpen = value;

        if (IsInventoryOpen)
        {
            MagicCircle.SetActive(true);
        }

        yield return null;
    }

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        GameStateBundle = value;

        if (GameStateBundle.StateBundle.GameState.CurrentState.Equals(GameState.FREE_MOVEMENT))
        {
            WaresPanel.SetActive(false);
        }

        yield return null;
    }
}
