using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelHandler
{
    private GameObject Panel;

    public PanelHandler(ref GameObject panel)
    {
        this.Panel = panel;
    }

    public void OpenPanel()
    {
        Panel.SetActive(true);
    }

    public void ClosePanel()
    {
        Panel.SetActive(false);
    }


}
