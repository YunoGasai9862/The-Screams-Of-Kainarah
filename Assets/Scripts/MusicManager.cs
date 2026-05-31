using Assets.Annotations;
using System.Collections;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine;
using UnityEngine.UI;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(MusicManager), SubjectType = typeof(PlayerActionRelayer), ContextType = typeof(bool))]
public class MusicManager : MonoBehaviour, INotify<bool>
{
    [SerializeField] Toggle menuToggleSound;
    [SerializeField] AudioSource _bgGameMusic;
    [SerializeField] AudioSource _BossMusic;
    [SerializeField] AudioSource _Pickup;
    GlobalEnums.GameMusicState _gameState;

    private bool shouldPlayPickUpAudio;

    private Delegator Delegator { get; set; }

    private async void Awake()
    {
        StartCoroutine(Helper.GetDelegator<Delegator>(value => Delegator = value));

         Delegator.NotifySubjectWrapper(new ObserverContext<bool>()
        {
            Instance = gameObject,
            EntityType = typeof(MusicManager),
            SubjectType = typeof(PlayerActionRelayer),
        }, this);
    }
    void Start()
    {
        _gameState = GlobalEnums.GameMusicState.BACKGROUNDMUSIC;
        ChannelMusic(_gameState);
    }

    void Update()
    {
        if (menuToggleSound.isOn)
        {
            if (TrackingBosses.BossExists)
            {
                _gameState = GlobalEnums.GameMusicState.BOSSMUSIC;

            }
            else
            {
                _gameState = GlobalEnums.GameMusicState.BACKGROUNDMUSIC;

            }

            if (shouldPlayPickUpAudio)
            {
                _gameState = GlobalEnums.GameMusicState.PICKUP;
            }

            ChannelMusic(_gameState);
        }
        else
        {
            _gameState = GlobalEnums.GameMusicState.STOP;
            ChannelMusic(_gameState);
        }

    }
    public void ChannelMusic(GlobalEnums.GameMusicState state)
    {
        switch (state)
        {
            case GlobalEnums.GameMusicState.BACKGROUNDMUSIC:

                if (MovementHelperFunctions.boolConditionAndTester(!_bgGameMusic.isPlaying, _bgGameMusic.time == 0f))
                {
                    _bgGameMusic.Play();
                }
                _BossMusic.Stop();
                break;

            case GlobalEnums.GameMusicState.BOSSMUSIC:
                if (MovementHelperFunctions.boolConditionAndTester(!_BossMusic.isPlaying, _BossMusic.time == 0f)) //makes sure the same music is not playedagain
                {
                    _BossMusic.Play();
                }
                _bgGameMusic.Stop();
                break;

            case GlobalEnums.GameMusicState.PICKUP:
                _Pickup.Play();
                shouldPlayPickUpAudio = false;
                break;

            case GlobalEnums.GameMusicState.STOP:
                _bgGameMusic.Stop();
                _BossMusic.Stop();
                _Pickup.Stop();
                break;
        }

    }

    public IEnumerator Notify(bool value)
    {
        shouldPlayPickUpAudio = value;

        yield return null;
    }
}
