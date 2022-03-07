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
        //Debug.Log(vrCamera.position);
        if (jumpScript.currentlyJumping)
        {
            //if (vrCamera.position.y > jumpScript.startingHeight)
            //{
                float heightOffset = (vrCamera.position.y - moveScript.playerHeight) * heightRatio;
                float newHeight = vrCamera.position.y + heightOffset;
                transform.position = new Vector3(vrCamera.position.x, newHeight, vrCamera.position.z);
            //}

        }
        else
        {
            transform.position = vrCamera.position;
        }
        
        transform.rotation = vrCamera.rotation;
        
    }
}
