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
    public static GameStateManager instance { get; private set; }

    public CheckPointEvent onCheckpointSaveEvent; //checkpoint event
    public EntireSceneSaveEvent onEntireSceneSaveEvent;

    public List<string> jsonSerializedData = new List<string>();

    public IGameStateHandler[] gameStateHandlerObjects;

    public class ObjectDataWrapperClass
    {
        public List<SceneData.ObjectData> objects;
    }
    private  void Awake()
    {
        if(instance == null) //so only instance is there
            instance = this;

    }

    private async void Start()
    {
        await LoadGame();
    }

    public static void ChangeLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex + 1);
    }

    public async Task LoadGame() //implement LoadGame with Json etc by saving states
    {
        if (this._sceneData == null)
        {
            Debug.Log("No data found, initializing everything to default");
            await NewGame();
        }

    }
    public Task LoadLastCheckPoint(string saveFileName)
    {
        var saveFilePath = Path.Combine(Application.persistentDataPath, saveFileName);
        var jsonData = File.ReadAllText(saveFilePath);
        var wrappedJsonData = "{\"objects\":" + jsonData + "}";
        Debug.Log(wrappedJsonData);
        try
        {
            ObjectDataWrapperClass wrapper = JsonUtility.FromJson<ObjectDataWrapperClass>(wrappedJsonData);
            Debug.Log(wrapper.objects.Count);
            List<SceneData.ObjectData> savedData = wrapper.objects;
            foreach(var gameObjectData in savedData)
            {
                Debug.Log(gameObjectData);
            }

        }catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }

        return Task.CompletedTask;


    }
    public async Task SaveGame(SceneData sceneData)
    {
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

    private async void OnApplicationQuit()
    {
        await SaveGame(this._sceneData);
    }


    public Task InvokeListeners(IGameStateHandler[] handlers)
    {
        foreach (var gameObjectState in handlers)
        {
            onCheckpointSaveEvent.AddListener(gameObjectState.GameStateHandler); //we subscribe to the game objects

            onCheckpointSaveEvent.Invoke(_sceneData); //gathering all the current state of the objects implementing IGameStateHandler
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
        File.WriteAllText(localFilename, completeJson);
        jsonSerializedData.Clear(); //doesn't store old data
        await LoadLastCheckPoint(fileName);

    }
}
