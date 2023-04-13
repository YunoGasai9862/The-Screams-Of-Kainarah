using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpenCloseManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryPanel;


    public void OpenInventory()
    {

        InventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        InventoryPanel.SetActive(false);
    }
}
