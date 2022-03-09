using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionCameraController : MonoBehaviour
{
    public Transform vrCamera;
    public JumpController jumpScript;
    public VRMove moveScript;
    public float heightRatio = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Side to side "stretching"
        float rig_offset = 6.0f;
        float x_offset_ratio = 2.0f; // This alters how much we extend the player's side-to-side movement
        float new_x = (vrCamera.position.x - rig_offset) * x_offset_ratio + rig_offset;
        if (jumpScript.currentlyJumping)
        {

            float heightOffset = (vrCamera.position.y - moveScript.playerHeight) * jumpScript.jumpRatio;

            float newHeight = moveScript.playerHeight + heightOffset;//vrCamera.position.y + heightOffset;
            //Debug.Log("vrCamera.position.x: " + vrCamera.position.x);
            //Debug.Log("Illusion Pos: " + (vrCamera.position.x * x_offset_ratio));
            transform.position = new Vector3(new_x, newHeight, vrCamera.position.z);

        }
        else
        {
            //Debug.Log("vrCamera.position.x: " + vrCamera.position.x);
            //Debug.Log("Illusion Pos: " + (vrCamera.position.x * x_offset_ratio));
            transform.position = new Vector3(new_x, vrCamera.position.y, vrCamera.position.z);
        }
        
        transform.rotation = vrCamera.rotation;
        
    }
}
