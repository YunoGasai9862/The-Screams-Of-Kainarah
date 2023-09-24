using System.Collections.Generic;
using UnityEngine;

public class PickableItemsClass : MonoBehaviour
{
    [SerializeField]
    public PickableItems pickableItems;
    public bool didPlayerCollideWithaPickableItem(string collisionObjectName)
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

    public GameObject returnGameObjectForTheKey(string keyName)
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


    public bool shouldThisItemBeDisabled(string collisionObjectName)
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
