using Assets.Scripts.BaseScene;
using UnityEngine;

public class ControlPanelTrigger : MonoBehaviorScene
{
    [SerializeField] GameObject currentSettingsPanel;
    [SerializeField] GameObject controlPanel;
    private OpenClose _oc;
    void Start()
    {
        _oc = new OpenClose(currentSettingsPanel, controlPanel);
    }

    // Update is called once per frame
    public void OpenControlPanel()
    {
        _oc.ToggleSecondPanelOn();
    }

    public void CloseControlPanel()
    {
        _oc.ToggleFirstPanelOn();
    }
}
