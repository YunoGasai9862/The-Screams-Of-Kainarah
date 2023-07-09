using UnityEngine;

public class gameObjectInstantiator : MonoBehaviour
{
    GameObject _gameObject;
    public gameObjectInstantiator(GameObject _gameObject)
    {
        this._gameObject = _gameObject;
    }

    public GameObject InstantiateGameObject(GameObject _gameObject, Vector3 _gameObjectPosition, Quaternion _rotationType)
    {
        _gameObject = Instantiate(_gameObject, _gameObjectPosition, _rotationType);
        return _gameObject;
    }

    public void DestroyGameObject()
    {
        Destroy(_gameObject);
    }
}
