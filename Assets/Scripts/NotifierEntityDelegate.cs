using UnityEngine;

public abstract class NotifierEntityDelegate : MonoBehaviour, IGenericDelegate<NotifierEntity>
{
    public abstract IGenericDelegate<NotifierEntity>.InvokeMethod InvokeCustomMethod { get; set; }
}