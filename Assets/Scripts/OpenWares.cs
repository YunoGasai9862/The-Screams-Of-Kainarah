using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngineInternal;
public class OpenWares : MonoBehaviour, IObserver<GenericStateBundle<GameStateBundle>>, INotify<bool>
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] GameObject WaresPanel;
    [SerializeField] GameStateEvent gameStateEvent;
    private GenericStateBundle<GameStateBundle> GameStateBundle { get; set; } = new GenericStateBundle<GameStateBundle>();

    private void OnMouseDown()
    {
        if (GameStateBundle.StateBundle.GameState.CurrentState.Equals(GameState.DIALOGUE_TAKING_PLACE) && !SceneSingleton.GetInventoryManager().IsPouchOpen)
        {
            WaresPanel.SetActive(true);

            GameStateBundle.StateBundle.GameState.CurrentState = GameState.SHOPPING;

            gameStateEvent.Invoke(GameStateBundle);
        }
    }

    public void OnNotify(GenericStateBundle<GameStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        GameStateBundle = data;

        if (GameStateBundle.StateBundle.GameState.CurrentState.Equals(GameState.FREE_MOVEMENT))
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
