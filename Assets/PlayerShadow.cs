using GlobalAccessAndGameHelper;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerShadow : MonoBehaviour
{
    private Vector2 m_Position;
    private SpriteRenderer m_SpriteRenderer;
    private Rigidbody2D m_Rigidbody;
    private bool isFixed = false;
    Vector2 newPosition;
    private Vector2 m_PreviousPos;
    private void Awake()
    {
        m_Position = transform.position;
        m_SpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_PreviousPos = transform.position;
    }
    // Update is called once per frame
    async void Update()
    {
        newPosition = await ShadowObjectsNewPosition(m_SpriteRenderer, m_Position, .5f);

        if (!isFixed)
        {
            transform.position = new Vector2(newPosition.x, newPosition.y); //updates it
            m_Position = transform.position;
        }

    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 position, float offset)
    {
        Vector2 result = HelperFunctions.FlipTheObjectToFaceParent(ref spriteRenderer, position, offset);

        if (m_PreviousPos.magnitude == result.magnitude)
            isFixed = true;
        else
            isFixed = false;

        m_PreviousPos = result;

        await Task.Delay(200); //to not let it calculate everytime

        return result;


    }

}
