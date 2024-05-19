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
    public bool CrystalIsMoving { get; set; } = false;
    public 
    void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _diamondUILocation = GameObject.FindWithTag("Diamond").GetComponent<RectTransform>();

        crystalCollideEvent.AddListener(CrystalCollideListener);
        _worldPosition = uiCamera.ScreenToWorldPoint(_diamondUILocation.position);
        _uiElementScreenPoint = RectTransformUtility.WorldToScreenPoint(uiCamera, _worldPosition);
    }

    private async void Update()
    {
        if (CrystalIsMoving)
        {
            if(await IsCrystalAtTheGuiPanel())
            {
                IncreaseValue = true;
                CrystalIsMoving = false;
                Destroy(gameObject);
            }
        }

    }

    public Task<bool> IsCrystalAtTheGuiPanel()
    {
        return Task.FromResult(((int)transform.position.x == (int)_uiElementScreenPoint.x));
    }
    
    public void CrystalCollideListener(Collider2D collider, bool didCollide)
    {
        if(collider.name == gameObject.name)
        {
            CrystalIsMoving = true;
            InventoryManagementSystem.Instance.AddInvoke(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);
            transform.DOMove(_uiElementScreenPoint, 1f).SetEase(Ease.InFlash);
        }
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }
}
