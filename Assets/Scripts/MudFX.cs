using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudFX : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource mudFX;

    public JumpController jumpController;

    public static bool walkingOnMud = false;
    // Start
    void Start()
    {
        audioObject = GameObject.Find("MudSound");
        mudFX = audioObject.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        mudFX.Play();
        walkingOnMud = true;
        Debug.Log("Walking on mud");
    }

    void OnTriggerExit(Collider other)
    {
        mudFX.Stop();
        walkingOnMud = false;
    }
}
