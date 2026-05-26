
using Annotations.Enums;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interfaces.Registry
{
    public interface IRegistry
    {
        bool Decommission(Int32 instanceId, Asset assetType);

        bool Register<T>(T value, Asset assetType) where T : UnityEngine.Object;

        IEnumerator ScanScene(int scanIntervalInSeconds = 60);

        UnityEngine.Object GetRegisteredObject(Asset assetType, string objectName);
    }
}
