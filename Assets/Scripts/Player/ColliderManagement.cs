using UnityEngine;

public class ColliderManagement : MonoBehaviour
{
    [SerializeField] public Collider2D _collider;
    private SpriteRenderer _sr;
    private bool _isFixed = false;

    private void Awake()
    {
        _sr = GetComponent<SpriteRenderer>();
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

    public void FixColliderOffset(ref Collider2D collider)
    {
        float temp = collider.offset.x;
        temp *= -1;
        collider.offset = new Vector2(temp, collider.offset.y);
        _isFixed = !_isFixed;
    }



}
