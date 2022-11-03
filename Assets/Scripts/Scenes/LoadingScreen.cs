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
        StartCoroutine(LoadSceneAsync(sceneName, fade));
    }

    IEnumerator LoadSceneAsync(string sceneName, bool fade = true)
    {
        Debug.Log("Loading Scene: " + sceneName);

        // i dont know what the fuck is wrong with this
        // this is dumb

        if (fadeObject != null && fade)
        {
            yield return new WaitForSeconds(.5f);
            fadeObject.GetComponent<Animator>().SetTrigger("fadeIn");
        }

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
            progress = Mathf.MoveTowards(progress, operation.progress + 0.1f, Time.deltaTime * lerpSpeed);

            loadingBarFill.value = progress;

            progressLabel.text = Mathf.CeilToInt(progress * 100f).ToString() + "%";

            if (progress >= 0.9f && !operation.allowSceneActivation)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                operation.allowSceneActivation = true;
                loadingScreen.SetActive(false);
                isLoading = false;
                PauseMenu.canPause = true;

                if (fadeObject != null)
                {
                    fadeObject.GetComponent<Animator>().SetTrigger("fadeOut");
                }
                onFinishedLoading.Invoke();
                
            }

            yield return null;

        }
        yield return null;

    }

}
