using System.Collections.Generic;
using UnityEngine;

public class PickableItemsUtility
{
    public PickableItems PickableItems { get; private set; }
    public PickableItemsUtility(PickableItems pickableItems)
    {
        if (pickableItems == null)
        {
            throw new NullException("Pickable Items is null!");
        }

        PickableItems = pickableItems;
    }

    public bool IsPickableItem(string collisionObjectName)
    {
        for (int i = 0; i < PickableItems.pickableEntities.Length; i++)
        {
            var element = PickableItems.pickableEntities[i];

            if (collisionObjectName == element.objectName)
            {
                return true;
            }
        }

        return false;
    }
    public GameObject GetGameObject(string keyName)
    {
        for (int i = 0; i < PickableItems.pickableEntities.Length; i++)
        {
            var element = PickableItems.pickableEntities[i];

            if (keyName == element.objectName)
            {
                return element.prefabToInstantiate;
            }
        }

        return null;
    }

    public bool ShouldThisItemBeDisabled(string collisionObjectName)
    {
        for (int i = 0; i < PickableItems.pickableEntities.Length; i++)
        {
            var element = PickableItems.pickableEntities[i];

            if (collisionObjectName == element.objectName)
            {
                return element.shouldBeDisabledAfterSomeTime;
            }
        }

        return true;
    }
}
