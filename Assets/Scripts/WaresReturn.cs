
using System.Threading;
using UnityEngine;

public class WaresReturn : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameStateEvent gameStateEvent;

    private PanelHandler m_panelHandler;

    void Start()
    {
        m_panelHandler = new PanelHandler(ref panel);
    }

    public void ClosePanel()
    {
        gameStateEvent.Invoke(GameStateConsumer.FREE_MOVEMENT);

        m_panelHandler.ClosePanel();
    }
}
