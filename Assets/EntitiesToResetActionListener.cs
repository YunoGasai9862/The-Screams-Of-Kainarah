using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class EntitiesToResetActionListener : MonoBehaviour, IObserver<EntitiesToReset>
{
    private async Task ResetAttributes(EntitiesToReset Data)
    {
        foreach(var entity in Data.entitiesToReset)
        {

        }
    }

    public void OnNotify(ref EntitiesToReset Data, params object[] optional)
    {
        
    }
}
