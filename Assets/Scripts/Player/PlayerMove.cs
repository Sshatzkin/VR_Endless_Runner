using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

public class PlayerMove : MonoBehaviour
{

    public float moveSpeed = 5f;
    public float strafeSpeed = 4f;

    public JumpController jumpController;

   //public SteamVR_Action_Boolean jumpAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed, Space.World);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (this.gameObject.transform.position.x > LevelBoundary.leftSide)
            {
              transform.Translate(Vector3.left * Time.deltaTime * strafeSpeed, Space.World);
            }
            
        }

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (this.gameObject.transform.position.x < LevelBoundary.rightSide)
            {
                transform.Translate(Vector3.right * Time.deltaTime * strafeSpeed, Space.World);
            }
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpController.jump();
        }

    }

}
