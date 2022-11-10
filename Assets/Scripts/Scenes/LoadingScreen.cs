using System.Net.Mime;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen instance;
    public GameObject fadeObject;
    public bool isLoading = false;

    public GameObject loadingScreen;
    public Slider loadingBarFill;
    public TextMeshProUGUI progressLabel;

    public UnityEvent onStartedLoading;
    public UnityEvent onFinishedLoading;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Loading Screen in the scene");
        }
        else
        {
            instance = this;
        }

    }

    private void Start() {
        loadingScreen.SetActive(false);
    }

    public static LoadingScreen GetInstance()
    {
        return instance;
    }

    public void LoadScene(string sceneName, bool fade = true)
    {
        if (!isLoading)
            StartCoroutine(LoadSceneAsync(sceneName, fade));
    }

    IEnumerator LoadSceneAsync(string sceneName, bool fade = true)
    {
        Debug.LogWarning("Async Loading Scene: " + sceneName);

        if (fadeObject != null && fade)
        {
            fadeObject.GetComponent<Animator>().SetTrigger("fadeIn");
            yield return new WaitForSeconds(.5f);
        }

        fadeObject.GetComponent<Image>().enabled = false;

        loadingScreen.SetActive(true);
        isLoading = true;
        PauseMenu.canPause = false;

        yield return new WaitForEndOfFrame();
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        onStartedLoading.Invoke();

        float progress = 0f;
        float lerpSpeed = 3f;
        while (!operation.isDone)
        {
            // lerp so the game doesn't look frozen
            progress = Mathf.MoveTowards(progress, operation.progress, Time.unscaledDeltaTime * lerpSpeed);

            loadingBarFill.value = progress;

            // add 10 because 90% is unity's equivalent of 100
            progressLabel.text = (Mathf.CeilToInt(progress * 100f) + 10).ToString() + "%";

            if (progress >= 0.9f && !operation.allowSceneActivation)
            {
                operation.allowSceneActivation = true;
                loadingScreen.SetActive(false);
                isLoading = false;
                PauseMenu.canPause = true;

                if (fadeObject != null && fade)
                {
                    fadeObject.GetComponent<Animator>().SetTrigger("fadeOut");
                }
                onFinishedLoading.Invoke();
                
            }
            yield return null;
        }
        

    }

}
