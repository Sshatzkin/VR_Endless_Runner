using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectJumpBoost : MonoBehaviour
{
    //public GameObject audioObject;
    //public AudioSource coinFX;

    // Start
   /*void Start()
    {
        audioObject = GameObject.Find("JumpCollect");
        jumpFX = audioObject.GetComponent<AudioSource>();
        console.log(""Sta)
    }*/
    
    void OnTriggerEnter(Collider other)
    {
        //jumpFX.Play();
        JumpController.powerLevel++;

        Destroy(gameObject);
        
    }
}
