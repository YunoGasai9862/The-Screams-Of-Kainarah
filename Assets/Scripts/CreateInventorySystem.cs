using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CreateInventorySystem : MonoBehaviour
{
    private const string QUANITY = "QUANTITY";

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

    private static Dictionary<string, InventoryItem> _inventoryItemsDict;

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

    async void Start()
    {

        _inventorySlots = new List<GameObject>();
        _inventoryCheck = new Queue<GameObject>();
        _inventoryTemp = new Queue<GameObject>();

        _inventoryItemsDict = new Dictionary<string, InventoryItem>();
        _ = (startX == 0 && startY == 0 && increment == 0 && decrement == 0) ? _boxes.GenerateInventory(SizeOftheInventory, -250, 150, 100, -50, PanelObject, ScriptTobeAddedForItems, slotTag) :
                    _boxes.GenerateInventory(SizeOftheInventory, startX, startY, increment, decrement, PanelObject, ScriptTobeAddedForItems, slotTag);

        await WipeCleanInventory(InventorySlots);
    }


    //use Dictionary Logic (Refactor)
    public static async Task<bool> AddToInventorySystem(GameObject itemToBeAdded, string tag)
    {
        var itemTemp = new InventoryItem(itemToBeAdded);

         if(_inventoryItemsDict.TryGetValue(tag, out InventoryItem value))
        {
            value.GetQuantity = value.GetQuantity++;

        }else
        {
            _inventoryItemsDict.Add(tag, itemTemp);
        }

        Debug.Log(_inventoryItemsDict.Count);

        await DisplayOnInventory(_inventoryItemsDict, InventorySlots);

        return true;
    }

    private static async Task DisplayOnInventory(Dictionary<string, InventoryItem> dict, List<GameObject> slots)
    {
        int x = 0;

        await WipeCleanInventory(slots);

        foreach (var item in dict.Values)
        {
            GameObject parentSlot = InventorySlots[x];

            await AddItemToTheSlot(parentSlot, item);

            x++;
        }

    }
    private static async Task WipeCleanInventory(List<GameObject> slots)
    {
        foreach(var slot in slots)
        {
            var children = slot.transform.Cast<Transform>().ToList(); //Casts to Transform because it does not directly implements IEnumerator
            children.ForEach(child => Destroy(child.gameObject));
        }

        await Task.FromResult(true);
    }

    private static async Task<bool> AddItemToTheSlot(GameObject parentSlot, InventoryItem item)
    {
        if(item.GetQuantity > 0)
        {
           GameObject quantityBox = InstantiateQuanityBox(QUANITY, item.GetQuantity.ToString(), 100f, 100f);
           quantityBox.transform.SetParent(parentSlot.transform, false);

           GameObject inventoryItem = new GameObject(item.GetInventoryItem.tag);
           Debug.Log(item.GetInventoryItem.tag);
           inventoryItem.transform.localScale = new Vector3(getScale, getScale, getScale);
           inventoryItem.transform.SetParent(parentSlot.transform, false);
            Debug.Log(item.GetInventoryItem.gameObject);

            Image sprite = inventoryItem.AddComponent<Image>();

           sprite.sprite = item.GetInventoryItem.GetComponent<SpriteRenderer>().sprite; //take the sprite renderer sprite, and project it onto an image for the UI
           Debug.Log(sprite);
           inventoryItem.tag = item.GetInventoryItem.tag;
            Debug.Log(item.GetInventoryItem.gameObject);

        }

        return await Task.FromResult(true);
    }

    /**
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
    */
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

    /**
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
    **/

    public static void UpdateNumericalValue(ref Transform Numerical, int sign)
    {
        TextMeshProUGUI _T = Numerical.GetComponent<TextMeshProUGUI>();
        int count = Int32.Parse(_T.text) + sign;
        _T.text = count.ToString("0");
    }
    public static GameObject InstantiateQuanityBox(string textBoxName, string initialCount, float initialXSize, float initialYSize)
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
