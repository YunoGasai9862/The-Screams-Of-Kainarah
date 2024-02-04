using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCrystal : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isMoving = false;
    private RectTransform _diamondUILocation;
    private Vector3 _diamondUILocaitonConverted, LocalPos;
    private static bool increaseValue = false;
    private CancellationTokenSource _cancellationTokenSource;   
    private CancellationToken _cancellationToken;
    public static bool IncreaseValue { get => increaseValue; set => increaseValue = value; }
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _diamondUILocation = GameObject.FindWithTag("Diamond").GetComponent<RectTransform>();

    }
    // Update is called once per frame
    async void Update()
    {
        MoveTheCrystalToTheGuiPanel();

        bool isAtTheGuiPanel = await IsCrystalAtTheGuiPanel();

        if(isAtTheGuiPanel)
        {
            IncreaseValue = true;
            Destroy(gameObject);
        }
    }

    public bool conditionsSatisfied(Transform transform, bool isMoving)
    {
        return transform != null && isMoving;
    }
    public async void MoveTheCrystalToTheGuiPanel()
    {
        if (conditionsSatisfied(transform, _isMoving))
        {
            _diamondUILocaitonConverted = Camera.main.ScreenToWorldPoint(_diamondUILocation.position); //converts UI position to world position
            LocalPos = _diamondUILocaitonConverted;
            LocalPos.z = 0;
            LocalPos.x--;
            transform.DOMove(LocalPos, .050f).SetEase(Ease.InFlash);
        }

        await Task.Delay(10);

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
           InventoryManagementSystem.Instance.AddInventoryItemEvent.Invoke(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);
        }
    }

    public Task<bool> IsCrystalAtTheGuiPanel()
    {
        return Task.FromResult(((int)transform.position.x == (int)LocalPos.x));
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }
}
