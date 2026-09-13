using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IMarkerManager
    {
        GameObject FindMarker(string markerName, string markerTag = "");
    } 
}
