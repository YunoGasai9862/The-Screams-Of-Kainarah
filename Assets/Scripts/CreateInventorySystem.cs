using System;
using System.Collections;
using System.Collections.Generic;
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

    private static List<GameObject> _inventorySlots;
    private static Queue<GameObject> _inventoryCheck;
    private static Queue<GameObject> _inventoryTemp;
    private static int i = 0;
    private static bool _alreadyExist = false;

    private static Dictionary<string, GameObject> _inventoryItemsDict;

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
    public static List<GameObject> InventorySlots { set=> _inventorySlots=value; get=> _inventorySlots;}

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }
    public static List<GameObject> GetInventoryList()
    {
        return _inventorySlots;
    }

    void Start()
    {

        _inventorySlots = new List<GameObject>();
        _inventoryCheck = new Queue<GameObject>();
        _inventoryTemp = new Queue<GameObject>();

        _inventoryItemsDict = new Dictionary<string, GameObject>();
        _ = (startX == 0 && startY == 0 && increment == 0 && decrement == 0) ? _boxes.GenerateInventory(SizeOftheInventory, -250, 150, 100, -50, PanelObject, ScriptTobeAddedForItems, slotTag) :
                    _boxes.GenerateInventory(SizeOftheInventory, startX, startY, increment, decrement, PanelObject, ScriptTobeAddedForItems, slotTag);
    }


    //use Dictionary Logic (Refactor)
    public static async Task<bool> AddToInventorySystem(GameObject gameObject, string tag)
    {
        var itemTemp = new GameObject(tag);
        GameObject ItemBox = null;
        GameObject textBox = InstantiateTextObject("Numerical", "1", 100f, 100f);

         if(_inventoryItemsDict.TryGetValue(tag, out GameObject value))
        {

        }else
        {
            _inventoryItemsDict.Add(tag, itemTemp);
        }

        await DisplayOnInventory(_inventoryItemsDict);

        return true;
    }

    private static async Task DisplayOnInventory(Dictionary<string, GameObject> dict)
    {
        int x = 0;

        foreach(var item in dict)
        {
            await WipeCleanInventory();

            GameObject parentSlot = InventorySlots[x];

            await AddItemToTheSlot(parentSlot, item); //continue tomorrow

            x++;
        }

    }

    private static Task WipeCleanInventory()
    {
        throw new NotImplementedException();
    }

    private static Task AddItemToTheSlot(GameObject parentSlot, KeyValuePair<string, GameObject> item)
    {
        throw new NotImplementedException();
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
            Exchange(ref _inventoryTemp, ref _inventoryCheck);
            Destroy(itemTemp);

        }
        else
        {
            while (_inventorySlots.Count != 0)
            {
                ItemBox = _inventorySlots.Dequeue();
                //find siblng
                _count++;

                if (ItemBox.transform.childCount == 0)
                {
                    _inventorySlots.Enqueue(ItemBox);

                    break;
                }

                _inventorySlots.Enqueue(ItemBox);

            }

            FindCorrectPosition(_count); //brings the inventory position back to its former state
            itemTemp.AddComponent<RectTransform>();
            itemTemp.transform.SetParent(ItemBox.transform, false);
            textBox.transform.SetParent(ItemBox.transform, false);
            Image image = itemTemp.AddComponent<Image>();
            image.sprite = itemTobeAdded;
            itemTemp.tag = Tag;
            i++;
            _inventoryCheck.Enqueue(itemTemp);
            _alreadyExist = true;
        }

        return Task.FromResult(true);

    }

    public static bool CheckPreviousItems(Sprite itemTobeAdded, string Tag)
    {

        while (_inventoryCheck.Count != 0 && _alreadyExist)
        {
            GameObject ExistingInventory = _inventoryCheck.Dequeue();

            if (ExistingInventory == null)
                continue;
                
            _inventoryTemp.Enqueue(ExistingInventory);

            if (ExistingInventory.GetComponent<Image>().sprite == itemTobeAdded || ExistingInventory.CompareTag(Tag))
            {
                Transform Numerical = ExistingInventory.transform.parent.Find("Numerical");
                UpdateNumericalValue(ref Numerical, 1);
                _alreadyExist = true;
                return true;
            }

            if (_inventoryCheck.Count == 0)
            {
                _alreadyExist = false;
            }

        }
        Exchange(ref _inventoryTemp, ref _inventoryCheck);

        return false;

    }
    public static void ExchangeQueueItems(ref Queue<GameObject> queue1, ref Queue<GameObject> queue2)
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
        while (_inventoryCheck.Count != 0)
        {
            GameObject _item = _inventoryCheck.Dequeue();
            if (_item != null && tag == _item.transform.tag)
            {
                _obj = _item;
                _inventoryTemp.Enqueue(_item);
                Exchange(ref _inventoryCheck, ref _inventoryTemp);
                return _obj;
            }
            _inventoryTemp.Enqueue(_item);
        }
        Exchange(ref _inventoryTemp, ref _inventoryCheck);

        return _obj;
    }

    public static void FindCorrectPosition(int Count)
    {
        int Size = _inventorySlots.Count - Count;
        GameObject temp;
        while (Size > 0)
        {
            temp = _inventorySlots.Dequeue();
            _inventorySlots.Enqueue(temp);
            Size--;
        }
    }

    public static void UpdateNumericalValue(ref Transform Numerical, int sign)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) + sign;
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
        while (_inventoryCheck.Count != 0)
        {
            GameObject itemTemp = _inventoryCheck.Dequeue();
            if (itemTemp != item)
            {
                _inventoryTemp.Enqueue(itemTemp);
            }

        }

        ExchangeQueueItems(ref _inventoryCheck, ref _inventoryTemp);
    }
    public static async Task<bool> ReduceItem(GameObject item)
    {
        if (CheckIfNumericalExists(ref item))
        {
            Transform textBox = item.transform; //numerical will always exist

            Transform[] siblings = item.transform.parent.GetComponentsInChildren<Transform>();

            int textBoxValue = int.Parse(textBox.GetComponent<TextMeshProUGUI>().text);

            UpdateNumericalValue(ref textBox, -1);

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
