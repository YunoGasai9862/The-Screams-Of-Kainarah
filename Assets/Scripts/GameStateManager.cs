using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

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

    public List<IGameStateHandler> gameStateHandlerObjects;

    public string GetFileLocationToLoad { get => _fileName; set => _fileName = value; }

    public class ObjectDataWrapperClass
    {
        public List<SceneData.ObjectData> objectsToSave;
    }
    private async void Awake()
    {
        if(instance == null) //so only instance is there
            instance = this;

        if (this._sceneData == null)
        {
            Debug.Log("No data found, initializing everything to default");
            await NewGame();
        }
    }
    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }

    public async Task LoadGame(string saveFileName, Semaphore lockingThread) //implement LoadGame with Json etc by saving states
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
            }
            else
            {
                foundObject.transform.position = objectToLoad.transform.position;
            }
        }
        await Task.CompletedTask;
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
        catch (System.Exception ex)
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


    public  Task InvokeListeners(List<IGameStateHandler> handlers)
    {
        foreach (var gameObjectState in handlers)
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
        catch (System.Exception e)
        {
            Debug.Log(e.Message);

        }

    }
}
