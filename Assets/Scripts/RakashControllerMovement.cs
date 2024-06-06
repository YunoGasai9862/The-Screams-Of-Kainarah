using System.Collections;
using UnityEngine;

public class RakashControllerMovement : MonoBehaviour, IReceiver<bool>
{
    private void Start()
    {
        
    }
    public bool CancelAction()
    {
        return true;
    }

    public bool PerformAction(bool value = false)
    {
        return true;
    }
}