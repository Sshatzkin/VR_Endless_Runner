using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionCameraController : MonoBehaviour
{
    public Transform vrCamera;
    public JumpController jumpScript;
    public VRMove moveScript;
    public float heightRatio = 1;

    public float x_offset_ratio = 1;//2.0f; // This alters how much we extend the player's side-to-side movement


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Side to side "stretching"
        float rig_offset = 6.0f;
        float new_x = (vrCamera.position.x - rig_offset) * x_offset_ratio + rig_offset;
        if (jumpScript.currentlyJumping || jumpScript.jumpTimeElapsed > 0)
        {

            float heightOffset = (vrCamera.position.y - moveScript.playerHeight) * jumpScript.jumpRatio;
            float ghostHeightOffset = heightOffset;
            if(jumpScript.jumpRatio > 2.5) {
                ghostHeightOffset *= 4f / 10f;
            }

            float newHeight = moveScript.playerHeight + heightOffset;//vrCamera.position.y + heightOffset;
            //Debug.Log("vrCamera.position.x: " + vrCamera.position.x);
            //Debug.Log("Illusion Pos: " + (vrCamera.position.x * x_offset_ratio));
            transform.position = new Vector3(new_x, Mathf.Max(newHeight, moveScript.playerHeight), vrCamera.position.z);
            GhostMove ghostScript = transform.GetComponentInChildren<GhostMove>();
            if(ghostScript) {
                ghostScript.transform.position = ghostScript.deltaPosition + new Vector3(new_x, Mathf.Max(moveScript.playerHeight + ghostHeightOffset, moveScript.playerHeight), vrCamera.position.z);
            }
        }
        else
        {
            //Debug.Log("vrCamera.position.x: " + vrCamera.position.x);
            //Debug.Log("Illusion Pos: " + (vrCamera.position.x * x_offset_ratio));
            transform.position = new Vector3(new_x, vrCamera.position.y, vrCamera.position.z);
            GhostMove ghostScript = transform.GetComponentInChildren<GhostMove>();
            if(ghostScript) {
                ghostScript.transform.position = transform.position + ghostScript.deltaPosition;
            }
        }
        GameObject ghost = moveScript.ghost;
        transform.rotation = vrCamera.rotation;
        if(ghost != null) {
            ghost.transform.rotation = vrCamera.rotation;
        }
        if(VRMove.shakeDuration > 0) {
            transform.rotation *= Quaternion.Euler(Random.Range(-moveScript.shakeAmount,moveScript.shakeAmount),Random.Range(-moveScript.shakeAmount,moveScript.shakeAmount),0);
            if(ghost != null) {
                ghost.transform.rotation *= Quaternion.Euler(Random.Range(-moveScript.ghostShakeAmount, moveScript.ghostShakeAmount), Random.Range(-moveScript.ghostShakeAmount, moveScript.ghostShakeAmount), 0);
            }
        }
        
    }
}
