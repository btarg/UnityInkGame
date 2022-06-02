using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void QuitButton()
    {
        Application.Quit();
    }

    public void LoadButton()
    {
        SceneLoader.GetInstance().LoadFromFile();
    }
}
