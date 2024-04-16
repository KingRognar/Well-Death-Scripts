using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureSoundController_Scr : SoundController_Scr
{
    public List<AudioClip> grunts = new List<AudioClip>();
    public List<AudioClip> swipes = new List<AudioClip>();
    public List<AudioClip> hits = new List<AudioClip>();

    public void PlayGrunt()
    {
        PlaySoundWithVariety(grunts[Random.Range(0, grunts.Count)], 5f, 5f);
    }
    public void PlaySwipe()
    {
        PlaySoundWithVariety(swipes[Random.Range(0, swipes.Count)], 5f, 5f);
    }
    public void PlayHit()
    {
        PlaySoundWithVariety(hits[Random.Range(0, hits.Count)], 5f, 5f);
    }
}
