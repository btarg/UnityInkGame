using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    private static LoadingScreen instance;
    public bool isLoading = false;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Loading Screen in the scene");
        }
        instance = this;
        DontDestroyOnLoad(this);

    }
    public static LoadingScreen GetInstance()
    {
        return instance;
    }

    public GameObject loadingScreen;
    public Slider loadingBarFill;
    public TextMeshProUGUI progressLabel;

    private void Start() {
        loadingScreen.SetActive(false);
    }

    public void LoadScene(string sceneName) {
        isLoading = true;
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName) {

        loadingScreen.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        PauseMenu.canPause = false;

        while (!operation.isDone) {
            
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            loadingBarFill.value = progress;

            progressLabel.text = Mathf.CeilToInt(operation.progress * 100f).ToString() + "%";

            yield return null;

        }

        loadingScreen.SetActive(false);
        isLoading = false;
        PauseMenu.canPause = true;

    }

}
