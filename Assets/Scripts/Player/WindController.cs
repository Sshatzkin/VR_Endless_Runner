using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    public GameObject audioObject;
    public AudioSource windFX;

    public ParticleSystem wind;

    public JumpController jumpController;

    public ArduinoController arduino;
    [SerializeField]
    private bool alreadyJumped = false;

    // Start
    void Start()
    {
        audioObject = GameObject.Find("WindSound");
        windFX = audioObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(arduino != null) {
            arduino.SetWeightTop();
            arduino.SetDCTime(50, 150);
            arduino.SetDriveTime(8,10);
            arduino.Arm();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (jumpController.jumpState == 4)
        {
            Debug.Log("Wind zone entered");
            CreateWind();
            alreadyJumped = true;
        }
        if((jumpController.jumpState == 1 || jumpController.jumpState == 9) && alreadyJumped) {
            Destroy(gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(arduino != null) {
            arduino.Disarm();
            Debug.Log("Destroy worked");
        }
    }

    void CreateWind()
    {
        wind.Play();
        windFX.Play();
        Debug.Log("Created Wind");
    }
}
