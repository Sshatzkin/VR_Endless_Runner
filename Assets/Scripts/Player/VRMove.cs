using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMove : MonoBehaviour
{
    public bool trackpad_mode;
    public bool trigger_mode;

    private Vector2 trackpad;
    private float trigger;
    private Vector3 moveDirection;
    private int GroundCount;
    public CapsuleCollider CapCollider;

    public SteamVR_Input_Sources MovementHand; // Set to Get Input From

    public SteamVR_Action_Vector2 TrackpadAction;
    public SteamVR_Action_Boolean JumpAction;
    public SteamVR_Action_Single TriggerSqueeze;

    public float InitialMovementSpeed; // Used on start and to calculate movement speed
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

    public float playerHeight = 0;
    
    void Awake() {
        // Subscribe to game state change (this is used for start menu)
        GameManager.OnGameStateChanged += OnOnGameStateChanged;
    }
    public void Start()
    {
        CapCollider = GetComponent<CapsuleCollider>();
    } 

    public void OnDestroy(){
        GameManager.OnGameStateChanged -= OnOnGameStateChanged;
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

    // Update is called once per frame
    void Update()
    {
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

            transform.Translate(Vector3.forward * Time.deltaTime * MovementSpeed, Space.World);

            if (GameManager.Instance.State == GameState.StartMenu ){
                if (trigger > 0){
                    Debug.Log("Starting Game");
                    GameManager.Instance.updateGameState(GameState.Running);
                }
            }
            else {
                MovementSpeed = timeToSpeed(InitialMovementSpeed, GameManager.Instance.time, speedIncreaseRatio, maxSpeed);

                if (trigger_mode){
                    if (trigger > 0 && (jumpController.jumpState != 3)) { //&& GroundCount > 0){
                        Debug.Log("Jumping");
                        jumpController.updateJumpState(3);
                        //jumpController.currentlyJumping = true;
                    }
                    else if (trigger == 0 && jumpController.jumpState != 9) {
                        jumpController.updateJumpState(9);
                        //jumpController.currentlyJumping = false;
                    }
                }
            }
            
            if (! jumpController.currentlyJumping){
                playerHeight = Head.transform.position.y;
            }
        }
       
    }

    /*
        This function uses logistic function of the form:
        f(x) = c / 1 + ae^(-kx)

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
    }

    private void OnCollisionExit(Collision collision)
    {
        GroundCount--;
    }

    /*private void Jump (Rigidbody RBody)
    {
            float jumpSpeed = Mathf.Sqrt(2 * jumpHeight * 9.81f);
            RBody.AddForce(0, jumpSpeed, 0, ForceMode.VelocityChange);
    }*/
}

