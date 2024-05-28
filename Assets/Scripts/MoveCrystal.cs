using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCrystal : MonoBehaviour
{
    private const string DIAMONG_TAG = "Diamond";
    private const float DELAY_DURATION = 5F;
    private const float X_DEGREES = 45f;
    private const float ERROR_TERM = 1f;

    // Start is called before the first frame update
    private RectTransform m_diamondUILocation;
    private static bool m_increaseValue = false;
    private CancellationTokenSource m_cancellationTokenSource;   
    private CancellationToken m_cancellationToken;
    //Movement fix
    [SerializeField]
    public CrystalCollideEvent crystalCollideEvent;

    public static bool IncreaseValue { get => m_increaseValue; set => m_increaseValue = value; }
    public bool CrystalIsMoving { get; set; } = false;
    public 
    void Start()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        m_cancellationToken = m_cancellationTokenSource.Token;
        m_diamondUILocation = GameObject.FindWithTag(DIAMONG_TAG).GetComponent<RectTransform>();

        crystalCollideEvent.AddListener(CrystalCollideListener);
    }

    private async void Update()
    {
        //Get Diamond location on update - it changes
       
        if (CrystalIsMoving)
        {
            if (await IsCrystalAtTheGuiPanel(m_diamondUILocation.position))
            {
                IncreaseValue = true;
                CrystalIsMoving = false;
                Destroy(gameObject);
            }
        }

    }

    public Task<bool> IsCrystalAtTheGuiPanel(Vector3 updatedCrystalUIPosition)
    {
        float difference = updatedCrystalUIPosition.x - updatedCrystalUIPosition.x * Mathf.Sin(X_DEGREES);
        return Task.FromResult(transform.position.x + difference + ERROR_TERM >= updatedCrystalUIPosition.x);
    }

    public void CrystalCollideListener(Collider2D collider, bool didCollide)
    {
        if(collider.name == gameObject.name)
        {
            CrystalIsMoving = true;
            InventoryManagementSystem.Instance.AddInvoke(gameObject.GetComponent<SpriteRenderer>().sprite, gameObject.tag);

            transform.DOMove(new Vector3(m_diamondUILocation.position.x * Mathf.Sin(X_DEGREES), m_diamondUILocation.position.y, m_diamondUILocation.position.z), DELAY_DURATION).SetEase(Ease.InFlash);
        }
    }

    private void OnDisable()
    {
        m_cancellationTokenSource.Cancel();
    }
}
