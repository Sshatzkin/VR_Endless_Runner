using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectJumpBoost : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource coinFX;

    // Start
    void Start()
    {
        audioObject = GameObject.Find("JumpCollect");
        coinFX = audioObject.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        coinFX.Play();
        JumpController.powerLevel++;

        Destroy(gameObject);
        
    }
}
