using UnityEngine;

public class DaggerController : MonoBehaviour
{

    private float _daggerSpeed = 20f;
    private Rigidbody2D _rb;
    private Animator _anim;
    private float _elapsedTime = 0;
    private bool _checker = true;
    private GameObject _player;
    private SpriteRenderer _daggerRenderer;
    private DaggerOnThrowEvent _onThrowEvent;
    public void Awake()
    {
        _onThrowEvent = new DaggerOnThrowEvent();
    }
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _daggerRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (_checker)
        {
            _elapsedTime += Time.deltaTime;
        }

        if (_elapsedTime > 1f)
        {
            _checker = false;
            _anim.SetBool("HitEnemy", true);
            Destroy(gameObject, .4f);
            _elapsedTime = 0;

        }

        if (IsPlayerFlipped(_player.transform) && _onThrowEvent.DaggerInMotion)
        {
            _daggerRenderer.flipX = true;
            _rb.linearVelocity = new Vector2(-_daggerSpeed, 0);
            _onThrowEvent.DaggerInMotion = false;

        }

        if (!IsPlayerFlipped(_player.transform) && _onThrowEvent.DaggerInMotion)
        {
            _daggerRenderer.flipX = false;
            _rb.linearVelocity = new Vector2(_daggerSpeed, 0);
            _onThrowEvent.DaggerInMotion = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _anim.SetBool("HitEnemy", true);
            Destroy(gameObject, .4f);
        }
    }
    private void OnThrowEventHandler(bool isDaggerInMotion)
    {
        _onThrowEvent.DaggerInMotion = isDaggerInMotion;
        _onThrowEvent.RemoveListener(OnThrowEventHandler);
    }
    public void Invoke(bool value)
    {
        _onThrowEvent.AddListener(OnThrowEventHandler);
        _onThrowEvent.Invoke(value);
    }

    public bool IsPlayerFlipped(Transform playerTransform)
    {
        return playerTransform.localScale.x < 0 ? true : false;
    }
}
