using UnityEngine;
public class InstantiatorController
{
    private readonly GameObject _prefab;
    private GameObject _gameObject;
    public InstantiatorController(GameObject prefab)
    {
        this._prefab = prefab;
    }

    public GameObject InstantiateGameObject(Vector3 gameObjectPosition, Quaternion rotationType)
    {
        _gameObject = Object.Instantiate(_prefab, gameObjectPosition, rotationType);
        return _gameObject;
    }

    public void DestroyGameObject(float time)
    {
        Object.Destroy(_gameObject, time);
    }

    public GameObject GetGameObject()
    {
        return _gameObject;
    }

    public void SetGameObjectParent(Transform parent)
    {
        _gameObject.transform.parent = parent;
    }
}