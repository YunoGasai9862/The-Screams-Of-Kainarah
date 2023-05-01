using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{
        [SerializeField] GameObject Panel;
      private PanelHandler panelHandler;
    void Start()
    {
        panelHandler = new PanelHandler(ref Panel);
    }

    // Update is called once per frame
    public void ClosePanel()
    {
        panelHandler.ClosePanel();
    }
}
