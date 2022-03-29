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

    // Start
    void Start()
    {
        audioObject = GameObject.Find("WindSound");
        windFX = audioObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        arduino.SetWeightTop();
        arduino.SetDCTime(50, 150);
        arduino.SetDriveTime(8,10);
        arduino.Arm();
    }

    void OnTriggerStay(Collider other)
    {
        if (jumpController.jumpState >= 6)
        {
            Debug.Log("Wind zone entered");
            CreateWind();
            Destroy(gameObject);
        }
    }

    void CreateWind()
    {
        wind.Play();
        windFX.Play();
    }
}
