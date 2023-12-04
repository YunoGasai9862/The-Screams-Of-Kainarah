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

    private static CreateInventorySystem instance;
    public static List<GameObject> InventorySlots { set=> _inventorySlots=value; get=> _inventorySlots;}
    public static float getScale { get => instance.scale; set => instance.scale = value; }
    public static Dictionary<string, InventoryItem> GetInventoryItemsDict { get => _inventoryItemsDict; set => _inventoryItemsDict = value; }

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

        _inventoryItemsDict = new Dictionary<string, InventoryItem>();
        _ = (startX == 0 && startY == 0 && increment == 0 && decrement == 0) ? _boxes.GenerateInventory(SizeOftheInventory, -250, 150, 100, -50, PanelObject, ScriptTobeAddedForItems, slotTag) :
                    _boxes.GenerateInventory(SizeOftheInventory, startX, startY, increment, decrement, PanelObject, ScriptTobeAddedForItems, slotTag);

    }


    //use Dictionary Logic (Refactor)
    public static async Task<bool> AddToInventorySystem(Sprite itemToBeAdded, string tag)
    {
        var itemTemp = new InventoryItem(itemToBeAdded, tag);

         if(GetInventoryItemsDict.TryGetValue(tag, out InventoryItem value))
        {
            value.GetQuantity = value.GetQuantity++;
            GetInventoryItemsDict[tag] = value;
        }
        else
        {
            GetInventoryItemsDict.Add(tag, itemTemp);
        }

        await DisplayOnInventory(GetInventoryItemsDict, InventorySlots);

        return true;
    }

    private static async Task DisplayOnInventory(Dictionary<string, InventoryItem> dict, List<GameObject> slots)
    {
        int x = 0;

        await WipeCleanInventory(slots);

        foreach (var item in dict.Values)
        {
            GameObject inventorySlot = InventorySlots[x];

            item.GetSlotAttachedTo = inventorySlot;
            
            await AddItemToTheSlot(inventorySlot, item);

            dict[item.GetTag] = item; //update the value

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

           GameObject inventoryItem = new GameObject(item.GetTag);
           inventoryItem.transform.localScale = new Vector3(getScale, getScale, getScale);
           inventoryItem.transform.SetParent(parentSlot.transform, false);

           Image sprite = inventoryItem.AddComponent<Image>();

           sprite.sprite = item.GetItemSprite; //take the sprite renderer sprite, and project it onto an image for the UI
           inventoryItem.tag = item.GetTag;

        }

        return await Task.FromResult(true);
    }
    public static GameObject CheckForObject(string tag)
    {
        GameObject slot = null;
        if(GetInventoryItemsDict.TryGetValue(tag, out InventoryItem value))
        {

        }

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

    public static GameObject InstantiateQuanityBox(string textBoxName, string initialCount, float initialXSize, float initialYSize)
    {
        GameObject textBox = new(textBoxName);
        textBox.AddComponent<TextMeshProUGUI>();
        textBox.GetComponent<RectTransform>().sizeDelta = new Vector2(initialXSize, initialYSize);
        textBox.GetComponent<TextMeshProUGUI>().text = initialCount;
        textBox.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.BottomRight;
        return textBox;
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
