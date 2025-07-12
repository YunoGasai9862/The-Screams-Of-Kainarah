using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngineInternal;
public class OpenWares : MonoBehaviour, IObserver<GenericState<GameState>>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] GameStateEvent gameStateEvent;
    private GenericState<GameState> CurrentGameState { get; set; } = new GenericState<GameState>();

    private void OnMouseDown()
    {
        if (CurrentGameState.State.Equals(GameState.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);

            CurrentGameState.State = GameState.SHOPPING;

            gameStateEvent.Invoke(CurrentGameState);
        }
    }

    public void OnNotify(GenericState<GameState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;

        if (CurrentGameState.Equals(GameState.FREE_MOVEMENT))
        {
            WaresPanel.SetActive(false);
        }
    }

    public Task Notify(bool value)
    {
        if (value)
        {
            MagicCircle.SetActive(true);
        }

        return Task.CompletedTask;
    }
}
