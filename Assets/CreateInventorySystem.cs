using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateInventorySystem : MonoBehaviour
{
    [SerializeField] GameObject PanelObject;
    [SerializeField] GameObject InventoryBox;
    [SerializeField] Canvas canvas;
    private static Queue<GameObject> inventoryList;
    private static Queue<GameObject> inventoryCheck;
    private static Queue<GameObject> inventoryTemp;
    private static int i = 0;
    private static bool _alreadyExist=false;
    private int SizeOftheInventory=6;

    [Space]
    [Header("Enter the start value of the grid: Default values=> -250, 150")]
    [SerializeField] int startX, startY;


    [Space]
    [Header("Enter the decrement and Increment Sizes: Default values=> 100, 50")]
    [SerializeField] int increment, decrement;


    private RectTransform _spriteLocation;

    

    void Start()
    {
        inventoryList=new Queue<GameObject>();
        inventoryCheck=new Queue<GameObject>();
        inventoryTemp=new Queue<GameObject>();
        _spriteLocation=PanelObject.GetComponent<RectTransform>();

        if(startX==0 && startY==0 && increment==0 && decrement==0)
            StartCoroutine(GenerateInventory(SizeOftheInventory, -250, 150, 100, -50));
        else
            StartCoroutine(GenerateInventory(SizeOftheInventory, startX, startY, increment,decrement));
    }



    IEnumerator GenerateInventory(int _Size, int _startX, int _startY, int _increment, int _decrement)
    {
        int increment = _startX;
        int decrement = _startY;

        for (int i=0; i<_Size; i++)
        {
            for(int j=0; j<_Size; j++)
            {
                Debug.Log(_spriteLocation.position);
                Vector3 IncrementalSize = new Vector3(increment, decrement);
                GameObject _temp= Instantiate(InventoryBox, IncrementalSize, Quaternion.identity);
                inventoryList.Enqueue(_temp);
                _temp.transform.SetParent(PanelObject.transform,false);
                increment += _increment;

            }
            decrement -= _decrement;
            increment = _startX;
        }


        yield return null;
    }

    public static bool AddToInventory(Sprite itemTobeAdded)
    {
        GameObject _temp = new GameObject("Item" + i);
        _temp.transform.localScale = new Vector3(.35f, .35f, .35f);

        if (inventoryList.Count == 0)
        {
            return false;
        }


        GameObject ItemBox = inventoryList.Dequeue();
        if(inventoryCheck.Count!=0)
        {
            while (inventoryCheck.Count != 0 && !_alreadyExist)
            {
                GameObject ExistingInventory = inventoryCheck.Dequeue();
                if (ExistingInventory.GetComponent<UnityEngine.UI.Image>().sprite == itemTobeAdded)
                {
                    _alreadyExist = true;
                    inventoryTemp.Enqueue(ExistingInventory);
                    break;
                }
                else
                {
                    _alreadyExist = false;

                }


            }
            TransferTheItemsToQueue(inventoryCheck, inventoryTemp);

        }
        
        if(!_alreadyExist)
        {

            RectTransform RT = _temp.AddComponent<RectTransform>();
            _temp.transform.SetParent(ItemBox.transform, false);
            UnityEngine.UI.Image image = _temp.AddComponent<UnityEngine.UI.Image>();
            image.sprite = itemTobeAdded;
            i++;
            inventoryCheck.Enqueue(_temp);

        }

        return true;


    }

    public static void TransferTheItemsToQueue(Queue<GameObject> queue1, Queue<GameObject> queue2)
    {
        if(queue1.Count==0)
        {
            Exchange(queue2, queue1);
        }else
        {
            Exchange(queue1, queue2);
        }
    }

    public static void Exchange(Queue<GameObject> queue1, Queue<GameObject> queue2)
    {
        while(queue1.Count != 0)
        {
            queue2.Enqueue(queue1.Dequeue());
        }
    }

}
