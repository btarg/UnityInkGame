using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    AudioSource source;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        // volume options
        float volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        
        volumeSlider.value = volume;
        SetMusicVolume(volume);

        // resolution options : https://www.youtube.com/watch?v=YOaYQrN1oYQ
        resolutions = Screen.resolutions;
        List<String> options = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        SetResolution(PlayerPrefs.GetInt("ResolutionIndex", 0));

    }

    public void SetFullscreen(Toggle fullscreen)
    {
        Screen.fullScreen = fullscreen.isOn;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        PlayerPrefs.SetInt("ResolutionIndex", resolutionIndex);
        Debug.Log("Set resolution index to " + resolutionIndex);
    }

    public void SetMusicVolume(float volume)
    {
        source = GameObject.FindGameObjectWithTag("MusicPlayer").GetComponent<AudioSource>();
        if (source == null)
        {
            return;
        }

        // set the "default volume" to prevent fading from messing with user settings
        source.volume = volume;
        source.gameObject.GetComponent<MusicPlayer>().defaultVolume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
