using UnityEngine;
using UnityEngine.UI;
using GlobalAccessAndGameHelper;
using System;

public class MusicManager : SubjectsToBeNotified
{
    [SerializeField] Toggle menuToggleSound;

    [SerializeField] AudioSource _bgGameMusic;
    [SerializeField] AudioSource _BossMusic;
    [SerializeField] AudioSource _Pickup;
    GameMusicState _gameState;

    void Start()
    {
        _gameState = GameMusicState.BACKGROUNDMUSIC;
        ChannelMusic(_gameState);

    }

    // Update is called once per frame
    void Update()
    {
        if (menuToggleSound.isOn)
        {
            if (TrackingBosses.BossExists)
            {
                _gameState = GameMusicState.BOSSMUSIC;

            }
            else
            {
                _gameState = GameMusicState.BACKGROUNDMUSIC;

            }

            if (PlayerHelperClassForOtherPurposes.AudioPickUp)
            {
                _gameState = GameMusicState.PICKUP;
            }

            ChannelMusic(_gameState);
        }
        else
        {
            _gameState = GameMusicState.STOP;
            ChannelMusic(_gameState);
        }

    }


    public void ChannelMusic(GameMusicState state)
    {
        switch (state)
        {
            case GameMusicState.BACKGROUNDMUSIC:
                if (!_bgGameMusic.isPlaying && _bgGameMusic.time == 0f)
                {
                    _bgGameMusic.Play();

                }
                _BossMusic.Stop();

                break;


            case GameMusicState.BOSSMUSIC:
                if (!_BossMusic.isPlaying && _BossMusic.time == 0f) //makes sure the same music is not playedagain
                {
                    _BossMusic.Play();

                }

                _bgGameMusic.Stop();

                break;

            case GameMusicState.PICKUP:
                _Pickup.Play();
                PlayerHelperClassForOtherPurposes.AudioPickUp = false;
                break;

            case GameMusicState.STOP:
                _bgGameMusic.Stop();
                _BossMusic.Stop();
                _Pickup.Stop();
                break;
        }



    }


}
