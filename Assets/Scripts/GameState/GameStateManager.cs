using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Assets.Scripts.ScenePersistence.Models;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(GameStateManager), ContextType = typeof(IGameStateHandler))]
public class GameStateManager : Assets.Scripts.Scene.Scene, IGameState, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<IGameStateHandler>, IRequest<GameStateManager>
{
    private SceneData _sceneData;
    private string _fileName;
    private Camera _mainCamera;
    private Vector3 _mainCameraOldPosition;

    [SerializeField]
    public string fileName;

    public ProgressBar progressBar;

    public List<string> jsonSerializedData = new List<string>();

    public List<IGameStateHandler> GameStateHandlerObjects { get; set; } = new List<IGameStateHandler>();

    private Delegator Delegator { get; set; }

    public string GetFileLocationToLoad { get => _fileName; set => _fileName = value; }

    public class ObjectDataWrapperClass
    {
        public List<SceneData.ObjectData> objectsToSave;
    }

    private void Awake()
    {
        if (_sceneData == null)
        {
            Debug.Log("No data found, initializing everything to default");

            StartCoroutine(NewGame());
        }
        _mainCamera = Camera.main;
        _mainCameraOldPosition = _mainCamera.transform.position;

       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        /*
        _mainCamera.transform.position = new Vector3(_mainCamera.transform.position.x, _mainCamera.transform.position.y, -100);
         */
    }

    public void ChangeLevel(int buildIndex)
    {
        LoadScene(buildIndex + 1);
    }

    public IEnumerator LoadLastCheckPoint()
    {
         yield return StartCoroutine(LoadLastCheckPoint(GetFileLocationToLoad));
    }

    public IEnumerator LoadLastCheckPoint(string saveFileName)
    {

        var saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        var jsonData = File.ReadAllText(saveFilePath);
        var wrappedJsonData = "{\"objectsToSave\":" + jsonData + "}"; //for deserializing
        Debug.Log($"Wrapped JsonData: {wrappedJsonData}");
        try
        {
            ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(wrappedJsonData);
            List<SceneData.ObjectData> savedData = wrapper.objectsToSave;
            foreach (var gameObjectData in savedData)
            {
                UpdateSceneData(gameObjectData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }

        yield return null;

    }

    public IEnumerator UpdateSceneData(SceneData.ObjectData gameObjectData)
    {
        GameObject gameObject = GameObject.FindWithTag(gameObjectData.tag);

        gameObject.transform.position = gameObjectData.position;

        gameObject.transform.rotation = gameObjectData.rotation;

        if(gameObjectData.entity!=null)
        {
            gameObject.GetComponent<AbstractEntity>().Health = gameObjectData.health;
        }

        yield return null;
    }

    public IEnumerator SaveGame()
    {
        yield return StartCoroutine(SaveGame(fileName));
    }

    public IEnumerator SaveGame(string fileName)
    {
        GameObject[] allGameObjectsInTheScene = FindObjectsByType<GameObject>(FindObjectsSortMode.None).Where(o=>o.transform == o.transform.root).ToArray(); //only parent objects
        List<SceneData.ObjectData> gameData = new List<SceneData.ObjectData>(); //different approach

        foreach (var gameObject in allGameObjectsInTheScene)
        {
            var gameObjectForSerializedData = JsonUtility.ToJson(new SceneData.ObjectData(gameObject.name, gameObject.tag, gameObject.transform.position, gameObject.transform.rotation));
            jsonSerializedData.Add(gameObjectForSerializedData);
        }
        var completeJson = "{\"objectsToSave\": [" + string.Join(",", jsonSerializedData) + "]}";
        Debug.Log(completeJson);
        string location = Path.Combine(Application.persistentDataPath, fileName);
        GetFileLocationToLoad = location;
        File.WriteAllText(location, completeJson);

        yield return null;
    }

    public IEnumerator NewGame()
    {
        _sceneData = new SceneData(); //initializes the new data

        yield return null;
    }

    public IEnumerator InvokeListeners(List<IGameStateHandler> handlers)
    {
        foreach (IGameStateHandler gameObjectState in handlers)
        {
            try
            {
                gameObjectState.GameStateHandler(_sceneData); //we gather the current state of the object implementing IGameStateHandler)
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);

            }
        }

        yield return null;
    }

    public IEnumerator SaveCheckPoint(string fileName)
    {
        yield return StartCoroutine(InvokeListeners(GameStateHandlerObjects));

        foreach (var objectToSave in this._sceneData.ObjectsToPersit)
        {
            var jsonObject = JsonUtility.ToJson(objectToSave);
            jsonSerializedData.Add(jsonObject);
        }
        var completeJson = "[" + string.Join(",", jsonSerializedData) + "]"; //joing them in a single file
        string localFilename = Path.Combine(Application.persistentDataPath, fileName);
        GetFileLocationToLoad = localFilename;
        File.WriteAllText(localFilename, completeJson);
        jsonSerializedData.Clear(); //remove old data
    }

    public IEnumerator LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);

        yield return null;
    }

    public IEnumerator Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<GameStateManager>() { Data = this, EntityType = typeof(GameStateManager) }, this));
    }

    public Task<IGameStateHandler> Request(INotify<IGameStateHandler> obsever)
    {
        GameStateHandlerObjects.Add((IGameStateHandler)obsever);

        return null;
    }

    public IEnumerator LoadGame(string saveFileName)
    {
        //load the whole scene
        string saveFileLocation = Path.Combine(Application.persistentDataPath, saveFileName);
        var jsonData = File.ReadAllText(saveFileLocation);
        ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(jsonData);
        var objectsToLoad = wrapper.objectsToSave;
        foreach (var objectToLoad in objectsToLoad)
        {
            var foundObject = GameObject.Find(objectToLoad.name);
            if (foundObject == null)
            {
                var prefab = Resources.Load<GameObject>(objectToLoad.name); //load the prefab
                GameObject go = Instantiate(prefab, objectToLoad.transform.position, objectToLoad.rotation); //instantiate it
                Debug.Log(go);
            }
            else
            {
                foundObject.transform.position = objectToLoad.transform.position;
            }
        }

        yield return null;
    }

    IEnumerator IGameState.RestartLevel()
    {
        yield return StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public async Task LoadGameAsync(string saveFileName, CancellationToken cancellationToken)
    {
        var jsonData = await File.ReadAllTextAsync(Path.Combine(Application.persistentDataPath, saveFileName));
        ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(jsonData);
        foreach (SceneData.ObjectData objectToLoad in wrapper.objectsToSave)
        {
            var foundObject = SceneUtils.Find(objectToLoad.name, false);
            if (foundObject == null)
            {
                GameObject prefab = (GameObject) Resources.LoadAsync<GameObject>(objectToLoad.name).asset; //load the prefab
                GameObject go = Instantiate(prefab, objectToLoad.transform.position, objectToLoad.rotation); //instantiate it
                Debug.Log($"Instantiated From [LoadGameAsync]: {go.name}");
            }
            else
            {
                foundObject.transform.position = objectToLoad.transform.position;
            }
        }
    }

    public Task SaveGameAsync(string fileName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task SaveCheckPointAsync(string saveFileName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task RestartLevelAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task LoadLastCheckPointAsync(string saveFileName, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task NewGameAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    async Task IGameState.LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneIndex);

        progressBar.value = loadingScene.progress;

        if (loadingScene.isDone)
        {
            _mainCamera.transform.position = _mainCameraOldPosition;
        }
    }
}
