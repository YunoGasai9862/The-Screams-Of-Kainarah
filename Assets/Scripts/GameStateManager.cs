using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    public async Task LoadLastCheckPoint()
    {
        await Task.CompletedTask;

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

    public async Task SaveCheckPoint()
    {
        gameStateHandlerObjects = GameObjectCreator.GameStateHandlerObjects(); //get all the objects

        await InvokeListeners(gameStateHandlerObjects);

        foreach (var objectToSave in this._sceneData.ObjectsToPersit)
        {
            var jsonObject = JsonUtility.ToJson(objectToSave);
            jsonSerializedData.Add(jsonObject);
        }
        var completeJson = "[" + string.Join(",", jsonSerializedData) + "]"; //joing them in a single file

        string fileName = Path.Combine(Application.persistentDataPath, "SaveData.json");

        File.WriteAllText(fileName, completeJson);

    }
}
