using System.Collections.Generic;
using UnityEngine;

public class LightPreprocessDelegatorManager : MonoBehaviour
{
    public LightPreprocessDelegator<MonoBehaviour> LightPreprocessDelegator { get; private set; }
    private void Awake()
    {
        LightPreprocessDelegator = new LightPreprocessDelegator<MonoBehaviour>();
    }
}