using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectSmashPower : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource boostFX;

    public ArduinoController arduino;

    // Start
   void Start()
    {
        audioObject = GameObject.Find("BoosterCollect");
        boostFX = audioObject.GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        boostFX.Play();
        JumpController.smashPower += 2;
        // arduino.SetWeightTop();
        arduino.SendManual(-.3f, 1000);
        arduino.SetDCTime(50, 150);
        arduino.SetDriveTime(6,8);
        Destroy(gameObject);
        
    }
}
