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
    private Vector2 m_previousResults;
    private void Awake()
    {
        m_Position = transform.position;
        m_SpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_previousResults = transform.position;
    }
    // Update is called once per frame
    async void Update()
    {
        Vector2 newPosition = await ShadowObjectsNewPosition(m_SpriteRenderer, m_Position, .5f);

        if (!isFixed)
        {
            m_previousResults = newPosition;
            transform.position = new Vector2(newPosition.x, newPosition.y); //updates it
        }
    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 position, float offset)
    {
        Vector2 result = HelperFunctions.FlipTheObjectToFaceParent(ref spriteRenderer, position, offset);

        if (result.magnitude == m_previousResults.magnitude)
            isFixed = true;
        else
            isFixed = false;

        Debug.Log(result + " " + m_previousResults);

        await Task.Delay(200); //to not let it calculate everytime

        return result;


    }

}
