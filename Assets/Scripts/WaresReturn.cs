
using System.Threading;
using UnityEngine;

public class WaresReturn : MonoBehaviour, IStateBundle
{
    [SerializeField] GameObject panel;
    [SerializeField] GameStateEvent gameStateEvent;

    private PanelHandler m_panelHandler;

    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    void Start()
    {
        m_panelHandler = new PanelHandler(ref panel);
    }

    public void ClosePanel()
    {
        CurrentGameState.StateBundle.GameState = new State<GameState>()
        {
            CurrentState = GameState.FREE_MOVEMENT,
            IsConcluded = false
        };

        gameStateEvent.Invoke(CurrentGameState);

        m_panelHandler.ClosePanel();
    }
}
