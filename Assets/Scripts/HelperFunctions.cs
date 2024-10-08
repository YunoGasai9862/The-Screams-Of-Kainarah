using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.Rendering.Universal;
public static class HelperFunctions
{
    public static float CalculateScreenWidth(Camera _mainCamera)
    {
        float aspectRatio = _mainCamera.aspect;
        return aspectRatio * _mainCamera.orthographicSize;
    }

    public static IEnumerator TuneDownIntensityToZero(Light2D _light)
    {
        while (_light.intensity > 0f)
        {
            _light.intensity -= 10 * Time.deltaTime;

            yield return new WaitForSeconds(.1f);
        }

    }
    public static Vector2 FlipTheObjectToFaceParent(ref SpriteRenderer spriteRenderer, Vector2 parentPos, Vector2 position, float offsetX)
    {
        Vector2 flipped = Vector2.zero;

        if (spriteRenderer.flipX)
        {
            flipped = new Vector2(parentPos.x + offsetX, position.y);
        }
        else
        {
            flipped = new Vector2(parentPos.x - offsetX, position.y);

        }
        return flipped;
    }
    public static bool CheckDistance(Animator animator, float distanceLessThan, float distanceGreaterThan, GameObject player)
    {
        return Vector3.Distance(player.transform.position, animator.transform.position) <= distanceLessThan && Vector3.Distance(player.transform.position, animator.transform.position) >= distanceGreaterThan;
    }
    public static void DelayAttack(Animator animator, float timeSpanBetweenEachAttack, string triggerName)
    {
        var timeSpan = 0f;
        while (timeSpan < timeSpanBetweenEachAttack)
            timeSpan += Time.deltaTime;

        if (timeSpan > timeSpanBetweenEachAttack)
        {
            animator.SetTrigger(triggerName);
        }
    }

    public static bool IsEntityMonoBehavior(EntityType entityType)
    {
        return entityType.Equals(EntityType.MonoBehavior);
    }

    public static Task SetAsParent(GameObject child, GameObject parent)
    {
        child.transform.parent = parent.transform;

        return Task.CompletedTask;
    }

}
