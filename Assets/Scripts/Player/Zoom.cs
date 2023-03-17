using UnityEngine;

[RequireComponent(typeof(Camera))]
public class Zoom : MonoBehaviour
{
    float zoom = 15f;
    float normal;
    //float normalSens = 1.0f;
    //public float sensitivityMultiplier = 0.5f;
    public int smooth = 10;
    bool isZoomed = false;
    private Camera cam;

    private PlayerControls controls;

    // Use this for initialization
    void Start()
    {
        controls = new PlayerControls();
        controls.Enable();

        cam = GetComponent<Camera>();
        //normalSens = player.m_LookSensitivity;

    }

    void UpdateFOV()
    {
        float setFOV = PlayerPrefs.GetFloat("FOV", 0f);
        if (setFOV == 0f)
        {
            return;
        }
        normal = setFOV;
        zoom = setFOV / 4f;
    }

    // Update is called once per frame
    void Update()
    {
        if (DialogueManager.GetInstance().dialogueIsPlaying || PauseMenu.GameIsPaused) {
            return;
        }

        UpdateFOV();

        isZoomed = controls.FirstPerson.Zoom.ReadValue<float>() > 0f;
        if (isZoomed)
        {
            // Zoom in the camera (Gradually increases FOV)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, zoom, Time.deltaTime * smooth);
            //player.m_LookSensitivity = normalSens * sensitivityMultiplier;
        }
        else
        {
            // Revert FOV (Zoom out)
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, normal, Time.deltaTime * smooth);
            //player.m_LookSensitivity = normalSens;
        }
    }
}
