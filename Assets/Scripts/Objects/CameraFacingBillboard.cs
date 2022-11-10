using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    Camera referenceCamera;

    void Awake()
    {
        // if no camera referenced, grab the main camera
        if (!referenceCamera)
            referenceCamera = Camera.main;
    }
   
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, referenceCamera.transform.rotation.eulerAngles.y, 0f);

    }
}