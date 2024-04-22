
using System;
using UnityEngine;

[Serializable]
public class LightPreProcessWrapper : MonoBehaviour
{
    [SerializeField]
    public MonoBehaviour LightPreprocess; //Monobehavior can be of any type/script (generic)

    //when this property is accessed, it returns/converts it to the appropriate type
    public ILightPreprocess LightCustomPreprocess()
    {
        ILightPreprocess LightCustomPreprocess = LightPreprocess as ILightPreprocess;
        //do some checks here
        return LightCustomPreprocess;
    }
}