using UnityEngine;

public class GameObjectInstantiator
{
    GameObject _prefab;
    GameObject _gameObject;
    public GameObjectInstantiator(GameObject _prefab)
    {
        this._prefab = _prefab;
    }

    public GameObject InstantiateGameObject(Vector3 _gameObjectPosition, Quaternion _rotationType)
    {
        _gameObject = Object.Instantiate(_prefab, _gameObjectPosition, _rotationType);
        return _gameObject;
    }

    public void DestroyGameObject()
    {
        Object.Destroy(_gameObject);
    }

    public GameObject getGameObject()
    {
        return _gameObject;
    }

    public void setGameObjectParent(Transform parent)
    {
        this._gameObject.transform.parent = parent;
    }
}
