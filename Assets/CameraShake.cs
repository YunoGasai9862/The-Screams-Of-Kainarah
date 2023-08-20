
using PlayerAnimationHandler;
using System.Threading.Tasks;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] Camera _mainCamera;
    private int _currentPlayerState = AnimationStateKeeper.currentPlayerState; //to see if the player is attacking or not
    void Start()
    {

    }


    void Update()
    {

    }

    private async Task<bool> shakeCamera(Camera _camera) //do it tomorrow
    {
        await Task.Delay(100);
        return  false;
    }
}
