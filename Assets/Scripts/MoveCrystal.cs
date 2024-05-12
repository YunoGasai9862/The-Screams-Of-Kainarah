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
    private Vector3 _worldPosition;
    private Vector3 _uiElementScreenPoint;

    //Movement fix
    [SerializeField]
    public Camera uiCamera;
    public CrystalCollideEvent crystalCollideEvent;

    public static bool IncreaseValue { get => increaseValue; set => increaseValue = value; }
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _diamondUILocation = GameObject.FindWithTag("Diamond").GetComponent<RectTransform>();

        crystalCollideEvent.AddListener(CrystalCollideListener);
        _worldPosition = uiCamera.ScreenToWorldPoint(_diamondUILocation.position);
        _uiElementScreenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, _worldPosition);
    }   
    // Update is called once per frame
    async void Update()
    {

        // MoveTheCrystalToTheGuiPanel();

        //  bool isAtTheGuiPanel = await IsCrystalAtTheGuiPanel();

        //   if(isAtTheGuiPanel)
        // {
        //     IncreaseValue = true;
        ///      Destroy(gameObject);
        //  }
    }

    public bool conditionsSatisfied(Transform transform, bool isMoving)
    {
        return transform != null && isMoving;
    }
    public async void MoveTheCrystalToTheGuiPanel()
    {
        if (conditionsSatisfied(transform, _isMoving))
        {
            Debug.Log("Here");
            _diamondUILocaitonConverted = Camera.main.ScreenToWorldPoint(_diamondUILocation.position); //converts UI position to world position
            Debug.Log(_diamondUILocaitonConverted);
            LocalPos = _diamondUILocaitonConverted;
            LocalPos.z = 0;
            LocalPos.x--;
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
           //InventoryManagementSystem.Instance.AddInvoke(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);
        }
    }

    public Task<bool> IsCrystalAtTheGuiPanel()
    {
        return Task.FromResult(((int)transform.position.x == (int)LocalPos.x));
    }
    
    public void CrystalCollideListener(Collider2D collider, bool didCollide)
    {
        //gameObject.GetComponent<Collider2D>().enabled = false;
        //use your own animation maybe?
        if(collider.name == gameObject.name)
        {
            Debug.Log($"MOVE {didCollide}");
            transform.DOMove(_uiElementScreenPoint, 1f).SetEase(Ease.InFlash);
        }
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }
}
