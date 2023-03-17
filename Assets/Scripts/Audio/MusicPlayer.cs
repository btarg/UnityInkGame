using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{

    public List<AudioClip> soundtrack;

    [Header("Fade")]
    public float fadeTime = 2f;
    private float startFadeOutOffset = 1f;

    public float defaultVolume = 1f;
    AudioSource source;
    List<AudioClip> played = new List<AudioClip>();

    private void Start()
    {
        source = gameObject.GetComponent<AudioSource>();
        defaultVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    }

    private void Awake()
    {
        StartCoroutine(StartPlaying());
    }

    private void Update()
    {
        defaultVolume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    }

    IEnumerator StartPlaying()
    {
        // wait before starting music
        yield return new WaitForSeconds(1);
        PlayNextSong();
    }

    public void Pause()
    {
        if (source == null)
            return;
        source.Pause();
    }
    public void Unpause()
    {
        if (source == null)
            return;
        source.UnPause();
    }

    public void FadeOutCurrentSong()
    {
        // fade out last song if there was one playing
        if (source.isPlaying)
        {
            Debug.Log("fading out audio...");
            StartCoroutine(AudioHelper.FadeOut(source, fadeTime));
        }
    }

    public void PlayNextSong()
    {

        // if there are no more possibilities then reset the soundtrack
        if (soundtrack.Count == 0)
        {
            Debug.Log("Resetting music player...");
            soundtrack = played;
            played.Clear();
        }

        AudioClip clip = soundtrack.ToArray()[Random.Range(0, soundtrack.Count - 1)];

        Debug.Log("Now Playing: " + clip.name);

        source.clip = clip;

        // Remove played songs from the list of possibilities
        played.Add(clip);
        soundtrack.Remove(clip);

        // fade audio in
        StartCoroutine(AudioHelper.FadeIn(source, fadeTime, defaultVolume));

        // invoke fade out effect and new song later
        startFadeOutOffset = source.clip.length / 8;
        Invoke("FadeOutCurrentSong", source.clip.length - startFadeOutOffset);
        Invoke("PlayNextSong", source.clip.length);
    }
}
