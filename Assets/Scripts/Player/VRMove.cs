using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRMove : MonoBehaviour
{
    public bool testing_mode;

    private Vector2 trackpad;
    private float trigger;
    private Vector3 moveDirection;
    private int GroundCount;
    public CapsuleCollider CapCollider;

    public SteamVR_Input_Sources MovementHand; // Set to Get Input From

    public SteamVR_Action_Vector2 TrackpadAction;
    public SteamVR_Action_Boolean JumpAction;
    public SteamVR_Action_Single TriggerSqueeze;

    public float MovementSpeed;
    public float Deadzone; // The Deadzone of the trackpad. used to prevent unwanted walking.
    
    public GameObject Head;
    public GameObject AxisHand; // Hand Controller GameObject
    public PhysicMaterial NoFrictionMaterial;
    public PhysicMaterial FrictionMaterial;

    public JumpController jumpController;

    public float playerHeight = 0;
    
    public void Start()
    {
        CapCollider = GetComponent<CapsuleCollider>();
    } 

    // Update is called once per frame
    void Update()
    {
        updateInput();
        updateCollider();
        
        Rigidbody RBody = GetComponent<Rigidbody>();

        if (testing_mode){
            
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

            if (JumpAction.GetStateDown(MovementHand) && GroundCount > 0)
            {
                jumpController.jump();
            }

            Debug.Log("Trigger: " + trigger);
            if (trigger > 0 && GroundCount > 0){
                playerHeight = Head.transform.localPosition.y;
                jumpController.currentlyJumping = true;
            }
            else {
                jumpController.currentlyJumping = false;
            }
            Debug.Log("JUMP STATUS: " + jumpController.currentlyJumping);
        }
       
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
        if (!jumpController.currentlyJumping){
            CapCollider.height = Head.transform.localPosition.y;
        }
        
        CapCollider.center = new Vector3(Head.transform.localPosition.x, Head.transform.localPosition.y / 2, Head.transform.localPosition.z);
    }


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

