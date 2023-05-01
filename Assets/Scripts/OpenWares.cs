using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWares : MonoBehaviour
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] Interactable checkingDialogue;
    [SerializeField] GameObject WaresPanel;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Interactable.MultipleDialogues[checkingDialogue.WizardPlayerConvo])
        {
            MagicCircle.SetActive(true);
        }

  
    }
    private void OnMouseDown()
    {
        WaresPanel.SetActive(true);
    }
}
