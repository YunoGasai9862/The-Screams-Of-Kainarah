
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
    [SerializeField] GameObject Panel;
    private PanelHandler panelHandler;
    void Start()
    {
        panelHandler = new PanelHandler(ref Panel);
    }
    public void ClosePanel()
    {
        OpenWares.Buying = false;
        panelHandler.ClosePanel();
    }

}
