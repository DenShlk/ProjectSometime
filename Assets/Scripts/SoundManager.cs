using Hellmade.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip musicClip;
    public float globalVolume = 0.3f;
    public float musicPitchInFrozenTime = 0.3f;

    private void Start()
    {
        EazySoundManager.GlobalVolume = globalVolume;
        EazySoundManager.PlayMusic(musicClip, 1f, true, false);
    }

    private void Update()
    {

        var music = EazySoundManager.GetMusicAudio(musicClip);
        if (GlobalClock.TimeDirection == 0)
            music.Pitch = musicPitchInFrozenTime;
        else
            music.Pitch = GlobalClock.TimeDirection;
    }
}
