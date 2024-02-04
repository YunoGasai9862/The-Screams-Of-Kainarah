using UnityEngine.Events;
using UnityEngine;
public class AddInventoryItemEvent: UnityEvent<Sprite, string>
{
    private Sprite _sprite;
    private string _item;
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public string Item { get => _item; set => _item = value; }

    public AddInventoryItemEvent(Sprite sprite, string itemTag)
    {
        Sprite = sprite;
        Item = itemTag;
    }
    public AddInventoryItemEvent() { Sprite = null; Item = string.Empty; }

}