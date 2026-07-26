using Annotations.Enums;
using Assets.Annotations;
using Assets.Exceptions;
using Assets.Scripts.Checkpoint.Models;
using Assets.Scripts.GameState.Models;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Newtonsoft.Json;
using Pathfinding.Util;
using System;
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
[Asset(Asset.MONOBEHAVIOR, "GameStateManager", InstantiationOrder = 15)]
public class GameStateManager : Assets.Scripts.Scene.Scene, IGameState, Assets.Scripts.Interfaces.Mediator.EnhancedV3.IRequest<IGameStateHandler>, IRequest<GameStateManager>
{
    private SceneData _sceneData;

    private Camera _mainCamera;

    private Vector3 _mainCameraOldPosition;

    public ProgressBar progressBar;

    public List<IGameStateHandler> GameStateHandlerObjects { get; set; } = new List<IGameStateHandler>();

    private Delegator Delegator { get; set; }

    private SceneRegistry SceneRegistry { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Awake()
    {
        SceneUtils = await BaseScene.GetSceneUtilsAsync();

        SceneRegistry = SceneUtils.FindObject<SceneRegistry>();

        if (SceneRegistry == null)
        {
            Debug.Log($"[CRITICAL] SceneRegistry Regsitry is null...");

            throw new MissingEntityException($"SceneRegistry Registry could not be found in the scene! Please ensure that there is an active game object with the SceneRegistry component attached to it in the scene.");
        }

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

    public IEnumerator LoadLastCheckPoint(System.Guid id)
    {

        var saveFilePath = Path.Combine(Application.persistentDataPath, id.ToString());
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

    public IEnumerator SaveGame(System.Guid id, string sceneVersion)
    {
        GameStateManagerDto gameStateManagerDto = SaveSceneSnapshot(id, SceneManager.GetActiveScene().name, sceneVersion, Path.Combine(Application.persistentDataPath, id.ToString()));

        File.WriteAllText(gameStateManagerDto.Location, gameStateManagerDto.JsonBlob);

        yield return null;
    }

    public IEnumerator NewGame()
    {
        _sceneData = new SceneData(); //initializes the new data

        yield return null;
    }

    public void InvokeListeners(List<IGameStateHandler> handlers)
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
    }

    public IEnumerator SaveCheckPoint(System.Guid id, string sceneVersion)
    {
        GameStateManagerDto gameStateManagerDto = GetCheckpointData(id);

        File.WriteAllText(gameStateManagerDto.Location, gameStateManagerDto.JsonBlob);

        yield return null;
    }

    private GameStateManagerDto GetCheckpointData(System.Guid id)
    {
        List<string> jsonSerializedData = new List<string>();

        InvokeListeners(GameStateHandlerObjects);

        foreach (var objectToSave in _sceneData.ObjectsToPersit)
        {
            var jsonObject = JsonUtility.ToJson(objectToSave);
            jsonSerializedData.Add(jsonObject);
        }

        var completeJson = "[" + string.Join(",", jsonSerializedData) + "]"; //joing them in a single file

        string localFilename = Path.Combine(Application.persistentDataPath, id.ToString());

        return new GameStateManagerDto
        {
            JsonBlob = completeJson,
            Location = localFilename
        };
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

    public IEnumerator LoadGame(System.Guid id)
    {
        //load the whole scene
        string saveFileLocation = Path.Combine(Application.persistentDataPath, id.ToString());
        var jsonData = File.ReadAllText(saveFileLocation);
        ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(jsonData);
        var objectsToLoad = wrapper.objectsToSave;
        foreach (var objectToLoad in objectsToLoad)
        {
            var foundObject = GameObject.Find(objectToLoad.name);
            if (foundObject == null)
            {
                GameObject prefab = Resources.Load<GameObject>(objectToLoad.name); //load the prefab
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

    public async Task LoadGameAsync(System.Guid id,CancellationToken cancellationToken)
    {
        var jsonData = await File.ReadAllTextAsync(Path.Combine(Application.persistentDataPath, id.ToString()));
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

    public async Task SaveGameAsync(System.Guid id, string sceneVersion, CancellationToken cancellationToken)
    {
        GameStateManagerDto gameStateManagerDto = SaveSceneSnapshot(id, SceneManager.GetActiveScene().name, sceneVersion, Path.Combine(Application.persistentDataPath, id.ToString()));

        await File.WriteAllTextAsync(gameStateManagerDto.Location, gameStateManagerDto.JsonBlob);
    }

    private GameStateManagerDto SaveSceneSnapshot(System.Guid id, string sceneName, string sceneVersion, string fileLocation)
    {
        CheckPointMetaData metaData = null;

        if (File.Exists(fileLocation))
        {
            using (StreamReader reader = new StreamReader(Path.Combine(Application.persistentDataPath, id.ToString()), false))
            {
               metaData = JsonConvert.DeserializeObject<CheckPointMetaData>(reader.ReadToEnd());
            }

        } else
        {
            metaData = new CheckPointMetaData(id, sceneName, sceneVersion, fileLocation);
        }  

        List<string> jsonSerializedData = new List<string>();
        List<SceneData.ObjectData> gameData = new List<SceneData.ObjectData>(); //different approach

        foreach (var gameObject in SceneRegistry.GetRegisteredGameObjects().Values.ToList())
        {
            var gameObjectForSerializedData = JsonUtility.ToJson(new SceneData.ObjectData(gameObject.name, gameObject.tag, gameObject.transform.position, gameObject.transform.rotation));

            jsonSerializedData.Add(gameObjectForSerializedData);
        }

        metaData.ObjectDataWrapper = new ObjectDataWrapperClass { objectsToSave = gameData };

        string completeJson = JsonUtility.ToJson(metaData);

        File.WriteAllText(fileLocation, completeJson);

        return new GameStateManagerDto
        {
            JsonBlob = completeJson,
            Location = fileLocation
        };

    }

    public Task SaveCheckPointAsync(System.Guid id, string sceneVersion, CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task RestartLevelAsync(CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public Task LoadLastCheckPointAsync(System.Guid id, CancellationToken cancellationToken)
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

    public string GetSaveFileLocation(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, fileName);
    }
}
