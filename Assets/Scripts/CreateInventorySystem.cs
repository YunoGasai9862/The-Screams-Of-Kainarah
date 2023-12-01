using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateInventorySystem : MonoBehaviour
{
    [SerializeField] GameObject PanelObject;
    [SerializeField] string ScriptTobeAddedForItems;
    [SerializeField] int SizeOftheInventory = 6;
    [SerializeField] GenerateBoxes _boxes;
    [SerializeField] string slotTag;

    private static Queue<GameObject> inventoryList;
    private static Queue<GameObject> inventoryCheck;
    private static Queue<GameObject> inventoryTemp;
    private static int i = 0;
    private static bool _alreadyExist = false;

    [Space]
    [Header("Enter the start value of the grid: Default values=> -250, 150")]
    [SerializeField] int startX, startY;


    [Space]
    [Header("Enter the decrement and Increment Sizes: Default values=> 100, 50")]
    [SerializeField] int increment, decrement;

    [Space]
    [Header("Enter scale for each slot")]
    [SerializeField] float scale;

    private static float getScale { get => instance.scale; set => instance.scale = value; }
    private static CreateInventorySystem instance;
    public static Queue<GameObject> InventoryList { set=> inventoryList=value; get=>inventoryList;}

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public static Queue<GameObject> GetInventoryList()
    {
        return inventoryList;
    }

    void Start()
    {

        inventoryList = new Queue<GameObject>();
        inventoryCheck = new Queue<GameObject>();
        inventoryTemp = new Queue<GameObject>();
        _ = (startX == 0 && startY == 0 && increment == 0 && decrement == 0) ? _boxes.GenerateInventory(SizeOftheInventory, -250, 150, 100, -50, PanelObject, ScriptTobeAddedForItems, slotTag) :
                    _boxes.GenerateInventory(SizeOftheInventory, startX, startY, increment, decrement, PanelObject, ScriptTobeAddedForItems, slotTag);
    }


    public static Task<bool> AddToInventory(Sprite itemTobeAdded, string Tag) 
    {
        var itemTemp = new GameObject("Item" + i);
        GameObject ItemBox = null;
        GameObject textBox = InstantiateTextObject("Numerical", "1", 100f, 100f);

        itemTemp.transform.localScale = new Vector3(getScale, getScale, getScale);
        int _count = 0;
        if (CheckPreviousItems(itemTobeAdded, Tag))
        {
            Exchange(ref inventoryTemp, ref inventoryCheck);
            Destroy(itemTemp);

        }
        else
        {
            while (inventoryList.Count != 0)
            {
                ItemBox = inventoryList.Dequeue();
                //find sibling
                _count++;

                if (ItemBox.transform.childCount == 0)
                {
                    inventoryList.Enqueue(ItemBox);

                    break;
                }

                inventoryList.Enqueue(ItemBox);

            }

            FindCorrectPosition(_count); //brings the inventory position back to its former state
            itemTemp.AddComponent<RectTransform>();
            itemTemp.transform.SetParent(ItemBox.transform, false);
            textBox.transform.SetParent(ItemBox.transform, false);
            Image image = itemTemp.AddComponent<Image>();
            image.sprite = itemTobeAdded;
            itemTemp.tag = Tag;
            i++;
            inventoryCheck.Enqueue(itemTemp);
            _alreadyExist = true;
        }

        return Task.FromResult(true);

    }

    public static bool CheckPreviousItems(Sprite itemTobeAdded, string Tag)
    {

        while (inventoryCheck.Count != 0 && _alreadyExist)
        {
            GameObject ExistingInventory = inventoryCheck.Dequeue();
            if (ExistingInventory != null)
                inventoryTemp.Enqueue(ExistingInventory);

            if (ExistingInventory != null && (ExistingInventory.GetComponent<Image>().sprite == itemTobeAdded || ExistingInventory.CompareTag(Tag)))
            {
                Transform Numerical = ExistingInventory.transform.parent.Find("Numerical");
                Increment(ref Numerical);
                _alreadyExist = true;
                return true;

            }

            if (inventoryCheck.Count == 0)
            {
                _alreadyExist = false;
            }

        }

        Exchange(ref inventoryTemp, ref inventoryCheck);
        return false;

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
            if (temp != null)
                queue2.Enqueue(temp);
        }
    }
    public static GameObject CheckForObject(string tag)
    {
        GameObject _obj = null;
        while (inventoryCheck.Count != 0)
        {
            GameObject _item = inventoryCheck.Dequeue();
            if (_item != null && tag == _item.transform.tag)
            {
                _obj = _item;
                inventoryTemp.Enqueue(_item);
                Exchange(ref inventoryCheck, ref inventoryTemp);
                return _obj;
            }
            inventoryTemp.Enqueue(_item);
        }
        Exchange(ref inventoryTemp, ref inventoryCheck);

        return _obj;
    }

    public static void FindCorrectPosition(int Count)
    {
        int Size = inventoryList.Count - Count;
        GameObject temp;
        while (Size > 0)
        {
            temp = inventoryList.Dequeue();
            inventoryList.Enqueue(temp);

            Size--;
        }
    }

    public static void Increment(ref Transform Numerical)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) + 1;
        _T.text = count.ToString("0");
    }

    public static void Decrement(ref Transform Numerical)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) - 1;
        _T.text = count.ToString("0");

    }
    public static GameObject InstantiateTextObject(string textBoxName, string initialCount, float initialXSize, float initialYSize)
    {
        GameObject TextBox = new(textBoxName);
        TextBox.AddComponent<TextMeshProUGUI>();
        TextBox.GetComponent<RectTransform>().sizeDelta = new Vector2(initialXSize, initialYSize);
        TextBox.GetComponent<TextMeshProUGUI>().text = initialCount;
        TextBox.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.BottomRight;
        return TextBox;
    }

    public static void removeItemFromTheList(ref GameObject item)
    {
        while (inventoryCheck.Count != 0)
        {
            GameObject _item = inventoryCheck.Dequeue();
            if (_item != item)
            {
                inventoryTemp.Enqueue(_item);
            }

        }

        TransferTheItemsToQueue(ref inventoryCheck, ref inventoryTemp);
    }
    public static async Task<bool> ReduceItem(GameObject item)
    {
        if (CheckIfNumericalExists(ref item))
        {
            Transform textBox = item.transform; //numerical will always exist

            Transform[] siblings = item.transform.parent.GetComponentsInChildren<Transform>();

            int textBoxValue = int.Parse(textBox.GetComponent<TextMeshProUGUI>().text);

            Decrement(ref textBox);

            if (textBoxValue == 1)
            {
                removeItemFromTheList(ref item);

                for (int i = 0; i < siblings.Length; i++)
                {
                    if (i == 0)
                        continue;
                    else
                        Destroy(siblings[i].gameObject);
                }

            }
        }

        await Task.Delay(TimeSpan.FromSeconds(0));

        return true;

    }

    public static bool CheckIfNumericalExists(ref GameObject item)
    {
        return item.transform.name == "Numerical" || item.transform.Find("Numerical");
    }

}
