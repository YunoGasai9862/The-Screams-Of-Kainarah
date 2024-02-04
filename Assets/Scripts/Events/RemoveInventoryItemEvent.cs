using UnityEngine.Events;

public class RemoveInventoryItemEvent : UnityEvent<string>
{
    private string _item;
    public string Item { get => _item; set => _item = value; }

    public RemoveInventoryItemEvent(string item)
    {
        Item = item;
    }
    public RemoveInventoryItemEvent() { Item = string.Empty; }

}