using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
public static class HelperFunctions
{
    public static float CalculateScreenWidth(Camera _mainCamera)
    {
        float aspectRatio = _mainCamera.aspect;
        return aspectRatio * _mainCamera.orthographicSize;
    }

    public static bool isWithinOneOfTheAttackStates<T>(PlayerAttackEnum.PlayerAttackSlash _attackState, Animator _playerAnimator)
    {
        int Size = System.Enum.GetValues(typeof(T)).Length;
        for (int i = 0; i < Size; i++)
        {
            //do it tomorrow
        }

        return false;
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
}
