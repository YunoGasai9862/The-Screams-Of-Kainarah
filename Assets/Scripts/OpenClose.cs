using UnityEngine;

public class OpenClose
{
    private GameObject FirstPanel;
    private GameObject SecondPanel;


    public OpenClose(GameObject _fp, GameObject _sp)
    {
        this.FirstPanel = _fp;
        this.SecondPanel = _sp;
    }

    public void ToggleFirstPanelOn()
    {
        this.FirstPanel.SetActive(true);
        this.SecondPanel.SetActive(false);
    }

    public void ToggleSecondPanelOn()
    {
        this.SecondPanel.SetActive(true);
        this.FirstPanel.SetActive(false);
    }
}
