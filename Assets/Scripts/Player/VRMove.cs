using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMove : MonoBehaviour
{
    public bool trackpad_mode;
    public bool trigger_mode;
    public bool keyboard_mode;

    private Vector2 trackpad;
    private float trigger;
    private Vector3 moveDirection;
    private int GroundCount;
    public CapsuleCollider CapCollider;

    public SteamVR_Input_Sources MovementHand; // Set to Get Input From

    public SteamVR_Action_Vector2 TrackpadAction;
    public SteamVR_Action_Boolean JumpAction;
    public SteamVR_Action_Single TriggerSqueeze;

    public static float InitialMovementSpeed; // Used on start and to calculate movement speed
    public float MovementSpeed;
    public float maxSpeed;
    public float speedIncreaseRatio; // Used to calculate increased movement speed over time
    public float Deadzone; // The Deadzone of the trackpad. used to prevent unwanted walking.
    
    public GameObject Head;
    public GameObject IllusionHead;
    public GameObject AxisHand; // Hand Controller GameObject
    public PhysicMaterial NoFrictionMaterial;
    public PhysicMaterial FrictionMaterial;

    public JumpController jumpController;

    public ArduinoController arduino; 

    public float playerHeight = 0;

    // Camera shake variables
    public static float shakeDuration = 0f;     // How long the object should shake for.
    public float shakeAmount = 2f;              // Amplitude of the shake. A larger value shakes the camera harder.
    public float decreaseFactor = 1.0f;
    public GameObject ghost;
    public float ghostShakeAmount;
    public Quaternion originalRotation;         // Store camera rotation before shaking
    public bool justShook = false;

    // Vibration haptics stuff
    public float next_time;
    public bool flip = false;

    // Sound stuff
    public GameObject audioObjectFalling;
    public AudioSource fallingFX;

    public bool started = false;
    public bool rotationChanged = false;
    public Vector3 noVRCameraPosition;

    void Awake() {
        // Subscribe to game state change (this is used for start menu)
        GameManager.OnGameStateChanged += OnOnGameStateChanged;
        JumpController.OnJumpStateChanged += OnOnJumpStateChanged;
    }
    public void Start()
    {
        CapCollider = GetComponent<CapsuleCollider>();

        audioObjectFalling = GameObject.Find("FallingSound");
        fallingFX = audioObjectFalling.GetComponent<AudioSource>();
        originalRotation = Head.transform.localRotation;

        
        arduino.SendManual(20,1500);
        arduino.Disarm();
    } 

    public void OnDestroy(){
        GameManager.OnGameStateChanged -= OnOnGameStateChanged;
        JumpController.OnJumpStateChanged -= OnOnJumpStateChanged;
    }


    private void OnOnGameStateChanged (GameState obj){
        switch (obj){
            case GameState.StartMenu:
                MovementSpeed = 0;
                break;
            case GameState.Running:
                MovementSpeed = InitialMovementSpeed;
                break;
        }
    }

    private void OnOnJumpStateChanged (int jumpState){
        switch (jumpState)
        {
            case 1:
                InitialMovementSpeed = 4;
                maxSpeed = 20f;
                MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);
                Debug.Log("Movement Speed:"+ MovementSpeed.ToString());
                break;
            case 4:
                InitialMovementSpeed = 0.01f;
                maxSpeed = 25f;
                MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);
                Debug.Log("Movement Speed:"+ MovementSpeed.ToString());
                break;
            case 9:
                InitialMovementSpeed = 4;
                maxSpeed = 20f;
                MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);
                Debug.Log("Movement Speed:"+ MovementSpeed.ToString());
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!started && transform.eulerAngles != Vector3.zero) {
            rotationChanged = true;
        }
        if(!started && MovementSpeed != 0) {
            started = true;
            if(!rotationChanged) {
                Head.transform.localEulerAngles = Vector3.zero;
                Head.transform.position = noVRCameraPosition;
            }
        }
        updateInput();
        updateCollider();
        
        Rigidbody RBody = GetComponent<Rigidbody>();

        if (trackpad_mode){
            
             moveDirection = Quaternion.AngleAxis(Angle(trackpad) + AxisHand.transform.localRotation.eulerAngles.y, Vector3.up) * Vector3.forward; // Get the angle of the touch and correct it for the rotation of the controller
             transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed, Space.World);
            
            Vector3 velocity = new Vector3(0, 0, 0);

            if (trackpad.magnitude > Deadzone)
            {
                // make sure the touch isn't in the deadzone and we aren't going too fast
                CapCollider.material = NoFrictionMaterial;
                velocity = - moveDirection;
                if (JumpAction.GetStateDown(MovementHand) && GroundCount > 0)
                {
                    jumpController.jump();
                }
                RBody.AddForce(velocity.x*MovementSpeed - RBody.velocity.x, 0, velocity.z*MovementSpeed - RBody.velocity.z, ForceMode.VelocityChange);
                Debug.Log("Velocity: " + velocity);
                Debug.Log("Movement Direction: " + moveDirection);
            }
            else if (GroundCount > 0){
                CapCollider.material = FrictionMaterial;
            }
        }
        else { // Not Testing Mode

            if (keyboard_mode)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed, Space.World);

                if (GameManager.Instance.State == GameState.StartMenu)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        playerHeight = Head.transform.position.y;
                        Debug.Log("Starting Game");
                        GameManager.Instance.updateGameState(GameState.Running);
                    }
                }
                else
                {
                    MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);

                    // Left / Right movement
                    if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                    {
                        if (this.gameObject.transform.position.x > LevelBoundary.leftSide)
                        {
                            transform.Translate(Vector3.left * Time.deltaTime * 2, Space.World);
                        }

                    }
                    if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                    {
                        if (this.gameObject.transform.position.x < LevelBoundary.rightSide)
                        {
                            transform.Translate(Vector3.right * Time.deltaTime * 2, Space.World);
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        if (trigger > 0 && (jumpController.jumpState != 3))
                        { //&& GroundCount > 0){
                            Debug.Log("Jumping");
                            jumpController.updateJumpState(3);
                            //jumpController.currentlyJumping = true;
                        }
                        else if (trigger == 0 && jumpController.jumpState != 9)
                        {
                            jumpController.updateJumpState(9);
                            //jumpController.currentlyJumping = false;
                        }
                    }
                }

                //transform.Translate(new Vector3(0, velocity, 0) * Time.deltaTime);

                // if (!jumpController.currentlyJumping && jumpController.jumpState == 1)
                // {
                //     playerHeight = Head.transform.position.y;
                // }
            }

            else
            {
                transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed, Space.World);

                if (GameManager.Instance.State == GameState.StartMenu)
                {
                    if (trigger > 0)
                    {
                        Debug.Log("Starting Game");
                        GameManager.Instance.updateGameState(GameState.Running);
                    }
                }
                else
                {
                    MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);

                    if (trigger_mode)
                    {
                        if (trigger > 0 && (jumpController.jumpState != 3))
                        { //&& GroundCount > 0){
                            Debug.Log("Jumping");
                            jumpController.updateJumpState(3);
                            //jumpController.currentlyJumping = true;
                        }
                        else if (trigger == 0 && jumpController.jumpState != 9)
                        {
                            jumpController.updateJumpState(9);
                            //jumpController.currentlyJumping = false;
                        }
                    }
                }

                // if (!jumpController.currentlyJumping)
                // {
                //     playerHeight = Head.transform.position.y;
                // }
            }
        }

        
        if (shakeDuration > 0) // will need to check if is jumping or not by checking jump state
        {
            // shake camera by rotating camera
            // justShook = true;
            
            // if(ghost != null) {
            //     ghost.transform.rotation = originalRotation;
            //     ghost.transform.rotation *= Quaternion.Euler(Random.Range(-ghostShakeAmount,ghostShakeAmount),Random.Range(-ghostShakeAmount,ghostShakeAmount),0);
            // }
            InitialMovementSpeed = 10; // slow down speed of player

            shakeDuration -= Time.deltaTime * decreaseFactor;

            // haptic feedback 
            // if (Time.time > next_time) { 
            //     if (flip) arduino.SendManual(-.1f, 50);
            //     else arduino.SendManual(.1f, 50);
            //     flip = !flip;
            //     next_time = Time.time + 0.05f;
            //     //Debug.Log("Flipping! " + next_time);
            // }
        }
        else if (MudFX.walkingOnMud){
            InitialMovementSpeed = 10; // slow down speed of player
            shakeDuration = 2;

        }
        else
        {
            // used to restore angle
            // if(justShook) {
            //     Head.transform.localRotation = originalRotation;
            //     if(ghost != null) {
            //         ghost.transform.rotation = originalRotation;
            //     }
            //     justShook = false;
            // }
            // originalRotation = Head.transform.localRotation;
            InitialMovementSpeed = 4; // restore speed of player
            shakeDuration = 0f;
        }
    }

    /*
        This function uses logistic function of the form:
        f(x) = c / (1 + ae^(-kx))

        With setting c = 20, a = 4, k = 0.01,  it starts at 4 movespeed and takes about 5 mins to get to 20

    */
    public float timeToSpeed(float initialMS, float time, float timeSpeedRatio, float maxSpeed){
        return maxSpeed / (1 + (initialMS * Mathf.Exp(- timeSpeedRatio * time)));
    }

    public static float Angle (Vector2 p_vector2)
    {

        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else 
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }

    private void updateCollider()
    {
        /*if (!jumpController.currentlyJumping){
            CapCollider.height = Head.transform.localPosition.y;
        }*/
        CapCollider.height = playerHeight;
        
        CapCollider.center = new Vector3(IllusionHead.transform.localPosition.x, IllusionHead.transform.localPosition.y - (CapCollider.height / 2), IllusionHead.transform.localPosition.z);
    }
    //IllusionHead.transform.localPosition.y - 

    private void updateInput()
    {
        trackpad = TrackpadAction.GetAxis(MovementHand);
        trigger = TriggerSqueeze.GetAxis(MovementHand);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GroundCount++;

        if (collision.gameObject.tag == "Obstacle" && jumpController.jumpState >= 3 && jumpController.jumpState < 9) // deadzone for jumping and colliding with obstacle
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true); // bypass all obstacles collider
        }
        else if (collision.gameObject.tag == "Obstacle" && collision.gameObject.name != "pumkin")
        {          
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("Player collided with: " + collision.gameObject.name);
            fallingFX.Play();
            shakeDuration = 1.5f;
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), true); // bypass all obstacles collider
        }

        if (collision.gameObject.name == "Ground Plane")
        {
            Debug.Log("Landng on the ground!!");
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GroundCount--;
        if (shakeDuration <= 0){
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>(), false); // restore all obstacles collider
        }
    }

    /*private void Jump (Rigidbody RBody)
    {
            float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * 9.81f);
            RBody.AddForce(0, jumpSpeed, 0, ForceMode.VelocityChange);
    }*/
}

