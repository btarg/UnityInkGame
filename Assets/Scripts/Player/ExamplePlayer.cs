using UnityEngine;
using KinematicCharacterController;


public class ExamplePlayer : MonoBehaviour
{
    public ExampleCharacterController Character;
    public ExampleCharacterCamera CharacterCamera;


    private PlayerControls controls;
    private bool isCrouching = false;

    private float crouchSpeed = 4.0f;
    private float crouchedFraming;

    public bool canMove = true;


    private void Start()
    {

        // Tell camera to follow transform
        CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CharacterCamera.IgnoredColliders.Clear();
        CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());

        controls = new PlayerControls();
        controls.Enable();

        crouchedFraming = CharacterCamera.FollowPointFraming.y - (Character.CrouchedCapsuleHeight - 0.25f);

    }

    private void Update()
    {
        if (!canMove || DialogueManager.GetInstance().dialogueIsPlaying || LoadingScreen.GetInstance().isLoading)
            return;

        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        Vector2 moveVector = controls.FirstPerson.Move.ReadValue<Vector2>();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = moveVector.y;
        characterInputs.MoveAxisRight = moveVector.x;
        characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
        characterInputs.JumpDown = controls.FirstPerson.Jump.WasPressedThisFrame();


        isCrouching = controls.FirstPerson.Crouch.IsPressed();
        characterInputs.CrouchDown = isCrouching;


        // Apply inputs to character
        Character.SetInputs(ref characterInputs);

        // if crouch is being held or if there is an obstruction, the capsule height will be set to the crouched height
        if (isCrouching || Character.isObstructed)
        {
            CharacterCamera.FollowPointFraming.y = Mathf.MoveTowards(CharacterCamera.FollowPointFraming.y, crouchedFraming, Time.deltaTime * crouchSpeed);
        }
        else
        {
            CharacterCamera.FollowPointFraming.y = Mathf.MoveTowards(CharacterCamera.FollowPointFraming.y, 0f, Time.deltaTime * crouchSpeed);
        }


    }

    private void LateUpdate()
    {
        if (!canMove || DialogueManager.GetInstance().dialogueIsPlaying || LoadingScreen.GetInstance().isLoading)
            return;

        // Handle rotating the camera along with physics movers
        if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        {
            CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
        }

        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        Vector2 cameraInput = controls.FirstPerson.Camera.ReadValue<Vector2>();

        float mouseLookAxisUp = cameraInput.y;
        float mouseLookAxisRight = cameraInput.x;
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        CharacterCamera.UpdateWithInput(Time.deltaTime, 0f, lookInputVector);
        CharacterCamera.TargetDistance = 0f;

    }

}
