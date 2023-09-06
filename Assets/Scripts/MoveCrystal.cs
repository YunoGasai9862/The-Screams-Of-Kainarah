using DG.Tweening;
using UnityEngine;

public class MoveCrystal : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isMoving = false;
    private RectTransform _diamondUILocation;
    private Vector3 _diamondUILocaitonConverted, LocalPos;
    public static bool increaseValue = false;
    void Start()
    {
        _diamondUILocation = GameObject.FindWithTag("Diamond").GetComponent<RectTransform>();

    }

    // Update is called once per frame
    void Update()
    {


        if (transform != null && _isMoving)
        {
            _diamondUILocaitonConverted = Camera.main.ScreenToWorldPoint(_diamondUILocation.position); //converts UI position to world position
            LocalPos = _diamondUILocaitonConverted;
            Debug.Log("Here");
            LocalPos.z = 0;
            LocalPos.x--;
            transform.DOMove(LocalPos, .050f).SetEase(Ease.InFlash);
            // transform.GetComponent<BoxCollider2D>().enabled = false;


        }

        OnDestroy();

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.CompareTag("Sword"))
        {
            _isMoving = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.CompareTag("Sword"))
        {
            CreateInventorySystem.AddToInventory(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);
        }
    }


    private void OnDestroy()
    {
        if (transform != null && ((int)transform.position.x == (int)LocalPos.x))
        {
            increaseValue = true;
            Destroy(gameObject);
        }
    }
}
