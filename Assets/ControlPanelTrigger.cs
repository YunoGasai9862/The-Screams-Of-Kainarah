using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPanelTrigger : MonoBehaviour
{
    [SerializeField] GameObject currentSettingsPanel;
    [SerializeField] GameObject ControlPanel;
    private OpenClose OC;
    void Start()
    {
        OC = new OpenClose(currentSettingsPanel, ControlPanel);
    }

    // Update is called once per frame
     public void OpenControlPanel()
    {
        OC.ToggleSecondPanelOn();
    }

    public void CloseControlPanel()
    {
        OC.ToggleFirstPanelOn();
    }
}
