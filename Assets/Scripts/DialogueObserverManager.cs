using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DialogueEntityScriptableObject;

public class DialogueObserverManager : MonoBehaviour, IObserver<DialogueEntity>
{

    DialogueManagerDictionary<DialogueEntity, >

    private void OnEnable()
    {
        PlayerObserverListenerHelper.DialogueEntites.AddObserver(this);

    }

    private void OnDisable()
    {
        PlayerObserverListenerHelper.DialogueEntites.RemoveOberver(this); 
    }
    public void OnNotify(ref DialogueEntity Data, params object[] optional)
    {
       
    }

}
