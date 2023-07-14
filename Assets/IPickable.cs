using UnityEngine;

public abstract class IPickable
{
    public abstract Collider2D Collider { get; }
    public abstract void DestroyPickable();
}
