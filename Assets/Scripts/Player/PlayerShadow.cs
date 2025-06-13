using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private Vector2 m_Position;
    private SpriteRenderer m_SpriteRenderer;
    private Vector2 m_newPosition;
    private Vector2 m_parentPos;
    private CancellationToken _token;
    private CancellationTokenSource _tokenSource;

    [SerializeField]
    public float initialoffsetY;
    public float initialoffsetX;

    private void Awake()
    {
        m_Position = new Vector2(transform.position.x + initialoffsetX, transform.position.y + initialoffsetY);
        m_SpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_parentPos = transform.parent.position;
        _tokenSource= new CancellationTokenSource();
        _token = _tokenSource.Token;
    }
    // Update is called once per frame
     async void Update()
    {
        m_newPosition = await ShadowObjectsNewPosition(m_SpriteRenderer, m_parentPos, m_Position, 0.5f, 10);

        if(!_token.IsCancellationRequested) //extra check due to async programming
        {
            transform.position = new Vector2(m_newPosition.x, m_newPosition.y); //updates it

            m_Position = transform.position;

            m_parentPos = transform.parent.position;
        }
    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offsetx, int delyForShadowInMiliseconds)
    {
        Vector2 result = new(0, 0);

        result = Helper.FlipTheObjectToFaceParent(ref spriteRenderer, parentPos, position, offsetx);

        await Task.Delay(delyForShadowInMiliseconds, _token); //why making it zero fix the issue of getting the null exception (debug tomorrow)

        return result;

    }

    private void OnDisable()
    {
        _tokenSource.Cancel();

    }

}
