using UnityEngine.Events;

public class InventoryManagemenetEvent: UnityEvent<bool>
{
    private bool _shouldReduce;
    private bool _shouldAddToInventory;
    public bool ShouldReduce { get => _shouldReduce; set => _shouldReduce = value; }
    public bool ShouldAddToInventory { get => _shouldAddToInventory; set=> _shouldAddToInventory = value; }
    public InventoryManagemenetEvent()
    {
        this._shouldAddToInventory = false;
        this._shouldReduce = false;
    }

}