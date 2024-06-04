using DG.Tweening;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCrystal : MonoBehaviour
{
    private const string DIAMONG_TAG = "Diamond";
    private const float DELAY_DURATION = 3f;
    private const float X_DEGREES = 45f;
    private const float ERROR_TERM = 1f;
    private const float BOUNDS_ERROR_TERM = 5f;

    // Start is called before the first frame update
    private RectTransform m_diamondUILocation;
    private static bool m_increaseValue = false;
    private CancellationTokenSource m_cancellationTokenSource;   
    private CancellationToken m_cancellationToken;
    private float m_heightOfCamera;
    private float m_WidthOfCamera;
    private Vector3 m_initialPosition;
    private SpriteRenderer m_spriteRenderer;
    //Movement fix
    [SerializeField]
    public CrystalCollideEvent crystalCollideEvent;
    public CrystalUIIncrementEvent crystalUIIncrementEvent;

    public bool CrystalIsMoving { get; set; } = false;

    public bool OutsideScreenBounds { get; set; } = false;
    public 
    void Start()
    {
        m_cancellationTokenSource = new CancellationTokenSource();
        m_cancellationToken = m_cancellationTokenSource.Token;
        m_diamondUILocation = GameObject.FindWithTag(DIAMONG_TAG).GetComponent<RectTransform>();
        m_initialPosition = transform.position;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        crystalCollideEvent.AddListener(CrystalCollideListener);

        m_heightOfCamera = 2f * Camera.main.orthographicSize;
        //aspect ratio = width / height
        m_WidthOfCamera = m_heightOfCamera * Camera.main.aspect;

    }

    private async void Update()
    {
       
        if (CrystalIsMoving)
        {
            if (await IsOutsideCameraBounds(transform.position, m_initialPosition, m_WidthOfCamera, m_heightOfCamera) && !OutsideScreenBounds)
            {
                m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, 0);
                OutsideScreenBounds = true;
            }

            if (await IsCrystalAtTheGuiPanel(m_diamondUILocation.position))
            {
                CrystalIsMoving = false;
                Destroy(gameObject);
            }
        }
    }

    private Task<bool> IsOutsideCameraBounds(Vector3 currentPos, Vector3 initialPos, float cameraWidth, float cameraHeight)
    {
        return Task.FromResult((currentPos.x  >= initialPos.x - BOUNDS_ERROR_TERM + cameraWidth / 2) && (currentPos.y >= initialPos.y - BOUNDS_ERROR_TERM + cameraHeight / 2));
    }

    private Task<bool> IsCrystalAtTheGuiPanel(Vector3 updatedCrystalUIPosition)
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
