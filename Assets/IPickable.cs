using UnityEngine;

public class IPickable
{
    private Collider2D _collider;

    public IPickable(ref Collider2D collider)
    {
        _collider = collider;
    }
    public Collider2D Collider
    {

        set { _collider = value; }
        get { return _collider; }
    }

    public void DestroyPickableFromScene()
    {
        Object.Destroy(_collider.gameObject);
    }

    public void AddToInventory()
    {
        CreateInventorySystem.AddToInventory(_collider.gameObject.GetComponent<SpriteRenderer>().sprite, _collider.gameObject.tag);

    }

}
