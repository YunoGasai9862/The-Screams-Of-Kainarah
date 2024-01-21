using UnityEngine;
public class GameObjectInstantiator
{
    private readonly GameObject _prefab;
    private GameObject _gameObject;
    public GameObjectInstantiator(GameObject _prefab)
    {
        this._prefab = _prefab;
    }

    public GameObject InstantiateGameObject(Vector3 _gameObjectPosition, Quaternion _rotationType)
    {
        _gameObject = Object.Instantiate(_prefab, _gameObjectPosition, _rotationType);
        return _gameObject;
    }

    public void DestroyGameObject(float time)
    {
        Object.Destroy(_gameObject, time);
    }

    public GameObject getGameObject()
    {
        return _gameObject;
    }

    public void setGameObjectParent(Transform parent)
    {
        _gameObject.transform.parent = parent;
    }
}