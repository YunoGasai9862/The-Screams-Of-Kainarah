using System.Collections.Generic;
using UnityEngine;

public class PickableItemsHandler : MonoBehaviour
{
    [SerializeField]
    public PickableItems pickableItems;
    public bool DidPlayerCollideWithaPickableItem(string collisionObjectName)
    {
        for (int i = 0; i < pickableItems.pickableEntities.Length; i++)
        {
            var element = pickableItems.pickableEntities[i];

            if (collisionObjectName == element.objectName)
            {
                return true;
            }
        }

        return false;
    }
    public GameObject ReturnGameObjectForTheKey(string keyName)
    {
        for (int i = 0; i < pickableItems.pickableEntities.Length; i++)
        {
            var element = pickableItems.pickableEntities[i];

            if (keyName == element.objectName)
            {
                return element.prefabToInstantiate;
            }
        }

        return null;
    }

    public bool ShouldThisItemBeDisabled(string collisionObjectName)
    {
        for (int i = 0; i < pickableItems.pickableEntities.Length; i++)
        {
            var element = pickableItems.pickableEntities[i];

            if (collisionObjectName == element.objectName)
            {
                return element.shouldBeDisabledAfterSomeTime;
            }
        }

        return true;
    }
}
