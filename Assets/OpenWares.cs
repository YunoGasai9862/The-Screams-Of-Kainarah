using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWares : MonoBehaviour
{
    [SerializeField] GameObject MagicCircle;
    [SerializeField] Interactable checkingDialogue;
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

        if (Camera.main.WorldToScreenPoint(Input.mousePosition) == transform.position)
        {
            Debug.Log("Yes, clicked");
        }
    }
}
