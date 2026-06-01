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
public class GameStateManager : MonoBehaviour, IGameState, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<IGameStateHandler>, IRequest<GameStateManager>
{
    private SceneData _sceneData;
    private string _fileName;
    private Camera _mainCamera;
    private Vector3 _mainCameraOldPosition;

    [SerializeField]
    public string fileName;

    public ProgressBar progressBar;

    public CheckPointEvent onCheckpointSaveEvent; //checkpoint event

    public List<string> jsonSerializedData = new List<string>();

    public List<IGameStateHandler> GameStateHandlerObjects { get; set; } = new List<IGameStateHandler>();

    private Delegator Delegator { get; set; }

    public string GetFileLocationToLoad { get => _fileName; set => _fileName = value; }

    public class ObjectDataWrapperClass
    {
        public List<SceneData.ObjectData> objectsToSave;
    }

    private async void Awake()
    {
        if (_sceneData == null)
        {
            Debug.Log("No data found, initializing everything to default");
            await NewGame();
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
        SceneManager.LoadScene(buildIndex + 1);
    }

    public async Task LoadGame(string saveFileName, SemaphoreSlim lockingThread) //implement LoadGame with Json etc by saving states
    {
        //load the whole scene
        string saveFileLocation = Path.Combine(Application.persistentDataPath, saveFileName);
        var jsonData = File.ReadAllText(saveFileLocation);
        ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(jsonData);
        var objectsToLoad = wrapper.objectsToSave;
        foreach (var objectToLoad in objectsToLoad)
        {
            var foundObject = GameObject.Find(objectToLoad.name);
            if (foundObject==null)
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
        await Task.CompletedTask;
    }

    public async Task LoadLastCheckPoint(SemaphoreSlim lockingThread)
    {
        await LoadLastCheckPoint(GetFileLocationToLoad, lockingThread);
    }

    public async Task LoadLastCheckPoint(string saveFileName, SemaphoreSlim lockingThread)
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
                await UpdateSceneData(gameObjectData);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        finally
        {
            lockingThread.Release();
        }
       
    }

    public Task UpdateSceneData(SceneData.ObjectData gameObjectData)
    {
        GameObject gameObject = GameObject.FindWithTag(gameObjectData.tag);

        gameObject.transform.position = gameObjectData.position;

        gameObject.transform.rotation = gameObjectData.rotation;

        if(gameObjectData.entity!=null)
        {
            gameObject.GetComponent<AbstractEntity>().Health = gameObjectData.health;
        }
           
        return Task.CompletedTask;
    }

    public async Task SaveGame()
    {
        await SaveGame(fileName);   
    }

    public async Task SaveGame(string fileName)
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

        await Task.CompletedTask;
    }

    public Task NewGame()
    {
        this._sceneData = new SceneData(); //initializes the new data
        return Task.CompletedTask;
    }

    public async Task RestartLevel()
    {
        await SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }

    public Task InvokeListeners(List<IGameStateHandler> handlers)
    {
        foreach (IGameStateHandler gameObjectState in handlers)
        {
            try
            {
                onCheckpointSaveEvent.AddListener(gameObjectState.GameStateHandler); //we subscribe to the game object
                onCheckpointSaveEvent.Invoke(_sceneData); //gathering all the current state of the object implementing IGameStateHandler
                onCheckpointSaveEvent.RemoveListener(gameObjectState.GameStateHandler); //we de-subscribe until next point
            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);

            }

        }
        return Task.CompletedTask;
    }

    public async Task SaveCheckPoint(string fileName)
    {
        try
        {
            await InvokeListeners(GameStateHandlerObjects);

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
        catch (System.Exception e)
        {
            Debug.Log(e.Message);

        }

    }

    public Task LoadSceneAsync(int sceneIndex)
    {
        StartCoroutine(LoadScene(sceneIndex));
        return Task.CompletedTask;
    }

    public IEnumerator LoadScene(int sceneIndex)
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync(sceneIndex);

        //show it on the UI (percentage bar)
        float loadPercentage = loadingScene.progress;
        progressBar.value = loadPercentage;
        Debug.Log(loadPercentage);
        if (loadingScene.isDone)
        {
            _mainCamera.transform.position = _mainCameraOldPosition;
            yield return null; //fix this tomorrow
        }
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
}
