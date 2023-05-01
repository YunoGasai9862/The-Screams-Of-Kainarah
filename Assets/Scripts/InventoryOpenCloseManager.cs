using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpenCloseManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryPanel;

    private bool isOpenInventory=false;
    public void HandleInventory()
    {
        if(!isOpenInventory)
        {
            InventoryPanel.SetActive(true);
            isOpenInventory = true;
        }
        else
        {
            InventoryPanel.SetActive(false);
            isOpenInventory = false;
        }
       
    }

   
}
