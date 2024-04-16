using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundController_Scr : MonoBehaviour
{
    AudioSource audioSrc;

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
    }

    protected void PlaySoundWithVariety(AudioClip clip, float volumePercentileVariability, float pitchPercentileVariability)
    {
        audioSrc.volume = (50 + Random.Range(-volumePercentileVariability, volumePercentileVariability)) / 100;
        audioSrc.pitch = (100 + Random.Range(-pitchPercentileVariability, pitchPercentileVariability)) / 100;
        audioSrc.PlayOneShot(clip);
        audioSrc.volume = 1;
        audioSrc.pitch = 1;
    }
}
