using System.Collections.Generic;
using UnityEngine;

public class PickableItemsUtility
{
    public bool DidPlayerCollideWithaPickableItem(string collisionObjectName, PickableItems pickableItems)
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
    public GameObject ReturnGameObjectForTheKey(string keyName, PickableItems pickableItems)
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

    public bool ShouldThisItemBeDisabled(string collisionObjectName, PickableItems pickableItems)
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
