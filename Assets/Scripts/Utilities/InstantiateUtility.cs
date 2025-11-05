using UnityEngine;
public class InstantiateUtility
{
    private GameObject Prefab { get; set; }
    private GameObject InstantiatedPrefab { get; set; }

    public InstantiateUtility() { }

    public InstantiateUtility(GameObject prefab)
    {
        Prefab = prefab;
    }

    public GameObject InstantiateObject(Vector3 gameObjectPosition, Quaternion rotationType)
    {
        if (Prefab == null)
        {
            throw new System.ApplicationException("Prefab is missing!");
        }

        InstantiatedPrefab = Object.Instantiate(Prefab, gameObjectPosition, rotationType);

        return InstantiatedPrefab;
    }

    public void DestroyObjectAfter(float time = 0f)
    {
        Object.Destroy(InstantiatedPrefab, time);
    }

    public void SetPrefab(GameObject prefab)
    {
        Prefab = prefab;
    }

    public GameObject GetObject()
    {
        return InstantiatedPrefab;
    }

    public void SetObjectsParent(Transform parent)
    {
        if (InstantiatedPrefab == null)
        {
            throw new System.ApplicationException("InstantiatedPrefab is null!");
        }

        InstantiatedPrefab.transform.parent = parent;
    }
}