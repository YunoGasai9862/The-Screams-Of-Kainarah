using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] Toggle menuToggleSound;
    public enum GameState
    {

        BACKGROUNDMUSIC = 0, BOSSMUSIC=1, PICKUP=2, STOP=3
    }

    [SerializeField] AudioSource _bgGameMusic;
    [SerializeField] AudioSource _BossMusic;
    [SerializeField] AudioSource _Pickup;
    GameState _gameState;

    void Start()
    {
        _gameState = GameState.BACKGROUNDMUSIC;
        ChannelMusic(_gameState);

    }

    // Update is called once per frame
    void Update()
    {
        if(menuToggleSound.isOn)
        {
            if (TrackingBosses.BossExists)
            {
                _gameState = GameState.BOSSMUSIC;

            }
            else
            {
                _gameState = GameState.BACKGROUNDMUSIC;

            }

            if (Movement.AudioPickUp)
            {
                _gameState = GameState.PICKUP;
            }

            ChannelMusic(_gameState);
        }else
        {
            _gameState= GameState.STOP;
            ChannelMusic(_gameState);
        }

    }


    public void ChannelMusic(GameState state)
    {
       switch(state)
        {
            case GameState.BACKGROUNDMUSIC:
                if(!_bgGameMusic.isPlaying && _bgGameMusic.time == 0f)
                {
                    _bgGameMusic.Play();

                }
                _BossMusic.Stop();

                break;


            case GameState.BOSSMUSIC:
                if (!_BossMusic.isPlaying && _BossMusic.time == 0f) //makes sure the same music is not playedagain
                {
                    _BossMusic.Play();

                }

                _bgGameMusic.Stop();

                break;

            case GameState.PICKUP:
                _Pickup.Play();
                Movement.AudioPickUp = false;
                break;

            case GameState.STOP:
                _bgGameMusic.Stop();
                _BossMusic.Stop();
                _Pickup.Stop();
                break;
        }


        
    }

   
}
