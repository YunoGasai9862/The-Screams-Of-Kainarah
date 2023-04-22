using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class CreateInventorySystem : MonoBehaviour
{
    [SerializeField] GameObject PanelObject;
    [SerializeField] GameObject InventoryBox;
    [SerializeField] Canvas canvas;
    [SerializeField] string ScriptTobeAddedForItems;

    private static Queue<GameObject> inventoryList;
    private static Queue<GameObject> inventoryCheck;
    private static Queue<GameObject> inventoryTemp;
    private static int i = 0;
    private static int _count = 0;
    private static bool _alreadyExist=false;
    private int SizeOftheInventory=6;
    private static GameObject _temp;

    [Space]
    [Header("Enter the start value of the grid: Default values=> -250, 150")]
    [SerializeField] int startX, startY;


    [Space]
    [Header("Enter the decrement and Increment Sizes: Default values=> 100, 50")]
    [SerializeField] int increment, decrement;


    private RectTransform _spriteLocation;

    public static Queue<GameObject> GetInventoryList()
    {
        return inventoryList;
    }

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



    IEnumerator GenerateInventory(int _Size, int _startX, int _startY, int _increment, int _decrement )
    {
        int increment = _startX;
        int decrement = _startY;

        for (int i=0; i<_Size; i++)
        {
            for(int j=0; j<_Size; j++)
            {
                Vector3 IncrementalSize = new Vector3(increment, decrement);
                GameObject _temp= Instantiate(InventoryBox, IncrementalSize, Quaternion.identity);
                _temp.AddComponent(Type.GetType(ScriptTobeAddedForItems)); //adds the script
                _temp.name = ("item" + _count);
             
                inventoryList.Enqueue(_temp);
                _temp.transform.SetParent(PanelObject.transform,false);
                increment += _increment;
                _count++;

            }
            decrement -= _decrement;
            increment = _startX;
        }


        yield return null;
    }

    public static void AddToInventory(Sprite itemTobeAdded, string Tag)  //fix this tomorrow ->Only collectively addds if its the first slot.
    {
         _temp = new GameObject("Item" + i);
        _temp.transform.localScale = new Vector3(.35f, .35f, .35f);
        GameObject ItemBox=null;
        int _count=0;

        //finding empty slot
        while (inventoryList.Count!=0)
        {
            ItemBox= inventoryList.Dequeue();
            _count++;
      

            if (ItemBox.transform.childCount==0)
            {
                inventoryList.Enqueue(ItemBox);

                break;
            }

            inventoryList.Enqueue(ItemBox);

        }
        
        FindCorrectPosition(_count); //brings the inventory position back to its former state

        if (inventoryCheck.Count!=0)  //if already exists, then check through the previously stored items for increment
        {
            CheckPreviousItems(itemTobeAdded, Tag);
        }
        else
        {
            
            _alreadyExist = false;

        }


        if (!_alreadyExist)
        {

            _temp.AddComponent<RectTransform>();
            _temp.transform.SetParent(ItemBox.transform, false);
            Image image = _temp.AddComponent<Image>();
            image.sprite = itemTobeAdded;
            _temp.tag = Tag;
            i++;
            inventoryCheck.Enqueue(_temp);
            _alreadyExist = true;

        }



    
    }

    public static void CheckPreviousItems(Sprite itemTobeAdded, string Tag)
    {
        while (inventoryCheck.Count != 0 && _alreadyExist)
        {
            GameObject ExistingInventory = inventoryCheck.Dequeue();
            inventoryTemp.Enqueue(ExistingInventory);

            GameObject TextBox = InstantiateTextObject();

            if (ExistingInventory.GetComponent<Image>().sprite == itemTobeAdded || ExistingInventory.CompareTag(Tag))
            {
                Transform Numerical = ExistingInventory.transform.parent.Find("Numerical");
                Destroy(_temp);
                if (Numerical == null)
                {
                    TextBox.transform.SetParent(ExistingInventory.transform.parent, false);
                }
                else
                {
                    Increment(Numerical);
                    Destroy(TextBox);
                    
                }
                _alreadyExist = true;
                break;
            }

            if (inventoryCheck.Count == 0)
            {
                Destroy(TextBox);
                _alreadyExist = false;
            }

        }

        TransferTheItemsToQueue(ref inventoryCheck, ref inventoryTemp);


    }

    public static void TransferTheItemsToQueue(ref Queue<GameObject> queue1, ref Queue<GameObject> queue2)
    {
        if (queue1.Count == 0)
        {
            Exchange(ref queue2, ref queue1);
        }
        else
        {
            Exchange(ref queue1, ref queue2);
        }
    }

    public static void Exchange(ref Queue<GameObject> queue1, ref Queue<GameObject> queue2)
    {
        while (queue1.Count != 0)
        {
            GameObject temp = queue1.Dequeue();
            queue2.Enqueue(temp);
        }
    }
    public static void PrintQueue(Queue<GameObject> q)
    {
        for(int i=0; i<q.Count; i++)
        {
            Debug.Log(q.Dequeue().name);
        }
    }


    public static void FindCorrectPosition( int Count)
    {
        int Size = inventoryList.Count - Count;
        GameObject temp;
        while (Size>0)
        {
            temp=inventoryList.Dequeue();
            inventoryList.Enqueue(temp);
         
            Size--;
        }


    }


    public static void Increment(Transform Numerical)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) + 1;
        _T.text = count.ToString("0");
    }

    public static void Decrement(GameObject Numerical)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) -1;
        _T.text = count.ToString("0");
    }
    public static GameObject InstantiateTextObject()
    {
        GameObject TextBox = new GameObject("Numerical");
        TextBox.AddComponent<TextMeshProUGUI>();
        TextBox.GetComponent<RectTransform>().sizeDelta = new Vector2(100f, 100f);
        TextBox.GetComponent<TextMeshProUGUI>().text = "2";
        TextBox.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.BottomRight;

        return TextBox;
    }

    public static void CheckItem(ref GameObject item)
    {
        while(inventoryCheck.Count!=0)
        {
            GameObject _item= inventoryCheck.Dequeue();
            if (_item != item)
            {
                inventoryTemp.Enqueue(item);
            }

        }

        while(inventoryTemp.Count!=0)
        {
            inventoryCheck.Enqueue(inventoryTemp.Dequeue());
        }
    }
    public static void ReduceItem(GameObject item)
    {
            if(item.transform.childCount!=0)
            {

                GameObject TextBox = item.transform.Find("Numerical") ? item.transform.Find("Numerical").gameObject : null;
                if (TextBox != null)
                {

                    Decrement(TextBox);
                }
                else
                {

                    Destroy(TextBox);
                    CheckItem(ref item);
                    Destroy(item.transform.GetChild(0).gameObject);
                }


            }
            else
            {
                  return;
            }

    }

}
