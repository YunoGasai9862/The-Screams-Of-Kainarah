using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _bgGameMusic;
    [SerializeField] AudioSource _BossMusic;
    private bool _isPlaying = false;

    void Start()
    {
        _bgGameMusic.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (TrackingBosses.BossExists)
        {
            PlayBossMusic(_isPlaying);
            _isPlaying = true;

        }else
        {
            _isPlaying = false;
            PlayBGMusic(_isPlaying);
            _isPlaying = true;
        } 
        
       

    }


    public void PlayBossMusic(bool isPlaying)
    {
        if (!isPlaying)
        {
            _BossMusic.Play();
            _bgGameMusic.Stop();
        }
    }

    public void PlayBGMusic(bool isPlaying)
    {
        if(!isPlaying) {
            _BossMusic.Stop();
            _bgGameMusic.Play();

        }
       
    }
}
