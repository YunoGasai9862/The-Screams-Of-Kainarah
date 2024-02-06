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
    private DaggerOnThrowEvent _onThrowEvent = new DaggerOnThrowEvent();
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _daggerRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindWithTag("Player");
        _onThrowEvent.AddListener(OnThrowEventHandler);
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

        if (_player.GetComponent<SpriteRenderer>().flipX && _onThrowEvent.DaggerInMotion)
        {
            _daggerRenderer.flipX = true;
            _rb.velocity = new Vector2(-_daggerSpeed, 0);
            _onThrowEvent.DaggerInMotion = false;

        }

        if (!_player.GetComponent<SpriteRenderer>().flipX && _onThrowEvent.DaggerInMotion)
        {
            _daggerRenderer.flipX = false;
            _rb.velocity = new Vector2(_daggerSpeed, 0);
            _onThrowEvent.DaggerInMotion = false;


        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            // Destroy(collision.gameObject);
            _anim.SetBool("HitEnemy", true);
            Destroy(gameObject, .4f);


        }

    }

    public void OnThrowEventHandler(bool isDaggerInMotion)
    {
        _onThrowEvent.DaggerInMotion = isDaggerInMotion;
    }
}
