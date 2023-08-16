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
    private Vector2 m_newPosition;
    private Vector2 m_parentPos;
    private void Awake()
    {
        m_Position = transform.position;
        m_SpriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_parentPos = transform.parent.position;
    }
    // Update is called once per frame
    async void Update()
    {
        m_newPosition = await ShadowObjectsNewPosition(m_SpriteRenderer, m_parentPos, m_Position, .5f);

        transform.position = new Vector2(m_newPosition.x, m_newPosition.y); //updates it

        m_Position = transform.position;

        m_parentPos = transform.parent.position;


    }

    private async Task<Vector2> ShadowObjectsNewPosition(SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offset)
    {
        Vector2 result = HelperFunctions.FlipTheObjectToFaceParent(ref spriteRenderer, parentPos, position, offset);

        await Task.Delay(100); //to not let it calculate everytime

        return result;


    }

}
