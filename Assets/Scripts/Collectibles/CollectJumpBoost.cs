using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectJumpBoost : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource boostFX;

    // Start
   void Start()
    {
        audioObject = GameObject.Find("BoosterCollect");
        boostFX = audioObject.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        boostFX.Play();
        JumpController.powerLevel++;

        Destroy(gameObject);
        
    }
}
