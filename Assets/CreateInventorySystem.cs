using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class CreateInventorySystem : MonoBehaviour
{
    [SerializeField] GameObject PanelObject;
    [SerializeField] GameObject InventoryBox;

    [Space]
    [Header("Make sure the size doesnt exceed the screen")]
    [SerializeField] int SizeOftheInventory;

    private RectTransform _spriteLocation;

    

    void Start()
    {
        _spriteLocation=PanelObject.GetComponent<RectTransform>();

        StartCoroutine(GenerateInventory(SizeOftheInventory));
    }



    IEnumerator GenerateInventory(int Size)
    {
        int increment = -250;
        int decrement = 150;

        for (int i=0; i<Size; i++)
        {
            for(int j=0; j<Size; j++)
            {
                Debug.Log(_spriteLocation.position);
                Vector3 IncrementalSize = new Vector3(increment, decrement);
                GameObject _temp= Instantiate(InventoryBox, IncrementalSize, Quaternion.identity);
                _temp.transform.SetParent(PanelObject.transform,false);
                increment += 100;

            }
            decrement -= 50;
            increment = -250;
        }


        yield return null;
    }

}
