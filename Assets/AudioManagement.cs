using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagement : MonoBehaviour
{

    public AudioSource master;
    public AudioSource[] slaves;

    //public AudioClip[] clips;

    private IEnumerator SyncSources()
    {
        while (true)
        {
            foreach (var slave in slaves)
            {
                slave.timeSamples = master.timeSamples;
                yield return null;
            }
        }
    }

    public void playTrack(int trackNumber)
    {
        //slaves[trackNumber].GetComponent<AudioClip>();
        slaves[trackNumber].Play();
    }
    public void stopTrack(int trackNumber)
    {
        //slaves[trackNumber].GetComponent<AudioClip>().Stop();
        slaves[trackNumber].Stop();
    }
}
