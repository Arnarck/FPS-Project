using UnityEngine;



[System.Serializable]
struct FMovement_Settings
{
    public float friction;
    public float acceleration;
    public float run_multiplier;

    // Each struct already has a default constructor that initializes the object to zero. Therefore, the constructors that you can create for a struct must take one or more parameters.
    public FMovement_Settings(float friction = 15f, float acceleration = 100f, float run_multiplier = 2.5f)
    {
        this.friction = friction;
        this.acceleration = acceleration;
        this.run_multiplier = run_multiplier;
    }
}

[System.Serializable]
struct FCamera_Settings
{
    public float friction;
    public float acceleration;
    public float vertical_sensitivity;
    public float horizontal_sensitivity;
    public float min_yall;
    public float max_yall;

    public FCamera_Settings(float friction = 15f, float acceleration = 100f, float vertical_sensitivity = 3f, float horizontal_sensitivity = 2f, float min_yall = -60f, float max_yall = 60f)
    {
        this.friction = friction;
        this.acceleration = acceleration;
        this.horizontal_sensitivity = horizontal_sensitivity;
        this.vertical_sensitivity = vertical_sensitivity;
        this.min_yall = min_yall;
        this.max_yall = max_yall;
    }
}



public class Base_FPCharacterController : MonoBehaviour
{
    Vector3 dp, camera_dp, camera_start_rotation;

    [HideInInspector] public Vector3 camera_look;
    [HideInInspector] public Transform m_transform;

    public Transform fp_camera;
    public Base_CrossPlatformInput input;

    [Space]
    [SerializeField] FMovement_Settings movement_settings = new FMovement_Settings(15f, 100f);

    [Space]
    [SerializeField] FCamera_Settings camera_settings = new FCamera_Settings(15f, 100f, 3f, 2f, -60f, 60f);
    


    private void Awake()
    {
        m_transform = transform;
    }

    void Start()
    {
        camera_look = fp_camera.localEulerAngles;
        camera_start_rotation = camera_look;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float dt = Time.deltaTime;

        { // Character Movement
            Vector3 ddp;
            Vector3 movement_input = get_movement_input();
            Vector3 movement_direction = m_transform.forward * movement_input.z + m_transform.right * movement_input.x;

            ddp = movement_direction;
            ddp *= movement_settings.acceleration;
            if (input.get_key(EInput_State.HOLDING, EInput_Action.RUN) && movement_input.z > 0f && movement_input.x == 0f) ddp *= movement_settings.run_multiplier; // Pressing "Run" key and moving forward
            ddp -= dp * movement_settings.friction;

            dp += ddp * dt;

            m_transform.localPosition += (dp * dt) + (ddp * dt * dt * 0.5f);
        }

        { // Camera movement
            Vector3 ddp;
            Vector3 displacement;
            Vector3 camera_direction = input.camera_look_direction();

            camera_direction.x *= camera_settings.horizontal_sensitivity;
            camera_direction.y *= camera_settings.vertical_sensitivity;

            ddp = camera_direction;
            ddp *= camera_settings.acceleration;
            ddp -= camera_dp * camera_settings.friction;

            camera_dp += ddp * dt;
            displacement = (camera_dp * dt) + (ddp * dt * dt * 0.5f);

            camera_look += displacement;

            // Clamp rotation betweeen 0 to 360
            camera_look.y = Mathf.Clamp(camera_look.y, camera_settings.min_yall, camera_settings.max_yall);
            if (camera_look.x < 0f) camera_look.x += 360f;
            else if (camera_look.x >= 360f) camera_look.x -= 360f;

            // Apply rotation;
            fp_camera.localRotation = Quaternion.Euler(Vector3.right * -camera_look.y); // Rotates the camera vertically
            m_transform.localRotation = Quaternion.Euler(Vector3.up * camera_look.x); // Rotates the player itself horizontally
        }
    }

    public Vector3 get_movement_input()
    {
        Vector3 movement_input = Vector3.zero;

        if (input.get_key(EInput_State.HOLDING, EInput_Action.MOVE_FORWARD)) movement_input.z += 1f;
        if (input.get_key(EInput_State.HOLDING, EInput_Action.MOVE_BACKWARDS)) movement_input.z -= 1f;
        if (input.get_key(EInput_State.HOLDING, EInput_Action.MOVE_LEFT)) movement_input.x -= 1f;
        if (input.get_key(EInput_State.HOLDING, EInput_Action.MOVE_RIGHT)) movement_input.x += 1f;

        if (movement_input.x != 0f && movement_input.z != 0f) movement_input *= .5f; // Constraint movement if two axis are being pressed

        return movement_input;
    }
}