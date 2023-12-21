using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameStateManager : MonoBehaviour, IGameState
{
    private SceneData _sceneData;
    public static GameStateManager instance { get; private set; }

    public CheckPointEvent onCheckpointSaveEvent; //checkpoint event
    public EntireSceneSaveEvent onEntireSceneSaveEvent;

    public IGameStateHandler[] gameStateHandlerObjects;

    public SemaphoreSlim SemaphoreSlim { get; private set; }

    private  void Awake()
    {
        if(instance == null) //so only instance is there
            instance = this;

        SemaphoreSlim = new SemaphoreSlim(1);
    }

    private async void Start()
    {
        await LoadGame();
    }
    private async void Update()
    {
       await SemaphoreSlim.WaitAsync();
       gameStateHandlerObjects = await GameObjectCreator.GameStateHandlerObjects(SemaphoreSlim, 3); //keep polling for them each second?
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

    public Task SaveCheckPoint(SceneData sceneData)
    {
        onCheckpointSaveEvent.Invoke(sceneData); //calling event
        return Task.CompletedTask;

    }
}
