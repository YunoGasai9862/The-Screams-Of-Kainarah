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
        Debug.Log(GetInventoryItemsDict.Count);
        await DisplayOnInventory(GetInventoryItemsDict, InventorySlots);

        return true;
    }

    private static async Task DisplayOnInventory(Dictionary<string, InventoryItem> dict, List<GameObject> slots)
    {
        int x = 0;

        List<InventoryItem> tempItems = new List<InventoryItem>();

        await WipeCleanInventory(slots);

        foreach (var item in dict.Values)
        {
            GameObject inventorySlot = InventorySlots[x];

            item.GetSlotAttachedTo = inventorySlot;
            
            await AddItemToTheSlot(inventorySlot, item);

            tempItems.Add(item);

            x++;
        }

        foreach(var item in tempItems)
        {
            GetInventoryItemsDict[item.GetTag] = item; // uppdate the inventory with modified data
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
            quantityBox.tag = item.GetTag; //the same as the added gameobject

           GameObject inventoryItem = new GameObject(item.GetTag);
           inventoryItem.transform.localScale = new Vector3(getScale, getScale, getScale);
           inventoryItem.transform.SetParent(parentSlot.transform, false);

           Image sprite = inventoryItem.AddComponent<Image>();

           sprite.sprite = item.GetItemSprite; //take the sprite renderer sprite, and project it onto an image for the UI
           inventoryItem.tag = item.GetTag;

        }

        return await Task.FromResult(true);
    }
    public static GameObject GetSlotTheGameObjectIsAttachedTo(string tag)
    {
        if(GetInventoryItemsDict.TryGetValue(tag, out InventoryItem value))
        {
            return value.GetSlotAttachedTo;
        }

        return null;
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


    public static async Task<bool> ReduceQuantity(string tag)
    {
        if (GetInventoryItemsDict.TryGetValue(tag, out InventoryItem value))
        {
            var quantity = value.GetQuantity;
            quantity--; //reduce the quantity

            if (quantity < 1)
                GetInventoryItemsDict.Remove(tag);
            else
                GetInventoryItemsDict[tag].GetQuantity = quantity; //display the new quantity

            await DisplayOnInventory(GetInventoryItemsDict, InventorySlots); //display the new 

            return true;
        }
        return false;
    }

}
