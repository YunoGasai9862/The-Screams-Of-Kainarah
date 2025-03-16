using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegatorManager : MonoBehaviour
{
    private void Start()
    {
        //just cast explicitly and store :))
        LightPreprocessDelegator<MonoBehaviour> lightPreprocessDelegator = new LightPreprocessDelegator<MonoBehaviour>();
    }
   
}