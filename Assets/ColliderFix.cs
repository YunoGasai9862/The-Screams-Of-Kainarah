using UnityEngine;

public class ColliderFix : MonoBehaviour
{
    private SpriteRenderer _sr;
    private BoxCollider2D _collider;
    private bool _isFixed = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
        _collider = GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        if ((CheckFlip(ref _sr) && !_isFixed))
        {
            FixColliderOffset(ref _collider);
        }

        if (!CheckFlip(ref _sr) && _isFixed)
        {
            FixColliderOffset(ref _collider);
        }
    }

    public bool CheckFlip(ref SpriteRenderer _sr)
    {
        return _sr.flipX;
    }

    public void FixColliderOffset(ref BoxCollider2D bc2d)
    {
        float temp = bc2d.offset.x;
        temp *= -1;
        bc2d.offset = new Vector2(temp, bc2d.offset.y);
        _isFixed = !_isFixed;
    }



}
