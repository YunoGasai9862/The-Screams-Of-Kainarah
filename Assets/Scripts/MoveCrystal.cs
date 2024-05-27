using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCrystal : MonoBehaviour
{
    // Start is called before the first frame update
    private RectTransform _diamondUILocation;
    private static bool increaseValue = false;
    private CancellationTokenSource _cancellationTokenSource;   
    private CancellationToken _cancellationToken;
    private Vector3 _worldPosition;

    //Movement fix
    [SerializeField]
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
    }

    private async void Update()
    {
        //Get Diamond location on update - it changes
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
        return Task.FromResult(((int)transform.position.x == (int)_diamondUILocation.position.x));
    }
    
    public void CrystalCollideListener(Collider2D collider, bool didCollide)
    {
        if(collider.name == gameObject.name)
        {
            CrystalIsMoving = true;
            InventoryManagementSystem.Instance.AddInvoke(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);
            transform.DOMove(_diamondUILocation.position, 1f).SetEase(Ease.InFlash);
        }
    }

    private void OnDisable()
    {
        _cancellationTokenSource.Cancel();
    }
}
