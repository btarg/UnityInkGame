using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    private const string resourcePath = "Audio/";
    private Dictionary<string, AudioClip> audioClips;
    private AudioClip[] loadedClips;

    private AudioSource audioSource;

    private void Start()
    {
        // load all the audio clips in resources
        loadedClips = Resources.LoadAll<AudioClip>(resourcePath);
        audioSource = gameObject.AddComponent<AudioSource>();

    }

    public AudioClip GetAudioClip(string clipName)
    {
        foreach (AudioClip clip in loadedClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }
        return null;
    }

    public void Play(string clipName, bool createNewObject = false, float volume = 1.0f, float pitch = 1.0f, float delay = 0f)
    {
        AudioClip clip = GetAudioClip(clipName);

        if (createNewObject) {
            PlayClipInNewObject(clip, volume, pitch, delay);
        } else {
            PlayClip(clip, volume, pitch, delay);
        }
        
    }

    public void PlayClip(AudioClip clip, float volume, float pitch, float delay)
    {
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.PlayDelayed(delay);
    }

    public void PlayClipInNewObject(AudioClip clip, float volume, float pitch, float delay = 0f)
    {
        // Create a new object with an audioSource
        GameObject soundGameObject = new GameObject("_SoundPlayer");
        AudioSource newAudioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.pitch = pitch;

        // Destroy object once the sound is played
        audioSource.PlayDelayed(delay);
        GameObject.Destroy(soundGameObject, delay + audioSource.clip.length);
    }

    public void PlayRandom(string[] randomClips, float volume = 1.0f, float pitch = 1.0f, float delay = 0f)
    {
        int chosenIndex = Random.Range(0, randomClips.Length);
        Play(randomClips[chosenIndex], true, volume, pitch, delay);
    }

    public void PlayRandomClip(AudioClip[] randomClips, float volume = 1.0f, float pitch = 1.0f, float delay = 0f)
    {
        int chosenIndex = Random.Range(0, randomClips.Length);
        PlayClip(randomClips[chosenIndex], volume, pitch, delay);
    }
}
