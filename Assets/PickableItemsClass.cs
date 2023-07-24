using System.Collections.Generic;
using UnityEngine;

public class PickableItemsClass : MonoBehaviour
{

    [System.Serializable]
    public class pickableItemsKeyValuePair
    {
        public string name;
        public GameObject GameObject;
    }

    [SerializeField] List<pickableItemsKeyValuePair> pickableItems;
    public bool didPlayerCollideWithaPickableItem(string collisionObjectName)
    {
        for (int i = 0; i < pickableItems.Count; i++)
        {
            if (collisionObjectName == pickableItems[i].name)
            {
                return true;
            }
        }

        return false;
    }

    public GameObject returnGameObjectForTheKey(string keyName)
    {
        for (int i = 0; i < pickableItems.Count; i++)
        {
            if (keyName == pickableItems[i].name)
            {
                return pickableItems[i].GameObject;
            }
        }

        return null;
    }
}
