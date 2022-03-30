using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPumpkin : MonoBehaviour
{
    public GameObject audioObject;
    public GameObject audioObjectBoing;
    public AudioSource pumpkinFX;
    public AudioSource boingFX;

    public JumpController jumpController;

    public ArduinoController arduino;

    // Start
    void Start()
    {
        audioObject = GameObject.Find("PumpkinCollect");
        pumpkinFX = audioObject.GetComponent<AudioSource>();
        audioObjectBoing = GameObject.Find("BoingSound");
        boingFX = audioObjectBoing.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (JumpController.smashPower > 0 && jumpController.jumpState >=3 && jumpController.jumpState < 9)
        {
            pumpkinFX.Play();
            CollectibleController.pumpkinCount++;
            Destroy(gameObject);
            Debug.Log("Pumpkin smashed");
        }
        else{
            // no powerup, consider this as normal obstacle
            // VRMove.shakeDuration = 1.5f;
            boingFX.Play();
            Physics.IgnoreCollision(other, GetComponent<Collider>(), true);
            Debug.Log("No Pumpkin smashed");
        }
    }

    void OnTriggerExit(Collider other)
    {
        // arduino.Disarm();
    }
}
