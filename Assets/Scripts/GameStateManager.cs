using Pathfinding.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour, IGameState
{
    private SceneData _sceneData;
    private string _fileName;

    [SerializeField]
    public string fileName;
    public static GameStateManager instance { get; private set; }

    public CheckPointEvent onCheckpointSaveEvent; //checkpoint event
    public EntireSceneSaveEvent onEntireSceneSaveEvent;

    public List<string> jsonSerializedData = new List<string>();

    public IGameStateHandler[] gameStateHandlerObjects;

    public string GetFileLocationToLoad { get => _fileName; set => _fileName = value; }

    public class ObjectDataWrapperClass
    {
        public List<SceneData.ObjectData> objectsToSave;
    }
    public class CompleteObjectWrapperClass
    {
        public IDictionary<string, SceneData.CompleteObject> gameObjects;
    }
    private  void Awake()
    {
        if(instance == null) //so only instance is there
            instance = this;

    }
    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }

    public async Task LoadGame(string saveFileName, Semaphore lockingThread) //implement LoadGame with Json etc by saving states
    {
        if (this._sceneData == null)
        {
            Debug.Log("No data found, initializing everything to default");
            await NewGame();
        }

        //load the whole scene
        string saveFileLocation = Path.Combine(Application.persistentDataPath, saveFileName);
        var jsonData = File.ReadAllText(saveFileLocation);
        CompleteObjectWrapperClass wrapper = JsonUtility.FromJson<CompleteObjectWrapperClass>(jsonData);
        IDictionary<string, SceneData.CompleteObject> objectsToLoad = wrapper.gameObjects;
        foreach (var objectToLoad in objectsToLoad)
        {
            //continue
        }



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
                await ReAlignTheObjectWithSavedData(gameObjectData);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
        finally
        {
            lockingThread.Release();
        }
       
    }

    private Task ReAlignTheObjectWithSavedData(SceneData.ObjectData gameObjectData)
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

    [Obsolete]
    public async Task SaveGame(string fileName)
    {
        GameObject[] allGameObjectsInTheScene = FindObjectsOfType<GameObject>();
        IDictionary<string, SceneData.CompleteObject> gameData = new Dictionary<string, SceneData.CompleteObject>(); //different approach
        foreach (var gameObject in allGameObjectsInTheScene)
        {
            gameData.Add(gameObject.name, new SceneData.CompleteObject(gameObject, gameObject.transform));
        }
        var completeJson = "{\"objectsToSave\": [" + string.Join(",", gameData) + "]}";
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

    [Obsolete]
    private async void OnApplicationQuit()
    {
        await SaveGame(GetFileLocationToLoad);
    }


    public  Task InvokeListeners(IGameStateHandler[] handlers)
    {
        foreach (var gameObjectState in handlers)
        {
            onCheckpointSaveEvent.AddListener(gameObjectState.GameStateHandler); //we subscribe to the game object
            onCheckpointSaveEvent.Invoke(_sceneData); //gathering all the current state of the object implementing IGameStateHandler
            onCheckpointSaveEvent.RemoveListener(gameObjectState.GameStateHandler); //we de-subscribe until next point
        }
        return Task.CompletedTask;
    }

    public async Task SaveCheckPoint(string fileName)
    {
        gameStateHandlerObjects = GameObjectCreator.GameStateHandlerObjects(); //get all the objects

        await InvokeListeners(gameStateHandlerObjects);

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
}
