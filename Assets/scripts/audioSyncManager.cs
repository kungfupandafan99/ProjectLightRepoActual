using UnityEngine;
using System;

public class audioSyncManager : MonoBehaviour
{
    public static audioSyncManager instance;

    public float bpm = 140f; // Beats per minute of the music
    public AudioSource music;

    float secondsPerBeat;
    double nextBeatTime;
    int beatCount = 0;

    public event Action OnBeat; // Event triggered on each beat
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        
        instance = this;
        secondsPerBeat = 60f / bpm;
    }
    void Start()
    {
        nextBeatTime = AudioSettings.dspTime + secondsPerBeat; // Schedule the first beat   
        music.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(AudioSettings.dspTime >= nextBeatTime)
        {
            OnBeat?.Invoke(); // Trigger the beat event
            beatCount++;
            nextBeatTime += secondsPerBeat; // Schedule the next beat
        }
    }
    public void SetBPM(float newBPM)
    {
        bpm = newBPM;
        secondsPerBeat = 60f / bpm;
        music.pitch = newBPM / 140f;
    }
    public int getCurrentBeat()
    {
        return beatCount % 4;
    }
    
}
